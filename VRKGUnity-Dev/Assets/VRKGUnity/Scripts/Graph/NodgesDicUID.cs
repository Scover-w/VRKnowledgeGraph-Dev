using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Allow to pass/store Node and Edge more easily between functions
/// </summary>
public class NodgesDicUID
{
    public Dictionary<string, Node> NodesDicUID;
    public Dictionary<string, Edge> EdgesDicUID;


    public NodgesDicUID()
    {
        NodesDicUID = new();
        EdgesDicUID = new();
    }

    public void ResetAbsolutePosition(GraphConfiguration config)
    {
        int seed = config.SeedRandomPosition;


        foreach (var idAndNode in NodesDicUID)
        {
            idAndNode.Value.ResetAbsolutePosition(seed);
        }
    }
}


/// <summary>
/// Allow to pass/store Node and Edge more easily between functions
/// </summary>
public class Nodges
{
    public List<Node> Nodes;
    public List<Edge> Edges;


    public Nodges()
    {
        Nodes = new();
        Edges = new();
    }

    public Nodges(List<Node> nodes, List<Edge> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }
}


/// <summary>
/// Allow to pass/store Node and Edge more easily between functions
/// </summary>
public class HashSetNodges
{
    public HashSet<Node> Nodes;
    public HashSet<Edge> Edges;


    public HashSetNodges()
    {
        Nodes = new();
        Edges = new();
    }

    public HashSetNodges(HashSet<Node> nodes, HashSet<Edge> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    public HashSetNodges(NodgesDicUID nodgesDicUID)
    {
        Nodes = new();
        Edges = new();

        var nodesDict = nodgesDicUID.NodesDicUID;
        var edgesDict = nodgesDicUID.EdgesDicUID;

        foreach (var node in nodesDict.Values) 
        {
            Nodes.Add(node);
        }

        foreach (var edge in edgesDict.Values)
        {
            Edges.Add(edge);
        }
    }

}
