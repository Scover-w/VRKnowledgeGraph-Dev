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

/// <summary>
/// JSONAble, contain all the distants uris retrieved from internet.
/// Always used with <see cref="GraphDbRepository"/>
/// </summary>
public class GraphDbRepositoryDistantUris
{
    [JsonIgnore]
    public IReadOnlyDictionary<string, (string, string)> DistantUriLabels => _distantUriLabels;

    [JsonProperty("DistantUriLabels_")]
    readonly Dictionary<string, (string,string)> _distantUriLabels; // <uri,(propName, valueProp)> -> <http://viaf.org/viaf/143903205, (skos:prefLabel, Bibliothèque nationale (Francia))>


    private static string _fullpathFile;

    int _nbFinishedThread;
    int _nbPackFinishedThread;
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

        await RetrieveNames(nodges.NodesDicUID, ontoTreeDict, dataSynchro);
    }

    public async Task RetrieveNames(Dictionary<string, Node> uidAndNodes, IReadOnlyDictionary<string, OntologyTree> ontoTreeDict, DataSynchroManager dataSynchro)
    {
        uidAndNodes = uidAndNodes.GetNoLabeledNodes();
        uidAndNodes = uidAndNodes.GetNoOntoUriNodes(ontoTreeDict);


        if (uidAndNodes.Count == 0)
            return;

        _nbNodes = uidAndNodes.Count;

        var data = new LoadingDistantUriData(_nbNodes,true);
        dataSynchro.DataQueue.Enqueue(data);

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new(0);


        _nbPackFinishedThread = 0;
        _nbFinishedThread = 0;
        _nbPackbThreadToLaunch = Mathf.Clamp(_nbNodes - _nbFinishedThread, 0, MAX_THREAD);
        _nbStarted = 0;


        foreach (var idAndNode in uidAndNodes)
        {
            var node = idAndNode.Value;

            tasks.Add(Task.Run(async () =>
            {
                await RetrieveName(node, dataSynchro);
                Interlocked.Increment(ref _nbFinishedThread);

                LoadingDistantUriData data = new(_nbFinishedThread);
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

    private async Task RetrieveName(object nodeObj, DataSynchroManager dataSynchroObj)
    {
        var node = (Node)nodeObj;
        DataSynchroManager dataSynchro = dataSynchroObj;

        string uri = node.Uri;


        try
        {
            

            // Set value if already successfull retrieve saved
            bool needReturn = false;

            if (!uri.StartsWith("http"))
                return;

            lock (_distantUriLabels)
            {
                if (_distantUriLabels.TryGetValue(uri, out var propAndValue))
                {

                    if(propAndValue.Item1 != "-1")
                    {
                        node.Properties.Add(propAndValue.Item1, propAndValue.Item2);
                    }

                    needReturn = true;
                }
            }

            if (needReturn)
                return;

            string namespce = uri.ExtractUri().namespce;

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
                dataSynchro.AddLog("Récupération réussie pour " + node.Uri + " : " + value);

                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(uri, (property, value));
                }

                return;
            }
            else
            {
                dataSynchro.AddLog("Récupération échouée pour " + node.Uri);
                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(uri, ("-1", "-1"));
                }

                return;
            }
        }
        catch(Exception e ) 
        {
            Debug.Log(uri + e);
            return;
        }
    }

    private bool ExtractName(string xmlContent, out string property, out string value)
    {
        property = "";
        value = "";


        try
        {
            using StringReader stringReader = new(xmlContent);
            using XmlReader xmlReader = XmlReader.Create(stringReader);

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
        catch(Exception) 
        {
            return false;    
        }
        

        return false;
    }


    #region SAVE_LOAD
    public static async Task<GraphDbRepositoryDistantUris> Load(string pathRepo)
    {
        SetPath(pathRepo);

        if (File.Exists(_fullpathFile))
        {
            string json = await File.ReadAllTextAsync(_fullpathFile);
            var graphDistantUri = await JsonConvertHelper.DeserializeObjectAsync<GraphDbRepositoryDistantUris>(json);
            return graphDistantUri;
        }

        var graphDistantUriB = new GraphDbRepositoryDistantUris();
        await graphDistantUriB.Save();
        return graphDistantUriB;
    }

    public async Task Save()
    {
        string json = await JsonConvertHelper.SerializeObjectAsync(this, Newtonsoft.Json.Formatting.Indented);
        await File.WriteAllTextAsync(_fullpathFile, json);
    }

    private static void SetPath(string pathRepo)
    {
        _fullpathFile = Path.Combine(pathRepo, "GraphDbRepositoryDistantUris.json");
    }

    /// <summary>
    /// /!\ Be carefull with this function, it will permanently delete the file associated to this class
    /// </summary>
    public void DeleteFile()
    {
        if (!File.Exists(_fullpathFile))
            return;

        File.Delete(_fullpathFile);
    }
    #endregion
}
