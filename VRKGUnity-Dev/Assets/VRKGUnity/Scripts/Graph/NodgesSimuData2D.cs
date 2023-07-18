using System.Collections.Generic;
using UnityEngine;

public class NodgesSimuData2D
{
    public Dictionary<int, NodeSimuData2D> NodeSimuDatas;
    public Dictionary<int, EdgeSimuData> EdgeSimuDatas;

    public NodgesSimuData2D(Dictionary<int, NodeSimuData2D> nodeSimuDatas, Dictionary<int, EdgeSimuData> edgeSimuDatas)
    {
        NodeSimuDatas = nodeSimuDatas;
        EdgeSimuDatas = edgeSimuDatas;
    }
}

public class NodeSimuData2D
{
    public int Id;
    public Vector2 Position;
    public Vector2 Velocity;

    public NodeSimuData2D(int id, Vector3 position)
    {
        Id = id;
        Position = new Vector2(position.x, position.y);
        Velocity = Vector2.zero;
    }

    public NodeSimuData2D Clone()
    {
        return new NodeSimuData2D(Id, Position);
    }
}