using System.Collections.Generic;

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
