using System.Collections.Generic;
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

    public string Uri
    {
        get
        {
            if (Type == NodgeType.Literal)
                return Value;

            return Namespace + Value;
        }
    }

    public readonly int Id;
    public readonly NodgeType Type;


    /// <summary>
    /// Null if Type is a literal
    /// </summary>
    public readonly string Prefix;
    /// <summary>
    /// Null if Type is a literal
    /// </summary>
    public readonly string Namespace;

    /// <summary>
    /// Is a localName (if Type is a Uri) or a literal.
    /// </summary>
    public readonly string Value;

    public NodeStyler MainNodeStyler;
    public NodeStyler SubNodeStyler;

    public List<Edge> EdgeSource;
    public List<Edge> EdgeTarget;

    public Transform MainGraphNodeTf;
    public Transform SubGraphNodeTf;

    public Vector3 AbsolutePosition;
    
    public Dictionary<string, string> Properties;
    public HashSet<string> Medias;

    private bool _doDisplayMainNode;
    private bool _doDisplaySubNode;

    public OntoNodeGroup OntoNodeGroup;

    public float AverageShortestPathLength;
    public float BetweennessCentrality;
    public float ClosenessCentrality;
    public float ClusteringCoefficient;
    public float Degree;

    public bool IsHidden = true;

    bool _isHovered = false;
    bool _isSelected = false;
    bool _isInPropagation = false;

    static System.Random _random;

    public Node(int id, string type, string value, GraphDbRepositoryNamespaces repoNamespaces)
    {
        Id = id;
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


        EdgeSource = new();
        EdgeTarget = new();

        Properties = new();
        Medias = new();

        _doDisplayMainNode = false;
        _doDisplaySubNode = false;

        IsHidden = true;
    }

    public Node(string type, string value, GraphDbRepositoryNamespaces repoNamespaces)
    {
        Id = value.GetHashCode();
        Type = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;


        if (Type == NodgeType.Literal)
        {
            Value = value;
        }
        else
        {
            var (namespce, localName) = Value.ExtractUri();

            Value = localName;
            Namespace = namespce;

            Prefix = repoNamespaces.GetPrefix(Namespace);
        }

        EdgeSource = new();
        EdgeTarget = new();

        Properties = new();
        Medias = new();

        _doDisplayMainNode = false;
        _doDisplaySubNode = false;

        IsHidden = true;
    }


    

    public string GetName()
    {
        foreach(var propNameAndValue in Properties)
        {
            var propName = propNameAndValue.Key;

            if (propName.Contains("label") || propName.Contains("title") || propName.Contains("name"))
                return propNameAndValue.Value;

        }

        return null;
    }

    public void ResetAbsolutePosition(int seed)
    {
        _random = new System.Random(seed + Id);
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

    public void NodeNamesToProperties()
    {
        if (EdgeTarget.Count == 0)
            return;

        foreach(var edge in EdgeTarget)
        {
            if (edge.Type == NodgeType.Literal)
                continue;

            string edgeValue = edge.Uri;

            if (!ContainNameUri(edgeValue))
                continue;

            if (Properties.ContainsKey(edgeValue))
                continue;

            Properties.Add(edgeValue, edge.Target.Uri);
        }
    }

    public NodeSimuData ToSimuData()
    {
        return new NodeSimuData(Id, AbsolutePosition);
    }

    public NodeSimuData2D ToSimuData2D()
    {
        return new NodeSimuData2D(Id, AbsolutePosition);
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
        if (!IsHidden)
            return;

        _doDisplayMainNode = doDisplayMainNode;
        MainGraphNodeTf.gameObject.SetActive(_doDisplayMainNode);
    }

    public void DisplaySubNode(bool doDisplaySubNode)
    {
        if (!IsHidden)
            return;


        _doDisplaySubNode = doDisplaySubNode;
        SubGraphNodeTf.gameObject.SetActive(_doDisplaySubNode);
    }

    public void HideNode()
    {
        ResetInteractionState();

        DisplayMainNode(false);
        DisplaySubNode(false);

        IsHidden = false;

        HideEdges();
    }

    public void Unhide(GraphMode graphMode)
    {
        IsHidden = true;

        DisplayMainNode(true);

        UnhideEdges(graphMode);

        var graphConfig = GraphConfiguration.Instance;

        if (!(graphMode == GraphMode.Immersion && graphConfig.ShowWatch))
            return;

        DisplaySubNode(true);
    }

    private void HideEdges()
    {
        foreach (Edge edge in EdgeSource)
        {
            edge.HideEdge();
        }

        foreach (Edge edge in EdgeTarget)
        {
            edge.HideEdge();
        }
    }

    private void UnhideEdges(GraphMode graphMode)
    {
        foreach (Edge edge in EdgeSource)
        {
            edge.UnhideEdge(graphMode);
        }

        foreach (Edge edge in EdgeTarget)
        {
            edge.UnhideEdge(graphMode);
        }
    }

    public List<Node> GetNeighbors()
    {
        var neighbors = new List<Node>();

        foreach(var edge in EdgeSource)
        {
            if(!neighbors.Contains(edge.Target))
                neighbors.Add(edge.Target);
        }

        foreach (var edge in EdgeTarget)
        {
            if(!neighbors.Contains(edge.Source))
                neighbors.Add(edge.Source);
        }

        return neighbors;
    }




    #region Interaction
    private void ResetInteractionState()
    {
        _isHovered = false;
        _isSelected = false;
        _isInPropagation = false;
        UpdateMaterials();
    }

    public void SetPropagation(GraphMode graphMode, bool isInPropagation)
    {
        _isInPropagation = isInPropagation;

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
        if (MainNodeStyler != null)
            MainNodeStyler.UpdateMaterial(_isHovered, _isSelected, _isInPropagation);

        if (SubNodeStyler != null)
            SubNodeStyler.UpdateMaterial(_isHovered, _isSelected, _isInPropagation);
    }

    private void TryForceUnselect()
    {
        if (MainNodeStyler != null)
            MainNodeStyler.TryForceUnselect();

        if (SubNodeStyler != null)
            SubNodeStyler.TryForceUnselect();
    }

    #endregion
}



public enum NodgeType
{
    Uri,
    Literal
}