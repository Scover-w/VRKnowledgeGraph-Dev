using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// Ontology edge
/// </summary>
public class OntoEdge
{
    public int Id;
    public string Value;

    [JsonIgnore]
    public List<OntoNode> NodeSource;
    [JsonIgnore]
    public List<OntoNode> NodeTarget;


    public OntoEdge(int id, string value) 
    { 
        Id = id;
        Value = value;

        NodeSource = new();
        NodeTarget = new();
    }

}