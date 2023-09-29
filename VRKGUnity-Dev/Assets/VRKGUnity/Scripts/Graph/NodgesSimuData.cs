using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Contains the only useful datas for the <see cref="GraphSimulation"/>
/// </summary>
public class NodgesSimuData
{
    public Dictionary<string, NodeSimuData> NodeSimuDatas;
    public Dictionary<string, EdgeSimuData> EdgeSimuDatas;

    public NodgesSimuData(Dictionary<string, NodeSimuData> nodeSimuDatas, Dictionary<string, EdgeSimuData> edgeSimuDatas)
    {
        NodeSimuDatas = nodeSimuDatas;
        EdgeSimuDatas = edgeSimuDatas;
    }

    public NodgesSimuData(NodgesDicUID nodges)
    {
        NodeSimuDatas = nodges.NodesDicUID.ToSimuDatas();
        EdgeSimuDatas = nodges.EdgesDicUID.ToSimuDatas();
    }

}

/// <summary>
/// Contains the only useful node datas for the <see cref="GraphSimulation"/>
/// </summary>
public class NodeSimuData
{
    public string UID;
    public Vector3 Position;
    public Vector3 Velocity;

    public NodeSimuData(string uid, Vector3 position)
    {
        UID = uid;
        Position = position;
        Velocity = Vector3.zero;
    }

    public NodeSimuData(string uid, Vector3 position, Vector3 velocity)
    {
        UID = uid;
        Position = position;
        Velocity = velocity;
    }

    public NodeSimuData Clone()
    {
        return new NodeSimuData(UID, Position, Velocity);
    }
}

/// <summary>
/// Contains the only useful edge datas for the <see cref="GraphSimulation"/>
/// </summary>
public class EdgeSimuData
{
    public string UidA;
    public string UidB;

    public EdgeSimuData(string uidA, string uidB)
    {
        UidA = uidA;
        UidB = uidB;
    }
}