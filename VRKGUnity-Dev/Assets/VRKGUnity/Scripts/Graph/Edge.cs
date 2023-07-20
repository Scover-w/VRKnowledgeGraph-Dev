using QuikGraph;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public class Edge : IEdge<Node>
{
    public EdgeDirection EdgeDirection { get { return _edgeDirection; } }

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

    public readonly int Id;

    public readonly List<EdgeProperty> EdgeProperties;


    public Node Source { get; }
    public Node Target { get; }

    public Transform MainGraphEdgeTf;
    public Transform SubGraphEdgeTf;

    public LineRenderer MainGraphLine;
    public LineRenderer SubGraphLine;

    public EdgeStyler MainEdgeStyler;
    public EdgeStyler SubEdgeStyler;

    public bool IsHidden = true;

    private EdgeDirection _edgeDirection;
    private bool _doDisplayMainEdge;
    private bool _doDisplaySubEdge;

    public Edge(string type, string value, Node source, Node target, GraphDbRepositoryNamespaces repoNamespaces) 
    {
        Id = GetId(source.Uri, target.Uri);

        Source = source;
        Target = target;

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
        if (!IsHidden)
            return;

        _doDisplayMainEdge = doDisplayMainEdge;
        MainGraphLine.enabled = doDisplayMainEdge;
    }

    public void DisplaySubEdge(bool doDisplaySubEdge)
    {
        if (!IsHidden)
            return;

        _doDisplaySubEdge = doDisplaySubEdge;
        SubGraphLine.enabled = _doDisplaySubEdge;
    }

    public void HideEdge()
    {
        ResetInteractionValues();

        DisplayMainEdge(false);
        DisplaySubEdge(false);

        IsHidden = false;
    }

    public void UnhideEdge(GraphMode graphMode)
    {
        IsHidden = true;

        var graphConfig = GraphConfiguration.Instance;

        if (!graphConfig.DisplayEdges)
            return;

        DisplayMainEdge(true);

        if (!(graphMode == GraphMode.Immersion && graphConfig.ShowWatch))
            return;

        DisplaySubEdge(true);

    }

    private void ResetInteractionValues()
    {
        if (MainEdgeStyler != null)
            MainEdgeStyler.SetPropagation(false);

        if (SubEdgeStyler != null)
            SubEdgeStyler.SetPropagation(false);
    }

    public void SetPropagation(GraphMode graphMode, bool isInPropagation)
    {
        if (MainEdgeStyler != null)
            MainEdgeStyler.SetPropagation(isInPropagation);

        if (SubEdgeStyler == null)
            return;

        SubEdgeStyler.SetPropagation(isInPropagation);

        if (graphMode != GraphMode.Desk)
            return;

        DisplaySubEdge(isInPropagation);
    }



    public void CleanFromNodes()
    {
        Source?.CleanEdge(this, true);

        Target?.CleanEdge(this, false);
    }

    public EdgeSimuData ToSimuData()
    {
        return new EdgeSimuData(Source.Id, Target.Id);
    }


    public static int GetId(string valueA,string valueB)
    {
        var (valueC, valueD) = Sort(valueA, valueB);

        return (valueC + valueD).GetHashCode();
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
    public readonly bool IsDirectionInverted;

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