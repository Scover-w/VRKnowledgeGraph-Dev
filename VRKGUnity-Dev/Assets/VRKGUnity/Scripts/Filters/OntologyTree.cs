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
    Dictionary<int, OntoNode> _ontoNodes;
    Dictionary<int, OntoEdge> _ontoEdges;


    private OntologyTree(Dictionary<int, OntoNode> ontoNodes, Dictionary<int, OntoEdge> ontoEdges)
    {
        _ontoNodes = ontoNodes;
        _ontoEdges = ontoEdges;
    }

    public static async Task<OntologyTree> CreateAsync(IGraph graph)
    {
        return await Task.Run(() =>
        {
            OntologyTree ontologyTree = CreateOntologyTree(graph);
            return ontologyTree;
        });
    }

    private static OntologyTree CreateOntologyTree(IGraph graph)
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

        return new OntologyTree(ontoNodes, ontoEdges);


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


    public OntoNode GetSource()
    {
        var ontoNode = _ontoNodes.First().Value;



        while(ontoNode.NodeSource.Count > 0) 
        { 
            ontoNode = ontoNode.NodeSource[0];
        }

        return ontoNode;
    }

    public void SaveToFile()
    {
        List<string> names = new();

        foreach(var s in _ontoNodes)
        {
            names.Add(s.Value.Value.Replace("http://www.cidoc-crm.org/cidoc-crm/", "").Replace("_"," "));
        }

        names.Sort();

        string filePath = Path.Combine(Application.dataPath, "VRKGUnity", "Data", "Tests", "cidocNames.txt");

        // Create a new file or overwrite existing file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string str in names)
            {
                // Write each string on a new line
                writer.WriteLine(str);
            }
        }

        Debug.Log("Done");
    }
}



public class OntoNode
{
    public int Id;
    public string Value;

    public List<OntoNode> NodeSource;
    public List<OntoNode> NodeTarget;

    public List<OntoEdge> EdgeSource;
    public List<OntoEdge> EdgeTarget;

    public OntoNode(int id, string value)
    {
        Id = id;
        Value = value;  

        NodeSource = new();
        NodeTarget = new();

        EdgeSource = new();
        EdgeTarget = new();
    }

}

public class OntoEdge
{
    public int Id;
    public string Value;

    public List<OntoNode> NodeSource;
    public List<OntoNode> NodeTarget;


    public OntoEdge(int id, string value) 
    { 
        Id = id;
        Value = value;

        NodeSource = new();
        NodeTarget = new();
    }

}