using System.Collections.Generic;
using UnityEngine;

public class Node
{

    private static string[] _nameUris = new string[] {  "http://www.w3.org/2004/02/skos/core#prefLabel", 
                                                        "http://www.w3.org/2000/01/rdf-schema#label",
                                                        "http://www.w3.org/2004/02/skos/core#altLabel", 
                                                        "http://purl.org/dc/terms/title"};

    public bool IsSelected
    {
        get
        { 
            return _isSelected; 
        }
    }

    public int Id;
    public NodgeType Type;

    /// <summary>
    /// Is a uri (namespace + localName) or a literal.
    /// </summary>
    public string Value;

    public NodeStyler MainGraphStyler;
    public NodeStyler SubGraphStyler;

    public List<Edge> EdgeSource;
    public List<Edge> EdgeTarget;

    public Transform MainGraphNodeTf;
    public Transform SubGraphNodeTf;

    public Vector3 AbsolutePosition;
    public Vector3 AbsoluteVelocity;
    
    public Dictionary<string, string> Properties;
    private bool _doDisplayMainNode;
    private bool _doDisplaySubNode;

    public OntoNodeGroup OntoNodeGroup;

    public float AverageShortestPathLength;
    public float BetweennessCentrality;
    public float ClosenessCentrality;
    public float ClusteringCoefficient;
    public float Degree;

    bool _isHovered = false;
    bool _isSelected = false;
    bool _isInPropagation = false;

    static System.Random _random;

    public Node(int id, string type, string value)
    {
        Id = id;
        Type = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;
        Value = value;

        EdgeSource = new();
        EdgeTarget = new();

        Properties = new();

        _doDisplayMainNode = false;
        _doDisplaySubNode = false;
    }

    public Node(string type, string value)
    {
        Type = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;
        Value = value;
        Id = Value.GetHashCode();

        _doDisplayMainNode = false;
    }


    

    public string GetName()
    {
        int nbProperties = Properties.Count;


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

    public void NodeNamesToProperties()
    {
        if (EdgeTarget.Count == 0)
            return;

        foreach(var edge in EdgeTarget)
        {
            if (edge.Type == NodgeType.Literal)
                continue;

            string edgeValue = edge.Value;

            if (!ContainNameUri(edgeValue))
                continue;

            if (Properties.ContainsKey(edgeValue))
                continue;

            Properties.Add(edgeValue, edge.Target.Value);
        }
    }

    public NodeSimuData ToSimuData()
    {
        return new NodeSimuData(Id, AbsolutePosition, AbsoluteVelocity);
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
        _doDisplayMainNode = doDisplayMainNode;
        MainGraphNodeTf.gameObject.SetActive(_doDisplayMainNode);
    }

    public void DisplaySubNode(bool doDisplaySubNode)
    {
        _doDisplaySubNode = doDisplaySubNode;
        SubGraphNodeTf.gameObject.SetActive(_doDisplaySubNode);
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
    public void ResetInteractionState()
    {
        _isHovered = false;
        _isSelected = false;
        _isInPropagation = false;
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
        if (MainGraphStyler != null)
            MainGraphStyler.UpdateMaterial(_isHovered, _isSelected, _isInPropagation);

        if (SubGraphStyler != null)
            SubGraphStyler.UpdateMaterial(_isHovered, _isSelected, _isInPropagation);
    }

    private void TryForceUnselect()
    {
        if (MainGraphStyler != null)
            MainGraphStyler.TryForceUnselect();

        if (SubGraphStyler != null)
            SubGraphStyler.TryForceUnselect();
    }

    #endregion
}



public enum NodgeType
{
    Uri,
    Literal
}