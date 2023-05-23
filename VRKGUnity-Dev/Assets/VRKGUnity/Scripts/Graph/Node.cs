using System;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;

public class Node
{

    private static string[] _nameUriOrder = new string[] { "http://www.w3.org/2004/02/skos/core#prefLabel", "http://www.w3.org/2004/02/skos/core#altLabel", "http://www.w3.org/2000/01/rdf-schema#label" };

    public bool ActiveSelf
    {
        get
        {
            return _activeSelf;
        }
    }

    public int Id;
    public string Type;
    public string Value;

    public List<Edge> EdgeSource;
    public List<Edge> EdgeTarget;

    public Transform Tf;
    public Vector3 Position;
    public Vector3 Velocity;
    
    public Dictionary<string, string> Properties;
    private bool _activeSelf;

    public float AverageShortestPathLength;
    public float BetweennessCentrality;
    public int Degree;

    static System.Random _random;

    public Node(int id, string type, string value)
    {
        Id = id;
        Type = type;
        Value = value;

        EdgeSource = new();
        EdgeTarget = new();

        Properties = new();

        _activeSelf = false;
    }

    public Node(string type, string value)
    {
        Type = type;
        Value = value;
        Id = (Type + Value).GetHashCode();

        _activeSelf = false;
    }

    public string GetName()
    {
        int nbProperties = Properties.Count;


        foreach(var propNameAndValue in Properties)
        {
            var propName = propNameAndValue.Key;
            if (!(propName.Contains("label") || propName.Contains("title") || propName.Contains("name")))
                return propNameAndValue.Value;

        }

        return null;
    }

    public void ResetPosition(int seed)
    {
        _random = new System.Random(seed + Id);
        Position = new Vector3((float)_random.NextDouble() * 0.2f - 0.1f,
                       (float)_random.NextDouble() * 0.2f - 0.1f,
                       (float)_random.NextDouble() * 0.2f - 0.1f);
    }

    public bool DoesPropertiesContainName()
    {
        int nbProperties = Properties.Count;

        foreach(var prop in Properties) 
        {
            string propType = prop.Key.ToLower();

            if (propType.Contains("label") || propType.Contains("title") || propType.Contains("name"))
                return true;
        }

        return false;
    }

    public void CleanEdge(Edge edge, bool isSource)
    {
        if(isSource)
            EdgeSource.Remove(edge);
        else
            EdgeTarget.Remove(edge);
    }

    public NodeSimuData ToSimuData()
    {
        return new NodeSimuData(Id, Position, Velocity);
    }

    //private void ActivateNodes(int depth, Graph graph)
    //{
    //    graph.Add(this);
    //    SetActive(true);

    //    depth--;

    //    // if comes from source, next is targetNode, inverse
    //    for (int i = 0; i < 2; i++)
    //    {
    //        var edges = (i == 0) ? EdgeSource : EdgeTarget;
    //        int nbEdge = edges.Count;


    //        for (int j = 0; j < nbEdge; j++)
    //        {
    //            var edge = edges[j];

    //            if (graph.Contains(edge))
    //                continue;

    //            if (depth == 0)
    //                continue;

    //            graph.Add(edge);
    //            edge.SetActive(true);

    //            var nextNode = (i == 0) ? edge.Target : edge.Source;

    //            if (graph.Contains(nextNode))
    //                continue;

    //            nextNode.ActivateNodes(depth, graph);
    //        }
    //    }
    //}

    public void SetActive(bool value)
    {
        _activeSelf = value;
        Tf.gameObject.SetActive(value);
    }
}
