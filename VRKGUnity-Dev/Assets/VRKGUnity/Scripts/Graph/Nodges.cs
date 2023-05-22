using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodges
{
    public Dictionary<int, Node> NodesDicId;
    public Dictionary<int, Edge> EdgesDicId;


    public Nodges()
    {
        NodesDicId = new();
        EdgesDicId = new();
    }
}
