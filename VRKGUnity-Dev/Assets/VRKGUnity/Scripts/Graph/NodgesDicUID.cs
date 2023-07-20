using System.Collections.Generic;

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
}
