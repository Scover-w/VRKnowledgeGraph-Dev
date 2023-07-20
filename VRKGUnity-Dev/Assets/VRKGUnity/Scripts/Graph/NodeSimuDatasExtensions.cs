using System.Collections.Generic;

public static class NodeSimuDatasExtensions
{
    #region GraphSimulation
    public static Dictionary<string, NodeSimuData> Clone(this Dictionary<string, NodeSimuData> nodeSimuDatas)
    {
        Dictionary<string, NodeSimuData> cloned = new();

        foreach (var idAndData in nodeSimuDatas)
        {
            cloned.Add(idAndData.Key, idAndData.Value.Clone());
        }

        return cloned;
    }

    public static Dictionary<string, NodeSimuData> ToSimuDatas(this Dictionary<string, Node> nodesDicUID)
    {
        Dictionary<string, NodeSimuData> nodeSimuDatas = new();

        foreach (var uidAndNode in nodesDicUID)
        {
            nodeSimuDatas.Add(uidAndNode.Key, uidAndNode.Value.ToSimuData());
        }

        return nodeSimuDatas;
    }

    public static Dictionary<string, EdgeSimuData> ToSimuDatas(this Dictionary<string, Edge> edgesDicUID)
    {
        Dictionary<string, EdgeSimuData> edgeSimuDatas = new();

        foreach (var uidAndEdge in edgesDicUID)
        {
            edgeSimuDatas.Add(uidAndEdge.Key, uidAndEdge.Value.ToSimuData());
        }

        return edgeSimuDatas;
    }
    #endregion


    #region LensSimulation
    public static Dictionary<string, NodeSimuData2D> Clone(this Dictionary<string, NodeSimuData2D> nodeSimuDatas)
    {
        Dictionary<string, NodeSimuData2D> cloned = new();

        foreach (var idAndData in nodeSimuDatas)
        {
            cloned.Add(idAndData.Key, idAndData.Value.Clone());
        }

        return cloned;
    }

    public static Dictionary<string, NodeSimuData2D> ToSimuDatas2D(this Dictionary<string, Node> nodesAndId)
    {
        Dictionary<string, NodeSimuData2D> nodeSimuDatas2D = new();

        foreach (var nodeAndId in nodesAndId)
        {
            nodeSimuDatas2D.Add(nodeAndId.Key, nodeAndId.Value.ToSimuData2D());
        }

        return nodeSimuDatas2D;
    }


    public static Dictionary<string, EdgeSimuData> ToSimuDatas(this HashSet<Edge> edges)
    {
        Dictionary<string, EdgeSimuData> edgeSimuDatas = new();

        foreach (Edge edge in edges)
        {
            edgeSimuDatas.Add(edge.UID, edge.ToSimuData());
        }

        return edgeSimuDatas;
    }
    #endregion
}
