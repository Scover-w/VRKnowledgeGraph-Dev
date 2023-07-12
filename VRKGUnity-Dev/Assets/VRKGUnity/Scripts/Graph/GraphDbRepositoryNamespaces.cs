using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using VDS.RDF;

public class GraphDbRepositoryNamespaces
{
    [JsonIgnore]
    public IReadOnlyDictionary<string, OntologyTree> OntoTreeDict => _ontoTreeDict;


    [JsonProperty("UrisWithPrefixs_")]
    Dictionary<string, string> _namespacesAndPrefixs;

    Dictionary<string, OntologyTree> _ontoTreeDict;

    [JsonIgnore]
    private static string _fullpathFile;
    [JsonIgnore]
    private static string _pathRepo;

    [JsonIgnore]
    Dictionary<string, string> _defaultPrefixsDict;


    public GraphDbRepositoryNamespaces()
    {
        _ontoTreeDict = new();
        _namespacesAndPrefixs = new();
    }

    public async Task RetrieveNewNamespaces(JObject data,GraphDBAPI graphDBAPI)
    {
        HashSet<string> namespaces = new();
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
                namespaces.Add(uri.namespce);
            }

            if (pType == "uri" && pValue.StartsWith("http"))
            {
                var uri = pValue.ExtractUri();
                namespaces.Add(uri.namespce);
            }

            if (oType == "uri" && oValue.StartsWith("http"))
            {
                var uri = oValue.ExtractUri();
                namespaces.Add(uri.namespce);
            }
        }

        LoadDefaultPrefixsList();


        foreach (string namespce in namespaces)
        {
            if(_namespacesAndPrefixs.ContainsKey(namespce))
                continue;

            await TryRetrieveOntologyAndLoadInDatabase(graphDBAPI, _pathRepo, namespce);
            TryLoadPrefixFromList(namespce);
        }

        await Save();
    }


    private async Task TryRetrieveOntologyAndLoadInDatabase(GraphDBAPI graphDBAPI, string pathRepo, string namespce)
    {
        string xmlContent = await HttpHelper.RetrieveRdf(namespce);

        if (xmlContent.Length == 0)
            return;

        IGraph graph = new VDS.RDF.Graph();

        if (!graph.TryLoadFromRdf(xmlContent))
            return;


        await FileHelper.SaveAsync(xmlContent, pathRepo, namespce.CleanUriFromUrlPart() + ".rdf");
        graph.CleanFromLabelAndComment();
        string turtleContent = graph.ToTurtle();
        await graphDBAPI.LoadFileContentInDatabase(turtleContent, GraphDBAPIFileType.Turtle);
    }

    private void TryLoadPrefixFromList(string namespce)
    {
        if(_defaultPrefixsDict.ContainsKey(namespce))
        {
            AddPrefix(namespce, _defaultPrefixsDict[namespce]);
            return;
        }

        string prefix = CreatePrefix(namespce);
        AddPrefix(namespce, prefix);

        Debug.LogWarning("namepsace " + namespce + " couldn't be found in the default prefix list. Created " + prefix + ".");
    }

    private string CreatePrefix(string namespce)
    {
        var prefix = namespce.Replace("http://", "")
                             .Replace("https://", "");

        prefix = prefix.Split("/")[0];

        prefix = prefix.Replace("www.", "")
              .Replace(".com", "")
              .Replace(".fr", "")
              .Replace(".org", "");

        prefix = CapitalizeOnCharacter(prefix, '.');
        prefix = CapitalizeOnCharacter(prefix, '-');
        prefix = CapitalizeOnCharacter(prefix, '_');

        return prefix;
    }

    private void AddPrefix(string namespce, string prefix)
    {

        if(namespce.Contains("ark:"))
        {
            _namespacesAndPrefixs.Add(namespce, prefix);
            return;
        }


        HashSet<string> existingPrefixs = new();

        foreach(string existingPrefix in _namespacesAndPrefixs.Values)
        {
            existingPrefixs.Add(existingPrefix);
        }

        if (!existingPrefixs.Contains(prefix))
        {
            _namespacesAndPrefixs.Add(namespce, prefix);
            return;
        }

        int i = 1;
        string prefixNumbered;

        do
        {
            i++;
            prefixNumbered = prefix + i;


        } while (existingPrefixs.Contains(prefixNumbered));

        _namespacesAndPrefixs.Add(namespce, prefixNumbered);
    }

    // Exemple : purl.ontology.test -> purlOntologyTest
    public string CapitalizeOnCharacter(string input, char separator)
    {
        string[] words = input.Split(separator);

        for (int i = 1; i < words.Length; i++)
        {
            words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]);
        }

        return string.Concat(words);
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
                    continue;
                }

                if (!nameSpceA.StartsWith("http"))
                    continue;

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
        foreach (var ontologyTree in _ontoTreeDict.Values)
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

        ontoNode.NodesAttached.Add(definedNode);
        return true;
    }

    public bool CanAddNodeToOntoNode(Node simpleOntoNode, out OntoNode ontoNode)
    {
        ontoNode = null;
        if (!_ontoTreeDict.TryGetValue(simpleOntoNode.Value.ExtractUri().namespce, out OntologyTree ontoTree))
        {
            return false;
        }

        if (!ontoTree.TryGetOntoNode(simpleOntoNode.Id, out ontoNode))
        {
            return false;
        }

        return true;
    }

    public void ResetDefinedNodes()
    {
        foreach(var ontoUri in _ontoTreeDict)
        {
            ontoUri.Value.ResetDefinedNodes();
        }
    }

    public async Task BlockNamespaceToBeRetrieved(Dictionary<int, Node> nodeIds)
    {
        HashSet<string> newNamespaces = new();

        foreach(Node node in nodeIds.Values)
        {
            var value = node.Value;

            if (!value.StartsWith("http"))
                continue;

            var namepsce = node.Value.ExtractUri().namespce;

            if (newNamespaces.Contains(namepsce))
                continue;

            newNamespaces.Add(namepsce);
        }


        foreach(string namespce in newNamespaces)
        {
            if (_namespacesAndPrefixs.ContainsKey(namespce))
                continue;

            _namespacesAndPrefixs.Add(namespce, "");
        }

        await Save();
    }


    private void LoadDefaultPrefixsList()
    {
        var path = Path.Combine(Application.streamingAssetsPath, "prefixsList.ttl");


        _defaultPrefixsDict = new();

        if (!File.Exists(path))
        {
            Debug.LogError("prefixsList.ttl has been deleted");
            return;
        }

        using (StreamReader reader = new StreamReader(path))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {

                line = line.Replace("@prefix ", "");

                var elements = line.Split("<");

                string prefix = elements[0];
                string namespce = elements[1];


                prefix = prefix.Replace(" ", "").Replace(":", "");
                namespce = namespce.Replace(">", "");
                namespce = namespce.Substring(0, namespce.Length - 1);

                if (_defaultPrefixsDict.ContainsKey(namespce))
                    continue;

                _defaultPrefixsDict.Add(namespce, prefix);
            }
        }
    }

    #region SAVE_LOAD
    public async static Task<GraphDbRepositoryNamespaces> Load(string pathRepo)
    {
        _pathRepo = pathRepo;
        SetPath(_pathRepo);

        if (File.Exists(_fullpathFile))
        {
            string json = await File.ReadAllTextAsync(_fullpathFile);
            var repoNamespaces = JsonConvert.DeserializeObject<GraphDbRepositoryNamespaces>(json);

            return repoNamespaces;
        }


        var repoNamespacesB = new GraphDbRepositoryNamespaces();
        await repoNamespacesB.Save();
        return repoNamespacesB;
    }

    public async Task Save()
    {
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(_fullpathFile, json);
    }

    private static void SetPath(string pathRepo)
    {
        _fullpathFile = Path.Combine(pathRepo, "GraphDbRepositoryNamespaces.json");
    }
    #endregion

}