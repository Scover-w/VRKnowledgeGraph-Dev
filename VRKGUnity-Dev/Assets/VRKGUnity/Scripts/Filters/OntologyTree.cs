using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Expressions.Functions.XPath.String;
using VDS.RDF.Writing;

public class OntologyTree
{
    OntoNode _rootOntoNode;

    Dictionary<int, OntoNode> _ontoNodes;
    Dictionary<int, OntoEdge> _ontoEdges;

    string _uri;

    private OntologyTree(Dictionary<int, OntoNode> ontoNodes, Dictionary<int, OntoEdge> ontoEdges, string uri)
    {
        _ontoNodes = ontoNodes;
        _ontoEdges = ontoEdges;

        SetRootAndDepth();
        _uri = uri;
    }


    public static async Task<OntologyTree> TryCreateOntologyAndLoadInDatabase(GraphDBAPI graphDBAPI, string pathRepo, string uri)
    {
        string xmlContent = await HttpHelper.RetrieveRdf(uri);

        if (xmlContent.Length == 0)
            return null;


        IGraph graph = new VDS.RDF.Graph();

        if(!graph.TryLoadFromRdf(xmlContent))
            return null;

        await FileHelper.SaveAsync(xmlContent, pathRepo, CleanUri(uri) + ".rdf");

        graph.CleanFromLabelAndComment();

        string turtleContent = graph.ToTurtle();

        await graphDBAPI.LoadFileContentInDatabase(turtleContent, GraphDBAPIFileType.Turtle);

        var ontologyTree = await OntologyTree.CreateAsync(graph, uri);
        return ontologyTree;
    }

    public static async Task<OntologyTree> CreateByReloadingOntology(string pathRepo,string uri)
    {
        RdfXmlParser parser = new RdfXmlParser();
        IGraph graph = new VDS.RDF.Graph();
        string xmlContent = await FileHelper.LoadAsync(pathRepo, CleanUri(uri) + ".rdf");

        try
        {
            using (StringReader reader = new StringReader(xmlContent))
            {
                parser.Load(graph, reader);
            }
        }
        catch (RdfParseException e)
        {
            Debug.LogWarning("OntologyUri : failed parse already parsed content.");
            Debug.LogWarning(e);
            return null;
        }

        graph.CleanFromLabelAndComment();
        var ontologyTree = await CreateAsync(graph, uri);
        return ontologyTree;
    }

    public static async Task<OntologyTree> CreateAsync(IGraph graph, string uri)
    {
        return await Task.Run(() =>
        {
            OntologyTree ontologyTree = CreateOntologyTree(graph, uri);
            return ontologyTree;
        });
    }

    private static OntologyTree CreateOntologyTree(IGraph graph, string uri)
    {
        Dictionary<int, OntoNode> ontoNodes = new();
        Dictionary<int, OntoEdge> ontoEdges = new();


        foreach (Triple triple in graph.Triples)
        {
            string sValue = triple.Subject.ToString();
            string pValue = triple.Predicate.ToString();
            string oValue = triple.Object.ToString();


            if (pValue == "http://www.w3.org/2000/01/rdf-schema#subClassOf")
            {
                AddSubclassOf(sValue, oValue);
                continue;
            }


            bool isDomain = (pValue == "http://www.w3.org/2000/01/rdf-schema#domain");

            if (isDomain || pValue == "http://www.w3.org/2000/01/rdf-schema#range")
            {
                AddEdge(sValue, oValue, isDomain);
                continue;
            }

        }

        if (ontoNodes.Count == 0)
            Debug.Log("cc");

        return new OntologyTree(ontoNodes, ontoEdges, uri);


        void AddSubclassOf(string sValue, string oValue)
        {

            OntoNode ontoNodeSubject = GetOntoNode(sValue);
            OntoNode ontoNodeObject = GetOntoNode(oValue);

            ontoNodeObject.OntoNodeTarget.Add(ontoNodeSubject);
            ontoNodeSubject.OntoNodeSource.Add(ontoNodeObject);
        }

        void AddEdge(string propertyUri, string nodeValue, bool isDomain)
        {
            OntoNode ontoNode = GetOntoNode(nodeValue);
            OntoEdge ontoEdge = GetOntoEdge(propertyUri);

            if (isDomain)
            {
                ontoNode.OntoEdgeSource.Add(ontoEdge);
                ontoEdge.NodeSource.Add(ontoNode);
            }
            else
            {
                ontoNode.OntoEdgeTarget.Add(ontoEdge);
                ontoEdge.NodeTarget.Add(ontoNode);
            }
        }

        OntoEdge GetOntoEdge(string ontoEdgeValue)
        {
            var idOntoEdge = ontoEdgeValue.GetHashCode(); // TODO : change to get nodesource and node target value

            if (ontoEdges.TryGetValue(idOntoEdge, out OntoEdge ontoEdgeB))
            {
                return ontoEdgeB;
            }
            else
            {
                var ontoEdge = new OntoEdge(idOntoEdge, ontoEdgeValue);
                ontoEdges.Add(idOntoEdge, ontoEdge);
                return ontoEdge;
            }
        }


        OntoNode GetOntoNode(string ontoNodeValue)
        {
            var idOntoNode = ontoNodeValue.GetHashCode();

            if (ontoNodes.TryGetValue(idOntoNode, out OntoNode ontoNodeB))
            {
                return ontoNodeB;
            }
            else
            {
                var ontoNode = new OntoNode(idOntoNode, ontoNodeValue);
                ontoNodes.Add(idOntoNode, ontoNode);
                return ontoNode;
            }
        }
    }

    private void SetRootAndDepth()
    {
        if (_ontoNodes.Count == 0)
            return;

        _rootOntoNode = _ontoNodes.First().Value;



        while(_rootOntoNode.OntoNodeSource.Count > 0) 
        {
            _rootOntoNode = _rootOntoNode.OntoNodeSource[0];
        }

        SetDepth(_rootOntoNode, 0);
    }

    private void SetDepth(OntoNode ontoNode, int depth)
    {
        if (ontoNode.Depth < depth)
            return;

        ontoNode.Depth = depth;
        depth++;


        var nodeTarget = ontoNode.OntoNodeTarget;


        foreach (var ontoNodeChild in nodeTarget)
        {
            SetDepth(ontoNodeChild, depth);
        }
    }

    public OntoNode GetOntoNode(int id)
    {
        if (_ontoNodes.TryGetValue(id, out var ontoNode))
        {
            Debug.LogWarning("OntologyTree : Couldn't get onto node with id");
            return null;
        }

        return ontoNode;
    }

    public void ResetDefinedNodes()
    {
        foreach(var idAndOntoNode in _ontoNodes)
        {
            var ontoNode = idAndOntoNode.Value;
            ontoNode.NodesDefined = new();
        }
    }

    private static string CleanUri(string uri)
    {
        return uri.Replace("http://", "").Replace("/", "").Replace(".", "").Replace("\\", "").Replace("#", "");
    }
}
