using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

    Dictionary<string, int> _namepsceFailedAttempt;
    Dictionary<string, int> _namepsceInUse;
    const int _maxFailedAttempt = 5;

    public GraphDbRepositoryDistantUris()
    {
        _distantUriLabels = new();
    }


    public async Task RetrieveNames(JObject data, IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {
        var nodges = data.ExtractNodgesForDistantUri();

        await RetrieveNames(nodges.NodesDicId, ontoTreeDict);
    }

    public async Task RetrieveNames(Dictionary<int, Node> idAndNodes, IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {
        idAndNodes = idAndNodes.GetNoLabeledNodes();
        idAndNodes = idAndNodes.GetNoOntoUriNodes(ontoTreeDict);


        if (idAndNodes.Count == 0)
            return;

        _nbFinishedThread = 0;

        _nbNodes = idAndNodes.Count;

        _namepsceFailedAttempt = new();
        _namepsceInUse = new();

        Debug.Log("RetrieveNames Count " + _nbNodes);

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new SemaphoreSlim(0);

        foreach (var idAndNode in idAndNodes)
        {
            var node = idAndNode.Value;

            tasks.Add(Task.Run(async () =>
            {
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
        string namespce = "";
        try
        {
            var node = (Node)obj;
            string uri = node.Value;

            // Set value if already successfull retrieve saved
            lock (_distantUriLabels)
            {
                if (_distantUriLabels.TryGetValue(uri, out var propAndValue))
                {

                    if(propAndValue.Item1 != "-1")
                        node.Properties.Add(propAndValue.Item1, propAndValue.Item2);

                    return;
                }
            }

            if (!uri.StartsWith("http"))
                return;

            namespce = uri.ExtractUri().namespce;


            int nbInUse = -1;
            bool inUse = false;


            //do
            //{
            //    lock (_namepsceInUse)
            //    {
            //        if (_namepsceInUse.TryGetValue(namespce, out nbInUse))
            //        {
            //            if (nbInUse < _maxFailedAttempt)
            //            {
            //                nbInUse++;
            //                _namepsceInUse[namespce] = nbInUse;
            //                inUse = true;
            //            }
            //        }
            //        else
            //        {
            //            _namepsceInUse.Add(namespce, 1);
            //            inUse = true;
            //        }
            //    }

            //    if(!inUse)
            //        Thread.Sleep(1000);

            //}while(!inUse);
            



            // Don't TryRetrieve if max attempt for namespace reached
            //lock(_namepsceFailedAttempt)
            //{
            //    if(_namepsceFailedAttempt.TryGetValue(namespce, out int nbFailedAttempt))
            //    {
            //        Debug.Log("If failedattempt : " + nbFailedAttempt);
            //        if (nbFailedAttempt >= _maxFailedAttempt)
            //        {
            //            _namepsceInUse[namespce] = _namepsceInUse[namespce]--;
            //            return;
            //        }
            //    }
            //}

            string xmlContent = await HttpHelper.RetrieveRdf(uri);

            if (xmlContent == null || xmlContent.Length == 0)
            {
                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(uri, ("-1", "-1"));
                }

                //AddFailedAttempt(namespce);
                //_namepsceInUse[namespce] = _namepsceInUse[namespce]--;
                return;
            }

            if (ExtractName(xmlContent, out string property, out string value))
            {
                Debug.Log("Extracted " + uri + "  , " + property + " " + value);
                node.Properties.Add(property, value);


                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(uri, (property, value));
                }
            }
            else
            {
                lock (_distantUriLabels)
                {
                    _distantUriLabels.Add(uri, ("-1", "-1"));
                }

                //AddFailedAttempt(namespce);
            }

            //_namepsceInUse[namespce] = _namepsceInUse[namespce]--;
        }
        catch(Exception ex) 
        {
            //lock (_namepsceInUse)
            //    _namepsceInUse[namespce] = _namepsceInUse[namespce]--;
        }

        void AddFailedAttempt(string namespce)
        {
            lock (_namepsceFailedAttempt)
            {
                if (_namepsceFailedAttempt.TryGetValue(namespce, out int nbFailedAttempt))
                {
                    _namepsceFailedAttempt[namespce] = nbFailedAttempt + 1;
                    Debug.Log("Failed attempt : " + _namepsceFailedAttempt[namespce]);
                }
                else
                    _namepsceFailedAttempt.Add(namespce, 1);
            }
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
