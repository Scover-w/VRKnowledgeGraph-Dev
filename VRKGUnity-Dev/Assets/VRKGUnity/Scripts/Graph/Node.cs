﻿using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Node
{

    private static readonly string[] _nameUris = new string[] {  "http://www.w3.org/2004/02/skos/core#prefLabel", 
                                                        "http://www.w3.org/2000/01/rdf-schema#label",
                                                        "http://www.w3.org/2004/02/skos/core#altLabel", 
                                                        "http://purl.org/dc/terms/title"};

    public HashSet<Edge> Edges 
    { 
        get 
        {

            HashSet<Edge> edges = new();

            foreach(Edge edge in EdgeSource)
                edges.Add(edge);

            foreach (Edge edge in EdgeTarget)
                edges.Add(edge);

            return edges;
        } 
    }

    public string PrefixValue
    {
        get
        {
            if (Type == NodgeType.Literal)
                return Value;

            return Prefix + ":" + Value;
        }
    }

    public bool IsSelected
    {
        get
        { 
            return _isSelected; 
        }
    }

    public bool IsPropagated
    {
        get
        {
            return _isSelected || _isPropagated;
        }
    }

    public string Uri
    {
        get
        {
            if (Type == NodgeType.Literal)
                return Value;

            return Namespace + Value;
        }
    }

    public bool IsIsolated
    {
        get
        {
            return (EdgeSource.Count + EdgeTarget.Count) == 0;
        }
    }

    public string UID => Uri;
    public NodgeType Type { get; }


    /// <summary>
    /// Null if Type is a literal
    /// </summary>
    public string Prefix { get; }
    /// <summary>
    /// Null if Type is a literal
    /// </summary>
    public string Namespace { get; }

    /// <summary>
    /// Is a localName (if Type is a Uri) or a literal.
    /// </summary>
    public string Value { get; }

    public NodeStyler MainStyler;
    public NodeStyler SubStyler;

    public List<Edge> EdgeSource = new();
    public List<Edge> EdgeTarget = new();

    public Transform MainGraphNodeTf;
    public Transform SubGraphNodeTf;

    public Vector3 AbsolutePosition;
    
    public Dictionary<string, string> Properties = new();
    public HashSet<string> Medias = new();

    private bool _doDisplayMainNode;
    private bool _doDisplaySubNode;

    public OntoNodeGroup OntoNodeGroup;
    public OntoNode OntoNode;

    public float AverageShortestPathLength;
    public float BetweennessCentrality;
    public float ClosenessCentrality;
    public float ClusteringCoefficient;
    public float Degree;

    public bool IsHiddenFromFilter { get; private set; } = false;

    bool _isHovered = false;
    bool _isSelected = false;
    bool _isPropagated = false;

    static System.Random _random;

    public Node(string type, string value, GraphDbRepositoryNamespaces repoNamespaces)
    {
        Type = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;


        if(Type == NodgeType.Literal)
        {
            Value = value;
        }
        else
        {
            var (namespce, localName) = value.ExtractUri();

            Value = localName;
            Namespace = namespce;

            Prefix = repoNamespaces.GetPrefix(Namespace);
        }

        _doDisplayMainNode = false;
        _doDisplaySubNode = false;

        IsHiddenFromFilter = false;
    }
    

    public string GetShorterName()
    {
        string shorterName = null;

        foreach(var propNameAndValue in Properties)
        {
            var propName = propNameAndValue.Key.ToLower();

            if (!(propName.Contains("label") || propName.Contains("title") || propName.Contains("name")))
                continue;

            string name = propNameAndValue.Value;

            if(shorterName == null)
            {
                shorterName = name;
                continue;
            }

            if (name.Length < shorterName.Length)
                shorterName = name;

        }

        return shorterName;
    }

    public string GetPrefName()
    {
        string prefName = "";

        foreach (var propNameAndValue in Properties)
        {
            var propName = propNameAndValue.Key.ToLower();

            if(propName.Contains("prefLabel"))
                return propNameAndValue.Value;

            if (!(propName.Contains("label") || propName.Contains("title") || propName.Contains("name")))
                continue;


            string name = propNameAndValue.Value;

            if (name.Length > prefName.Length)
                prefName = name;

        }

        return (prefName.Length == 0) ? null : prefName;
    }

    public void ResetAbsolutePosition(int seed)
    {
        _random = new System.Random(seed + UID.GetHashCode());
        AbsolutePosition = new Vector3((float)_random.NextDouble() * 0.2f - 0.1f,
                       (float)_random.NextDouble() * 0.2f - 0.1f,
                       (float)_random.NextDouble() * 0.2f - 0.1f);
    }

    public bool DoesPropertiesContainName()
    {
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
        return new NodeSimuData(UID, AbsolutePosition);
    }

    public NodeSimuData2D ToSimuData2D()
    {
        return new NodeSimuData2D(UID, AbsolutePosition);
    }

    private bool ContainNameUri(string value)
    {
        foreach(var uri in _nameUris) 
        { 
            if(value == uri) 
                return true;
        
        }

        return false;
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

    public void DisplayMainNode(bool doDisplayMainNode)
    {
        if (IsHiddenFromFilter)
            return;

        _doDisplayMainNode = doDisplayMainNode;
        MainGraphNodeTf.gameObject.SetActive(_doDisplayMainNode);
    }

    public void DisplaySubNode(bool doDisplaySubNode)
    {
        if (IsHiddenFromFilter)
            return;


        _doDisplaySubNode = doDisplaySubNode;
        SubGraphNodeTf.gameObject.SetActive(_doDisplaySubNode);
    }

    public void HideFromFilter()
    {
        ResetInteractionState();

        DisplayMainNode(false);
        DisplaySubNode(false);

        IsHiddenFromFilter = true;

        OntoNode?.NodesAttached.Remove(this);
    }

    public void UnhideFromFilter(GraphMode graphMode)
    {
        IsHiddenFromFilter = false;

        DisplayMainNode(true);

        OntoNode?.NodesAttached.Add(this);

        var graphConfig = GraphConfiguration.Instance;

        if (!(graphMode == GraphMode.Immersion && graphConfig.DisplayGPS))
            return;

        DisplaySubNode(true);
    }

    public List<Node> GetNeighbors()
    {
        var neighbors = new List<Node>();

        foreach(var edge in EdgeSource)
        {
            Node nodeTarget = edge.Target;

            if (nodeTarget == null)
                continue;

            if (!neighbors.Contains(nodeTarget))
                neighbors.Add(nodeTarget);
        }

        foreach (var edge in EdgeTarget)
        {
            Node nodeSource = edge.Source;

            if (nodeSource == null)
                continue;

            if (!neighbors.Contains(nodeSource))
                neighbors.Add(nodeSource);
        }

        return neighbors;
    }




    #region Interaction
    private void ResetInteractionState()
    {
        _isHovered = false;
        _isSelected = false;
        _isPropagated = false;
        UpdateMaterials();
    }

    public void SetPropagation(GraphMode graphMode, bool isInPropagation)
    {
        _isPropagated = isInPropagation;

        UpdateMaterials();

        if (graphMode != GraphMode.Desk)
            return;

        DisplaySubNode(isInPropagation);
    }

    public void UnSelect()
    {
        if (!_isSelected)
            return;

        _isSelected = false;

        TryForceUnselect();
        UpdateMaterials();
    }

    public void OnHover()
    {
        _isHovered = !_isHovered;
        UpdateMaterials();

        var nodgeSelection = NodgeSelectionManager.Instance;

        if (_isHovered)
            nodgeSelection.Hover(this);
        else
            nodgeSelection.UnHover(this);
    }


    public void OnSelect()
    {
        _isSelected = !_isSelected;
        UpdateMaterials();

        var nodgeSelection = NodgeSelectionManager.Instance;

        if(_isSelected)
            nodgeSelection.Select(this);
        else
            nodgeSelection.UnSelect(this);
    }

    private void UpdateMaterials()
    {
        if (MainStyler != null)
            MainStyler.UpdateMaterial(_isHovered, _isSelected, _isPropagated);

        if (SubStyler != null)
            SubStyler.UpdateMaterial(_isHovered, _isSelected, _isPropagated);
    }

    private void TryForceUnselect()
    {
        if (MainStyler != null)
            MainStyler.TryForceUnselect();

        if (SubStyler != null)
            SubStyler.TryForceUnselect();
    }

    public void MoveEdgeWithNode(Vector3 worldPosition, bool isGraphType)
    {
        foreach(Edge edge in EdgeSource)
        {
            edge.SetSourcePositionFromMovingNode(worldPosition, isGraphType);
        }

        foreach (Edge edge in EdgeTarget)
        {
            edge.SetTargetPositionFromMovingNode(worldPosition, isGraphType);
        }
    }

    #endregion
}



public enum NodgeType
{
    Uri,
    Literal
}