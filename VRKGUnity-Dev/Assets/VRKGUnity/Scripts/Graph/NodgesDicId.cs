using System.Collections.Generic;

public class NodgesDicId
{
    public Dictionary<int, Node> NodesDicId;
    public Dictionary<int, Edge> EdgesDicId;


    public NodgesDicId()
    {
        NodesDicId = new();
        EdgesDicId = new();
    }

    public void ResetAbsolutePosition(GraphConfiguration config)
    {
        int seed = config.SeedRandomPosition;


        foreach (var idAndNode in NodesDicId)
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
