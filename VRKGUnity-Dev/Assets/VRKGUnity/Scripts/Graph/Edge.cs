using QuikGraph;
using UnityEngine;
using UnityEngine.UIElements;

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

    public Transform MegaTf;
    public Transform MiniTf;

    public LineRenderer MegaLine;
    public LineRenderer MiniLine;

    public EdgeStyler MegaStyler;
    public EdgeStyler MiniStyler;

    private bool _activeSelf;

    public Edge(string type, string value, Node source, Node target) 
    {
        Type = (type == "uri") ? NodgeType.Uri : NodgeType.Literal;
        Value = value;
        Source = source;
        Target = target;    

        Id = (source.Type + source.Value + target.Type + target.Value).GetHashCode();

        _activeSelf = false;
    }

    public void SetActive(bool value)
    {
        _activeSelf = value;
        MegaLine.enabled = value;
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