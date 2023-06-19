using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using UnityEngine;
using VDS.RDF;

public class GraphDbRepositoryUris
{
    [JsonIgnore]
    public IReadOnlyDictionary<string, OntologyTree> OntoTreeDict => _ontoTreeDict;


    [JsonProperty("Uris_")]
    HashSet<string> _uris;

    Dictionary<string, OntologyTree> _ontoTreeDict;

    private static string _fullpathFile;
    private static string _pathRepo;

    public GraphDbRepositoryUris()
    {
        _ontoTreeDict = new();
        _uris = new();
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
            if(_uris.Contains(uri))
            {
                continue;
            }

            TryRetrieveOntologyAndLoadInDatabase(graphDBAPI, _pathRepo, uri);
            _uris.Add(uri);
        }

        await Save();

    }


    private async void TryRetrieveOntologyAndLoadInDatabase(GraphDBAPI graphDBAPI, string pathRepo, string uri)
    {
        string xmlContent = await HttpHelper.RetrieveRdf(uri);

        if (xmlContent.Length == 0)
            return;

        IGraph graph = new VDS.RDF.Graph();

        if (!graph.TryLoadFromRdf(xmlContent))
            return;

        await FileHelper.SaveAsync(xmlContent, pathRepo, uri.CleanUriFromUrlPart() + ".rdf");

        graph.CleanFromLabelAndComment();

        string turtleContent = graph.ToTurtle();

        await graphDBAPI.LoadFileContentInDatabase(turtleContent, GraphDBAPIFileType.Turtle);
    }


    public async Task CreateOntologyTrees(GraphDBAPI graphDBAPI)
    {
        var sparqlQuery = "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                           "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> " +
                           "PREFIX owl: <http://www.w3.org/2002/07/owl#> " +
                           "SELECT ?s ?p ?o " +
                           " WHERE {  { " +
                           "?s ?p ?o. " +
                           "FILTER( ( (?p = rdf:type && (?o = rdfs:Class || ?o = owl:Class)) || ?p = rdfs:subClassOf ) )  }}";

        var json = await graphDBAPI.SelectQuery(sparqlQuery);
        var data = JsonConvert.DeserializeObject<JObject>(json);

        _ontoTreeDict = new();

        foreach (JToken binding in data["results"]["bindings"])
        {
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string pType = binding["p"]["type"].Value<string>();
            string pValue = binding["p"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();


            if(pValue == "http://www.w3.org/1999/02/22-rdf-syntax-ns#type")
            {
                var namescpe = sValue.ExtractUri().namespce;

                if (!namescpe.StartsWith("http"))
                    continue;

                var ontologyTree = TryGetOrCreateOntologyTree(namescpe);
                OntoNode ontoNode = new OntoNode(sValue);
                ontologyTree.AddOntoNode(ontoNode);

                continue;
            }


            if (pValue == "http://www.w3.org/2000/01/rdf-schema#subClassOf")
            {
                var nameSpceA = sValue.ExtractUri().namespce;
                var nameSpceB = oValue.ExtractUri().namespce;

                if(nameSpceA != nameSpceB)
                {
                    Debug.Log("Ho ho");
                    continue;
                }

                if (!nameSpceA.StartsWith("http"))
                    continue;

                if (sValue.Contains("http://www.cidoc-crm.org/cidoc-crm/E1_CRM_Entity") || oValue.Contains("http://www.cidoc-crm.org/cidoc-crm/E1_CRM_Entity"))
                    Debug.Log("Bipbop");

                var ontologyTree = TryGetOrCreateOntologyTree(nameSpceA);

                OntoNode sOntoNode = new OntoNode(sValue);
                OntoNode oOntoNode = new OntoNode(oValue);

                sOntoNode = ontologyTree.TryGetOrCreateOntoNode(sOntoNode);
                oOntoNode = ontologyTree.TryGetOrCreateOntoNode(oOntoNode);

                sOntoNode.AddOntoNodeSource(oOntoNode);
                oOntoNode.AddOntoNodeTarget(sOntoNode);
                continue;
            }
        }

        foreach(var ontologyTree in _ontoTreeDict.Values)
        {
            ontologyTree.SetRootAndDepth();
        }
    }

    private OntologyTree TryGetOrCreateOntologyTree(string namescpe)
    {
        if (_ontoTreeDict.TryGetValue(namescpe, out OntologyTree ontologyTree))
            return ontologyTree;

        ontologyTree = new OntologyTree(namescpe);
        _ontoTreeDict.Add(namescpe, ontologyTree);

        return ontologyTree;
    }

    public bool IsUriAnOnto(string nodeValue)
    {
        string namespce = nodeValue.ExtractUri().namespce;

        return _ontoTreeDict.ContainsKey(namespce);
    }

    public bool TryAddNodeToOntoNode(Node definedNode, Node simpleOntoNode)
    {
        if(!_ontoTreeDict.TryGetValue(simpleOntoNode.Value.ExtractUri().namespce, out OntologyTree ontoTree))
        {
            return false;
        }

        if(!ontoTree.TryGetOntoNode(simpleOntoNode.Id, out OntoNode ontoNode))
        {
            return false;
        }

        ontoNode.NodesDefined.Add(definedNode);
        return true;
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
    #endregion

}

public enum UriType
{
    None,
    Ontology
}