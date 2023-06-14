using Newtonsoft.Json;
using System.Collections.Generic;

public class OntoNode
{
    public int Id;
    public string Value;

    public int Depth = int.MaxValue;

    public List<OntoNode> OntoNodeSource;
    public List<OntoNode> OntoNodeTarget;

    public List<Node> NodesDefined;

    public OntoNode(int id, string value)
    {
        Id = id;
        Value = value;  

        OntoNodeSource = new();
        OntoNodeTarget = new();

        NodesDefined = new();
    }

    public OntoNode(string value)
    {
        Id = value.GetHashCode();
        Value = value;

        OntoNodeSource = new();
        OntoNodeTarget = new();

        NodesDefined = new();
    }

    public void AddSource(OntoNode ontoNode)
    {
        if (OntoNodeSource.Contains(ontoNode))
            return;

        OntoNodeSource.Add(ontoNode);
    }

    public void AddTarget(OntoNode ontoNode)
    {
        if (OntoNodeTarget.Contains(ontoNode))
            return;

        OntoNodeTarget.Add(ontoNode);
    }
}
