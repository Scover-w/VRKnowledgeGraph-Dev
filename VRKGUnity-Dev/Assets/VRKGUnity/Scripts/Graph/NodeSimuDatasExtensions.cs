using System.Collections.Generic;

public static class NodeSimuDatasExtensions
{
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
}
