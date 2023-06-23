using QuikGraph;
using UnityEngine;

public class Edge : IEdge<Node>
{
    public bool AciveSelf
    {
        get
        {
            return _activeSelf;
        }
    }

    public int Id;
    public NodgeType Type;
    public string Value;
    public Node Source { get; }
    public Node Target { get; }

    public Transform MainGraphEdgeTf;
    public Transform SubGraphEdgeTf;

    public LineRenderer MainGraphLine;
    public LineRenderer SubGraphLine;

    public EdgeStyler MainGraphStyler;
    public EdgeStyler SubGraphStyler;

    private bool _activeSelf;

    public Edge(string type, string value, Node source, Node target) 
    {
        Type = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;
        Value = value;
        Source = source;
        Target = target;    

        Id = (source.Value + target.Value).GetHashCode();

        _activeSelf = false;
    }

    public void SetActive(bool value)
    {
        _activeSelf = value;
        MainGraphLine.enabled = value;
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