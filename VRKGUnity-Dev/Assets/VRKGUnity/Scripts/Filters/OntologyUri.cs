using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

public class OntologyUri
{
    [JsonIgnore]
    public string Uri { get { return _uri; } }
    [JsonIgnore]
    public bool IsOntology { get { return _isOntology; } }

    [JsonIgnore]
    public OntologyTree OntologyTree { get { return _ontologyTree; } }


    OntologyTree _ontologyTree;

    [JsonProperty("Uri_")]
    string _uri;
    [JsonProperty("IsOntology_")]
    bool _isOntology;


    public OntologyUri(string uri)
    {
        _uri = uri;
        _isOntology = false;
    }

    public async Task TryCreateOntologyAndLoadInDatabase(GraphDBAPI graphDBAPI, string pathRepo)
    {
        RdfXmlParser parser = new RdfXmlParser();
        IGraph graph = new VDS.RDF.Graph();
        string xmlContent = await HttpHelper.RetrieveRdf(_uri);

       
        _isOntology = true;

        try
        {
            using (StringReader reader = new StringReader(xmlContent))
            {
                parser.Load(graph, reader);
            }
        }
        catch (RdfParseException e)
        {
            _isOntology = false;
        }

        if (!_isOntology)
            return;

        await FileHelper.SaveAsync(xmlContent, pathRepo, CleanUri(_uri) + ".rdf");

        graph.Clean();

        await graphDBAPI.LoadOntologyInDatabase(graph);

        _ontologyTree = await OntologyTree.CreateAsync(graph, _uri);
    }

    public async Task ReloadOntology(string pathRepo)
    {
        if(!_isOntology) 
            return;

        RdfXmlParser parser = new RdfXmlParser();
        IGraph graph = new VDS.RDF.Graph();
        string xmlContent = await FileHelper.LoadAsync(pathRepo, CleanUri(_uri) + ".rdf");

        try
        {
            using (StringReader reader = new StringReader(xmlContent))
            {
                parser.Load(graph, reader);
            }
        }
        catch (RdfParseException e)
        {
            Debug.LogWarning("OntologyUri : failed parse alreadyparsed content.");
            Debug.LogWarning(e);
            return;
        }

        graph.Clean();
        _ontologyTree = await OntologyTree.CreateAsync(graph,_uri);
    }

    private string CleanUri(string uri)
    {
        return uri.Replace("http://", "").Replace("/", "").Replace(".", "").Replace("\\", "").Replace("#","");
    }
}
