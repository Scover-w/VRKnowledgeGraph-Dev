using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class OntoNodeGroup
{
    public int Id { get { return OntoNode.Id; } }
    public int Depth { get { return OntoNode.Depth; } }

    public OntoNode OntoNode;

    public List<Node> Nodes;


    public OntoNodeGroup(OntoNode ontoNode)
    {
        OntoNode = ontoNode;
        Nodes = new List<Node>();
    }
    
    public OntoNode GetUpperOntoNode()
    {
        if (OntoNode.OntoNodeSource.Count == 0)
            return null;

        return OntoNode.OntoNodeSource[0];
    }

    public void SendNodesTo(OntoNodeGroup ontoGroup)
    {
        foreach (Node node in Nodes) 
        { 
            ontoGroup.Nodes.Add(node);
        }
    }

}
