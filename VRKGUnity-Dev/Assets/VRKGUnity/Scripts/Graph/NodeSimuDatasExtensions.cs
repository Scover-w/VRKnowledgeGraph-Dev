using System.Collections.Generic;

public static class NodeSimuDatasExtensions
{
    #region GraphSimulation
    public static Dictionary<int, NodeSimuData> Clone(this Dictionary<int, NodeSimuData> nodeSimuDatas)
    {
        Dictionary<int, NodeSimuData> cloned = new();

        foreach (var idAndData in nodeSimuDatas)
        {
            cloned.Add(idAndData.Key, idAndData.Value.Clone());
        }

        return cloned;
    }

    public static Dictionary<int, NodeSimuData> ToSimuDatas(this Dictionary<int, Node> nodesDicId)
    {
        Dictionary<int, NodeSimuData> nodeSimuDatas = new();

        foreach (var idAndNode in nodesDicId)
        {
            nodeSimuDatas.Add(idAndNode.Key, idAndNode.Value.ToSimuData());
        }

        return nodeSimuDatas;
    }

    public static Dictionary<int, EdgeSimuData> ToSimuDatas(this Dictionary<int, Edge> edgesDicId)
    {
        Dictionary<int, EdgeSimuData> edgeSimuDatas = new();

        foreach (var idAndEdge in edgesDicId)
        {
            edgeSimuDatas.Add(idAndEdge.Key, idAndEdge.Value.ToSimuData());
        }

        return edgeSimuDatas;
    }
    #endregion


    #region LensSimulation
    public static Dictionary<int, NodeSimuData2D> Clone(this Dictionary<int, NodeSimuData2D> nodeSimuDatas)
    {
        Dictionary<int, NodeSimuData2D> cloned = new();

        foreach (var idAndData in nodeSimuDatas)
        {
            cloned.Add(idAndData.Key, idAndData.Value.Clone());
        }

        return cloned;
    }

    public static Dictionary<int, NodeSimuData2D> ToSimuDatas2D(this Dictionary<int, Node> nodesAndId)
    {
        Dictionary<int, NodeSimuData2D> nodeSimuDatas2D = new();

        foreach (var nodeAndId in nodesAndId)
        {
            nodeSimuDatas2D.Add(nodeAndId.Key, nodeAndId.Value.ToSimuData2D());
        }

        return nodeSimuDatas2D;
    }


    public static Dictionary<int, EdgeSimuData> ToSimuDatas(this HashSet<Edge> edges)
    {
        Dictionary<int, EdgeSimuData> edgeSimuDatas = new();

        foreach (Edge edge in edges)
        {
            edgeSimuDatas.Add(edge.Id, edge.ToSimuData());
        }

        return edgeSimuDatas;
    }
    #endregion
}
