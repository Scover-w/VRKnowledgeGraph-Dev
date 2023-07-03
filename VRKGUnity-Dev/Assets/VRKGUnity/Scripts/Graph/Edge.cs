using QuikGraph;
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
        _doDisplayMainEdge = doDisplayMainEdge;
        MainGraphLine.enabled = doDisplayMainEdge;
    }

    public void DisplaySubEdge(bool doDisplaySubEdge)
    {
        _doDisplaySubEdge = doDisplaySubEdge;
        SubGraphLine.enabled = _doDisplaySubEdge;
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