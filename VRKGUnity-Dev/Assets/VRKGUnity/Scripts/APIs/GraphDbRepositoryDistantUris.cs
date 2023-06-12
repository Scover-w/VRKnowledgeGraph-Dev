using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlasticGui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

public class GraphDbRepositoryDistantUris
{
    [JsonIgnore]
    public IReadOnlyDictionary<string, (string, string)> DistantUriLabels => _distantUriLabels;

    [JsonProperty("DistantUriLabels_")]
    Dictionary<string, (string,string)> _distantUriLabels; // <uri,(propName, valueProp)> -> <http://viaf.org/viaf/143903205, (skos:prefLabel, Bibliothèque nationale (Francia))>

    [JsonIgnore]
    int _nbFinishedThread;
    [JsonIgnore]
    int _nbNodes;

    [JsonIgnore]
    private static string _fullpathFile;

    public GraphDbRepositoryDistantUris()
    {
        _distantUriLabels = new();
    }


    public async Task RetrieveNames(JObject data, ReadOnlyHashSet<string> ontoUris)
    {
        var nodges = data.ExtractNodgesForDistantUri();

        await RetrieveNames(nodges.NodesDicId, ontoUris);
    }

    public async Task RetrieveNames(Dictionary<int, Node> idAndNodes, ReadOnlyHashSet<string> ontoUris)
    {
        idAndNodes = idAndNodes.GetNoLabeledNodes();
        idAndNodes = idAndNodes.GetNoOntoUriNodes(ontoUris);


        if (idAndNodes.Count == 0)
            return;

        _nbFinishedThread = 0;

        _nbNodes = idAndNodes.Count;

        Debug.Log("RetrieveNames Count " + _nbNodes);

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new SemaphoreSlim(0);

        foreach (var idAndNode in idAndNodes)
        {
            tasks.Add(Task.Run(async () =>
            {

                var node = idAndNode.Value;
                await RetrieveName(node);

                // Increment the finished thread count atomically
                if (Interlocked.Increment(ref _nbFinishedThread) == _nbNodes)
                {
                    // Signal the semaphore when all threads have finished
                    semaphore.Release();
                }
            }));
        }

        await semaphore.WaitAsync();

        await Save();
        Debug.Log("Finished");
    }

    private async Task RetrieveName(object obj)
    {

        try
        {
            var node = (Node)obj;

            lock (_distantUriLabels)
            {
                if (_distantUriLabels.TryGetValue(node.Value, out var propAndValue))
                {

                    if(propAndValue.Item1 != "-1")
                        node.Properties.Add(propAndValue.Item1, propAndValue.Item2);

                    return;
                }
            }

            string xmlContent = await HttpHelper.RetrieveRdf(node.Value);

            if (xmlContent == null || xmlContent.Length == 0)
            {
                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(node.Value, ("-1", "-1"));
                }
                return;
            }

            if (ExtractName(xmlContent, out string property, out string value))
            {
                Debug.Log("Extracted " + node.Value + "  , " + property + " " + value);
                node.Properties.Add(property, value);


                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(node.Value, (property, value));
                }
            }
            else
            {
                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(node.Value, ("-1", "-1"));
                }
            }
        }
        catch(Exception ex) 
        {

        }
    }

    private bool ExtractName(string xmlContent, out string property, out string value)
    {
        property = "";
        value = "";


        try
        {
            using (StringReader stringReader = new StringReader(xmlContent))

            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    var name = xmlReader.Name.ToLower();

                    if (!(name.Contains("label") || name.Contains("title") || name.Contains("name")))
                        continue;

                    property = xmlReader.Name;
                    value = xmlReader.ReadElementContentAsString();

                    return true;
                }
            }
        }
        catch(Exception e) 
        {
            return false;    
        }
        

        return false;
    }


    #region SAVE_LOAD
    public static async Task<GraphDbRepositoryDistantUris> LoadAsync(string pathRepo)
    {
        SetPath(pathRepo);

        if (File.Exists(_fullpathFile))
        {
            string json = await File.ReadAllTextAsync(_fullpathFile);
            var graphDistantUri = JsonConvert.DeserializeObject<GraphDbRepositoryDistantUris>(json);
            return graphDistantUri;
        }

        var graphDistantUriB = new GraphDbRepositoryDistantUris();
        await graphDistantUriB.Save();
        return graphDistantUriB;
    }

    public async Task Save()
    {
        string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        await File.WriteAllTextAsync(_fullpathFile, json);
    }

    private static void SetPath(string pathRepo)
    {
        _fullpathFile = Path.Combine(pathRepo, "GraphDbRepositoryDistantUris.json");
    }
    #endregion
}
