using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Query.Expressions.Functions.XPath.String;

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

        return new OntologyTree(ontoNodes, ontoEdges, uri);


        void AddSubclassOf(string sValue, string oValue)
        {

            OntoNode ontoNodeSubject = GetOntoNode(sValue);
            OntoNode ontoNodeObject = GetOntoNode(oValue);

            ontoNodeObject.NodeTarget.Add(ontoNodeSubject);
            ontoNodeSubject.NodeSource.Add(ontoNodeObject);
        }

        void AddEdge(string propertyUri, string nodeValue, bool isDomain)
        {
            OntoNode ontoNode = GetOntoNode(nodeValue);
            OntoEdge ontoEdge = GetOntoEdge(propertyUri);

            if (isDomain)
            {
                ontoNode.EdgeSource.Add(ontoEdge);
                ontoEdge.NodeSource.Add(ontoNode);
            }
            else
            {
                ontoNode.EdgeTarget.Add(ontoEdge);
                ontoEdge.NodeTarget.Add(ontoNode);
            }
        }

        OntoEdge GetOntoEdge(string ontoEdgeValue)
        {
            var idOntoEdge = ontoEdgeValue.GetHashCode();

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
        _rootOntoNode = _ontoNodes.First().Value;



        while(_rootOntoNode.NodeSource.Count > 0) 
        {
            _rootOntoNode = _rootOntoNode.NodeSource[0];
        }

        SetDepth(_rootOntoNode, 0);
    }

    private void SetDepth(OntoNode ontoNode, int depth)
    {
        if (ontoNode.Depth < depth)
            return;

        ontoNode.Depth = depth;
        depth++;


        var nodeTarget = ontoNode.NodeTarget;


        foreach (var ontoNodeChild in nodeTarget)
        {
            SetDepth(ontoNodeChild, depth);
        }
    }
}
