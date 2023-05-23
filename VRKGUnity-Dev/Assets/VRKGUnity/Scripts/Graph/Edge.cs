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
    public string Type;
    public string Value;
    public Node Source { get; }
    public Node Target { get; }

    public LineRenderer Line;

    private bool _activeSelf;

    public Edge(string type, string value, Node source, Node target) 
    {
        Type = type;
        Value = value;
        Source = source;
        Target = target;    

        Id = (source.Type + source.Value + target.Type + target.Value).GetHashCode();

        _activeSelf = false;
    }

    public void SetActive(bool value)
    {
        _activeSelf = value;
        Line.enabled = value;
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