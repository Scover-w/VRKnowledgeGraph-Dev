using System.Collections.Generic;
using UnityEngine;




public class NodgesSimuData
{
    public Dictionary<int, NodeSimuData> NodeSimuDatas;
    public Dictionary<int, EdgeSimuData> EdgeSimuDatas;

    public NodgesSimuData(Dictionary<int, NodeSimuData> nodeSimuDatas, Dictionary<int, EdgeSimuData> edgeSimuDatas)
    {
        NodeSimuDatas = nodeSimuDatas;
        EdgeSimuDatas = edgeSimuDatas;
    }

    public NodgesSimuData(NodgesDicId nodges)
    {
        NodeSimuDatas = nodges.NodesDicId.ToSimuDatas();
        EdgeSimuDatas = nodges.EdgesDicId.ToSimuDatas();
    }

}

public class NodeSimuData
{
    public int Id;
    public Vector3 Position;
    public Vector3 Velocity;

    public NodeSimuData(int id, Vector3 position, Vector3 velocity)
    {
        Id = id;
        Position = position;
        Velocity = velocity;
    }

    public NodeSimuData Clone()
    {
        return new NodeSimuData(Id, Position, Velocity);
    }
}

public class EdgeSimuData
{
    public int IdA;
    public int IdB;

    public EdgeSimuData(int idA, int idB)
    {
        IdA = idA;
        IdB = idB;
    }
}