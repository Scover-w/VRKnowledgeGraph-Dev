using System.Collections.Generic;

public static class NodgeExtensions
{
    public static void DesactivateAll(this List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            node.DisplayMainNode(false);
        }
    }

    public static void DesactivateAll(this List<Edge> edges)
    {
        foreach (Edge edge in edges)
        {
            edge.DisplayMainEdge(false);
        }
    }
}