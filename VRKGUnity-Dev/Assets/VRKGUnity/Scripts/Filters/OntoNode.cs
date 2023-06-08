using Newtonsoft.Json;
using System.Collections.Generic;

public class OntoNode
{
    public int Id;
    public string Value;

    public int Depth = int.MaxValue;

    public List<OntoNode> NodeSource;
    public List<OntoNode> NodeTarget;

    public List<OntoEdge> EdgeSource;
    public List<OntoEdge> EdgeTarget;

    public OntoNode(int id, string value)
    {
        Id = id;
        Value = value;  

        NodeSource = new();
        NodeTarget = new();

        EdgeSource = new();
        EdgeTarget = new();
    }
}
