using System.Collections.Generic;
using UnityEngine;

public class NodgesSimuData2D
{
    public Dictionary<string, NodeSimuData2D> NodeSimuDatas;
    public Dictionary<string, EdgeSimuData> EdgeSimuDatas;

    public NodgesSimuData2D(Dictionary<string, NodeSimuData2D> nodeSimuDatas, Dictionary<string, EdgeSimuData> edgeSimuDatas)
    {
        NodeSimuDatas = nodeSimuDatas;
        EdgeSimuDatas = edgeSimuDatas;
    }
}

public class NodeSimuData2D
{
    public string UID;
    public Vector2 Position;
    public Vector2 Velocity;

    public NodeSimuData2D(string uid, Vector3 position)
    {
        UID = uid;
        Position = new Vector2(position.x, position.y);
        Velocity = Vector2.zero;
    }

    public NodeSimuData2D Clone()
    {
        return new NodeSimuData2D(UID, Position);
    }
}