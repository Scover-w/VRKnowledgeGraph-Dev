using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class GraphDbRepositoryDistantUris
{
    [JsonIgnore]
    public IReadOnlyDictionary<string, (string, string)> DistantUriLabels => _distantUriLabels;

    [JsonProperty("DistantUriLabels_")]
    Dictionary<string, (string,string)> _distantUriLabels; // <uri,(propName, valueProp)> -> <http://viaf.org/viaf/143903205, (skos:prefLabel, Bibliothèque nationale (Francia))>


    private static string _fullpathFile;

    int _nbFinishedThread;
    int _nbPackFinishedThread;
    int _nbPackThread;
    int _nbStarted;
    int _nbPackbThreadToLaunch;

    int _nbNodes;

    const int MAX_THREAD = 10;

    public GraphDbRepositoryDistantUris()
    {
        _distantUriLabels = new();
    }


    public async Task RetrieveNames(JObject data, IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, DataSynchroManager dataSynchro, GraphDbRepositoryNamespaces repoNamespaces)
    {
        var nodges = data.ExtractNodgesForDistantUri(repoNamespaces);

        await RetrieveNames(nodges.NodesDicId, ontoTreeDict, dataSynchro);
    }

    public async Task RetrieveNames(Dictionary<int, Node> idAndNodes, IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, DataSynchroManager dataSynchro)
    {
        idAndNodes = idAndNodes.GetNoLabeledNodes();
        idAndNodes = idAndNodes.GetNoOntoUriNodes(ontoTreeDict);


        if (idAndNodes.Count == 0)
            return;

        _nbNodes = idAndNodes.Count;

        var data = new LoadingDistantUriData(_nbNodes,true);
        dataSynchro.DataQueue.Enqueue(data);

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new SemaphoreSlim(0);


        _nbPackThread = 0;
        _nbPackFinishedThread = 0;
        _nbFinishedThread = 0;
        _nbPackbThreadToLaunch = Mathf.Clamp(_nbNodes - _nbFinishedThread, 0, MAX_THREAD);
        _nbStarted = 0;


        foreach (var idAndNode in idAndNodes)
        {
            var node = idAndNode.Value;

            tasks.Add(Task.Run(async () =>
            {
                await RetrieveName(node);
                Interlocked.Increment(ref _nbFinishedThread);

                LoadingDistantUriData data = new LoadingDistantUriData(_nbFinishedThread);
                dataSynchro.DataQueue.Enqueue(data);

                // Increment the finished thread count atomically
                if (Interlocked.Increment(ref _nbPackFinishedThread) == _nbPackbThreadToLaunch)
                {
                    semaphore.Release();
                }
            }));

            _nbStarted++;

            if (_nbStarted == _nbPackbThreadToLaunch)
            {
                await semaphore.WaitAsync();
                _nbPackFinishedThread = 0;
                _nbStarted = 0;
                _nbPackbThreadToLaunch = Mathf.Clamp(_nbNodes - _nbFinishedThread, 0, MAX_THREAD);
                semaphore = new SemaphoreSlim(0);
            }
        }

        await Save();
        Debug.Log("Finished");
    }

    private async Task RetrieveName(object obj)
    {
        string namespce = "";
        try
        {
            var node = (Node)obj;
            string uri = node.Uri;

            // Set value if already successfull retrieve saved
            bool needReturn = false;

            if (!uri.StartsWith("http"))
                return;

            lock (_distantUriLabels)
            {
                if (_distantUriLabels.TryGetValue(uri, out var propAndValue))
                {

                    if(propAndValue.Item1 != "-1")
                        node.Properties.Add(propAndValue.Item1, propAndValue.Item2);

                    needReturn = true;
                }
            }

            if (needReturn)
                return;

            namespce = uri.ExtractUri().namespce;

            string xmlContent = null;

            xmlContent = await HttpHelper.RetrieveRdf(uri);


            if (xmlContent == null || xmlContent.Length == 0)
            {
                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(uri, ("-1", "-1"));
                }

                return;
            }


            if (ExtractName(xmlContent, out string property, out string value))
            {
                node.Properties.Add(property, value);


                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(uri, (property, value));
                }

                return;
            }
            else
            {
                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(uri, ("-1", "-1"));
                }

                return;
            }
        }
        catch(Exception ex) 
        {
            return;
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
