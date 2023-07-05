﻿using QuikGraph;
using UnityEngine;

public class Edge : IEdge<Node>
{
    public int Id;
    public NodgeType Type;
    public string Value;
    public Node Source { get; }
    public Node Target { get; }

    public Transform MainGraphEdgeTf;
    public Transform SubGraphEdgeTf;

    public LineRenderer MainGraphLine;
    public LineRenderer SubGraphLine;

    public EdgeStyler MainEdgeStyler;
    public EdgeStyler SubEdgeStyler;

    public bool IsHidden = true;

    private bool _doDisplayMainEdge;
    private bool _doDisplaySubEdge;

    public Edge(string type, string value, Node source, Node target) 
    {
        Type = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;
        Value = value;
        Source = source;
        Target = target;    

        Id = (source.Value + target.Value).GetHashCode();

        _doDisplayMainEdge = false;
        _doDisplaySubEdge = false;
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
        if(Source != null)
        {
            Source.CleanEdge(this, true);
        }

        if(Target != null) 
        { 
            Target.CleanEdge(this, false);
        }
    }

    public EdgeSimuData ToSimuData()
    {
        return new EdgeSimuData(Source.Id, Target.Id);
    }
}