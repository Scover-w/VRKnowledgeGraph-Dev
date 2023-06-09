using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using UnityEngine;

public class GraphDbRepositoryUris
{
    [JsonIgnore]
    public IReadOnlyDictionary<string, OntologyTree> OntoTreeDict => _ontoTreeDict;

    [JsonProperty("OntoUris_")]
    HashSet<string> _ontoUris;

    [JsonProperty("Uris_")]
    HashSet<string> _uris;

    Dictionary<string, OntologyTree> _ontoTreeDict;

    private static string _fullpathFile;
    private static string _pathRepo;

    public GraphDbRepositoryUris()
    {
        _ontoTreeDict = new();
        _uris = new();
        _ontoUris = new();
    }

    public async Task RetrieveNewUris(JObject data,GraphDBAPI graphDBAPI)
    {
        HashSet<string> uris = new();

        // Detect all uris
        foreach (JToken binding in data["results"]["bindings"])
        {
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string pType = binding["p"]["type"].Value<string>();
            string pValue = binding["p"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();


            if (sType == "uri" && sValue.StartsWith("http"))
            {
                var uri = sValue.ExtractUri();
                uris.Add(uri.namespce);
            }

            if (pType == "uri" && pValue.StartsWith("http"))
            {
                var uri = pValue.ExtractUri();
                uris.Add(uri.namespce);
            }

            if (oType == "uri" && oValue.StartsWith("http"))
            {
                var uri = oValue.ExtractUri();
                uris.Add(uri.namespce);
            }
        }


        foreach(string uri in uris)
        {
            if(_ontoTreeDict.ContainsKey(uri) || _uris.Contains(uri))
            {
                continue;
            }

            // For each new uris
            var ontologyTree = await OntologyTree.TryCreateOntologyAndLoadInDatabase(graphDBAPI, _pathRepo, uri);

            if (ontologyTree == null)
            {
                _uris.Add(uri);
                continue;
            }

            _ontoTreeDict.Add(uri, ontologyTree);
            _ontoUris.Add(uri);
        }

        await Save();

    }


    public bool IsUriAnOnto(string nodeValue)
    {
        string namespce = nodeValue.ExtractUri().namespce;

        return _ontoTreeDict.ContainsKey(namespce);
    }

    public void AddNodeToOntoNode(Node definedNode, Node simpleOntoNode)
    {
        if(!_ontoTreeDict.TryGetValue(simpleOntoNode.Value.ExtractUri().namespce, out OntologyTree ontoTree))
        {
            Debug.LogWarning("GraphDbRepositoryUris : couldn't get value.");
            return;
        }

        var ontoNode = ontoTree.GetOntoNode(simpleOntoNode.Id);
        ontoNode.NodesDefined.Add(definedNode);
    }

    public void ResetDefinedNodes()
    {
        foreach(var ontoUri in _ontoTreeDict)
        {
            ontoUri.Value.ResetDefinedNodes();
        }
    }

    #region SAVE_LOAD
    public async static Task<GraphDbRepositoryUris> Load(string pathRepo)
    {
        _pathRepo = pathRepo;
        SetPath(_pathRepo);

        if (File.Exists(_fullpathFile))
        {
            string json = await File.ReadAllTextAsync(_fullpathFile);
            var graphOnto = JsonConvert.DeserializeObject<GraphDbRepositoryUris>(json);

            await graphOnto.ReloadExistingOntologies();

            return graphOnto;
        }


        var graphOntoB = new GraphDbRepositoryUris();
        await graphOntoB.Save();
        return graphOntoB;
    }

    public async Task Save()
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(_fullpathFile, json);
    }

    private static void SetPath(string pathRepo)
    {
        _fullpathFile = Path.Combine(pathRepo, "GraphDbRepositoryUris.json");
    }

    public async Task ReloadExistingOntologies()
    {
        _ontoTreeDict = new();
        foreach (var uri in _ontoUris) 
        {
            var ontoTree = await OntologyTree.CreateByReloadingOntology(_pathRepo, uri);
            _ontoTreeDict.Add(uri, ontoTree);
        }
    }
    #endregion

}

public enum UriType
{
    None,
    Ontology
}