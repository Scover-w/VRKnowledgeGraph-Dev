using QuikGraph;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Edge : IEdge<Node>
{
    public EdgeDirection EdgeDirection { get { return _edgeDirection; } }

    public bool IsPropagated
    {
        get
        {
            return _isPropagated;
        }
    }

    public string NameWithPrefix
    {
        get
        {
            StringBuilder sb = new();

            foreach(EdgeProperty edgeProperty in EdgeProperties)
            {
                if (edgeProperty.Type == NodgeType.Literal)
                    sb.AppendLine(edgeProperty.Value);
                else
                    sb.AppendLine(edgeProperty.Prefix + ":" + edgeProperty.Value);
            }

            return sb.ToString();
        }
    }

    public string UID { get; }

    public List<EdgeProperty> EdgeProperties { get; }


    public Node Source 
    { 
        get
        {
            return IsHiddenFromFilter? null : _source;
        }
    }
    public Node Target 
    { 
        get
        {
            return IsHiddenFromFilter ? null : _target;
        }
    }

    public Transform MainGraphEdgeTf;
    public Transform SubGraphEdgeTf;

    public LineRenderer MainGraphLine;
    public LineRenderer SubGraphLine;

    public EdgeStyler MainStyler;
    public EdgeStyler SubStyler;

    public bool IsHiddenFromFilter { get; private set; } = false;

    private Node _source;
    private Node _target;

    private EdgeDirection _edgeDirection;
    private bool _doDisplayMainEdge;
    private bool _doDisplaySubEdge;

    bool _isPropagated = false;

    public Edge(string type, string value, Node source, Node target, GraphDbRepositoryNamespaces repoNamespaces) 
    {
        UID = GetUID(source.Uri, target.Uri);

        _source = source;
        _target = target;

        EdgeProperties = new();

        _doDisplayMainEdge = false;
        _doDisplaySubEdge = false;


        NodgeType typeP = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;

        if (typeP == NodgeType.Literal)
        {
            EdgeProperties.Add(new EdgeProperty(value));
            return;
        }

        var (namespce, localName) = value.ExtractUri();
        EdgeProperties.Add(new EdgeProperty(repoNamespaces.GetPrefix(namespce), namespce, localName));

        _edgeDirection = EdgeDirection.Forward;
    }

    public void AddProperty(string type, string value, Node source, GraphDbRepositoryNamespaces repoNamespaces)
    {
        NodgeType typeP = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;
        bool isDirectionInverted = (Source != source);

        if (typeP == NodgeType.Literal)
        {
            EdgeProperties.Add(new EdgeProperty(value, isDirectionInverted));
            return;
        }

        var (namespce, localName) = value.ExtractUri();
        EdgeProperties.Add(new EdgeProperty(repoNamespaces.GetPrefix(namespce), namespce, localName, isDirectionInverted));

        if (isDirectionInverted)
            _edgeDirection = EdgeDirection.Both;
    }

    public void DisplayMainEdge(bool doDisplayMainEdge)
    {
        if (IsHiddenFromFilter)
            return;

        _doDisplayMainEdge = doDisplayMainEdge;
        MainGraphLine.enabled = doDisplayMainEdge;
    }

    public void DisplaySubEdge(bool doDisplaySubEdge)
    {
        if (IsHiddenFromFilter)
            return;

        _doDisplaySubEdge = doDisplaySubEdge;
        SubGraphLine.enabled = _doDisplaySubEdge;
    }

    public void HideEdge()
    {
        ResetInteractionValues();

        DisplayMainEdge(false);
        DisplaySubEdge(false);

        IsHiddenFromFilter = true;
    }

    public void UnhideEdge(GraphMode graphMode)
    {
        IsHiddenFromFilter = false;

        var graphConfig = GraphConfiguration.Instance;

        if (!graphConfig.DisplayEdges)
            return;

        DisplayMainEdge(true);

        if (!(graphMode == GraphMode.Immersion && graphConfig.DisplayGPS))
            return;

        DisplaySubEdge(true);

    }

    private void ResetInteractionValues()
    {
        _isPropagated = false;

        if (MainStyler != null)
            MainStyler.SetPropagation();

        if (SubStyler != null)
            SubStyler.SetPropagation();
    }

    public void SetPropagation(GraphMode graphMode, bool isPropagated)
    {
        _isPropagated = isPropagated;
        if (MainStyler != null)
            MainStyler.SetPropagation();

        if (SubStyler == null)
            return;

        SubStyler.SetPropagation();

        if (graphMode != GraphMode.Desk)
            return;

        DisplaySubEdge(isPropagated);
    }


    public void SetSourcePositionFromMovingNode(Vector3 worldPosition, bool isGraphType)
    {
        if(isGraphType)
            MainStyler.SetSourcePositionFromMovingNode(worldPosition);
        else
            SubStyler.SetSourcePositionFromMovingNode(worldPosition);
    }

    public void SetTargetPositionFromMovingNode(Vector3 worldPosition, bool isGraphType)
    {
        if (isGraphType)
            MainStyler.SetTargetPositionFromMovingNode(worldPosition);
        else
            SubStyler.SetTargetPositionFromMovingNode(worldPosition);
    }


    public void CleanFromNodes()
    {
        Source?.CleanEdge(this, true);

        Target?.CleanEdge(this, false);
    }

    public EdgeSimuData ToSimuData()
    {
        return new EdgeSimuData(Source.UID, Target.UID);
    }


    public static string GetUID(string valueA,string valueB)
    {
        var (valueC, valueD) = Sort(valueA, valueB);

        return valueC + valueD;
    }

    private static (string, string) Sort(string valueA, string valueB)
    {
        if (string.Compare(valueA, valueB) > 0)
        {
            return (valueB, valueA);
        }
        else
        {
            return (valueA, valueB);
        }
    }
}


public class EdgeProperty
{
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
    public bool IsDirectionInverted { get; }

    public EdgeProperty(string value, bool isDirectionInverted = false)
    {
        Type = NodgeType.Literal;
        Value = value;
        IsDirectionInverted = isDirectionInverted;
    }

    public EdgeProperty(string prefix, string namepsce, string value, bool isDirectionInverted = false)
    {
        Type = NodgeType.Uri;
        Prefix = prefix;
        Namespace = namepsce;
        Value = value;

        IsDirectionInverted = isDirectionInverted;
    }
}

public enum EdgeDirection
{
    Forward,
    Both
}