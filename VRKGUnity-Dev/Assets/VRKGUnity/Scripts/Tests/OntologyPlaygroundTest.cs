using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;

public class OntologyPlaygroundTest : MonoBehaviour
{

    public string OntologyToLoadFileName = "";
    public string OntologyToLoadUri = "";

    [ContextMenu("Load Ontology")]
    private async void LoadOntology()
    {

        var xmlContent = await FileHelper.LoadAsync(Application.dataPath, "VRKGUnity", "Data", "cap44_1455283593", OntologyToLoadFileName);

        IGraph graph = new VDS.RDF.Graph();

        if (!graph.TryLoadFromRdf(xmlContent))
            return;

        graph.CleanFromLabelAndComment();

        TestOntology(graph);

    }


    private void TestOntology(IGraph graph)
    {
        Dictionary<int, OntoNode> ontoNodes = new();
        Dictionary<int, OntoEdge> ontoEdges = new();

        List<Triple> noUsableTriple = new();

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

            //For properties
            bool isDomain = (pValue == "http://www.w3.org/2000/01/rdf-schema#domain");

            if (isDomain || pValue == "http://www.w3.org/2000/01/rdf-schema#range")
            {
                AddEdge(sValue, oValue, isDomain);
                continue;
            }

            noUsableTriple.Add(triple);

        }

        Debug.Log(noUsableTriple);


        if (ontoNodes.Count == 0)
            Debug.Log("cc");

        //return new OntologyTree(ontoNodes, ontoEdges, uri);


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

}
