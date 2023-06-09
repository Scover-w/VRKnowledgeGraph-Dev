using Newtonsoft.Json;
using System.Collections.Generic;

public class OntoNode
{
    public int Id;
    public string Value;

    public int Depth = int.MaxValue;

    public List<OntoNode> OntoNodeSource;
    public List<OntoNode> OntoNodeTarget;

    public List<OntoEdge> OntoEdgeSource;
    public List<OntoEdge> OntoEdgeTarget;

    public List<Node> NodesDefined;

    public OntoNode(int id, string value)
    {
        Id = id;
        Value = value;  

        OntoNodeSource = new();
        OntoNodeTarget = new();

        OntoEdgeSource = new();
        OntoEdgeTarget = new();

        NodesDefined = new();
    }
}
