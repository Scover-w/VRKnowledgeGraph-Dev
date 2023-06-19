using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class OntoNodeGroup
{
    public int Id { get { return OntoNode.Id; } }
    public int Depth { get { return OntoNode.Depth; } }

    public int NbNode { get { return Nodes.Count; } }

    public int Height { get { return _height; } }

    public OntoNode OntoNode;

    public List<Node> Nodes;

    public List<OntoNodeGroup> OntoNodeGroupParent; 
    public List<OntoNodeGroup> OntoNodeGroupChild; 

    int _height;

    public OntoNodeGroup(OntoNode ontoNode)
    {
        OntoNode = ontoNode;
        Nodes = new List<Node>();
        OntoNodeGroupParent = new();
        OntoNodeGroupChild = new();
        _height = int.MaxValue;
    }
    
    public OntoNode GetUpperOntoNode()
    {
        if (OntoNode.IsFrozen)
            return null;


        if(OntoNode.OntoNodeSource.Count == 0)
            return null;

        return OntoNode.OntoNodeSource[0]; // TODO : Don't know yet how to select which Source to take
    }

    public void SendNodesTo(OntoNodeGroup ontoGroup)
    {
        foreach (Node node in Nodes) 
        { 
            ontoGroup.Nodes.Add(node);
        }
    }

    public void AddNode(Node node)
    {
        Nodes.Add(node);
    }


    /// <summary>
    /// ontoGroup parameter is the target
    /// </summary>
    /// <param name="ontoGroup"></param>
    public void LinkTo(OntoNodeGroup ontoGroup)
    {
        if (OntoNodeGroupChild.Contains(ontoGroup))
            return;

        OntoNodeGroupChild.Add(ontoGroup);



        var ontoNodeGroupParent = ontoGroup.OntoNodeGroupParent;

        if (ontoNodeGroupParent.Contains(this))
            return;

        ontoNodeGroupParent.Add(this);
    }

    public int ComputeHeight()
    {
        if(OntoNodeGroupChild.Count == 0)
        {
            _height = 0;
            return _height;
        }

        _height = int.MaxValue;


        foreach(var ontoGroupChild in OntoNodeGroupChild)
        {
            int heightChild = ontoGroupChild.ComputeHeight();

            if(heightChild < _height)
                _height = heightChild;

        }

        _height += 1;

        return _height;
    }

    

    public void RemoveFromParent()
    {
        foreach(var ontoGroupParent in OntoNodeGroupParent)
        {
            ontoGroupParent.OntoNodeGroupChild.Remove(this);
            ontoGroupParent.PropagateHeightToParent(0);
        }
    }

    public void ReplaceFromParent(OntoNodeGroup newOntoGroup)
    {
        foreach (OntoNodeGroup ontoGroupParent in OntoNodeGroupParent)
        {
            ontoGroupParent.OntoNodeGroupChild.Remove(this);
            ontoGroupParent.LinkTo(newOntoGroup);
        }

        newOntoGroup.PropagateHeightToParent(0);
    }

    public void PropagateHeightToParent(int newHeight)
    {
        if (newHeight >= _height)
            return;

        _height = newHeight;
        int heightparent = _height + 1;

        foreach (OntoNodeGroup ontoGroupParent in OntoNodeGroupParent)
        {
            ontoGroupParent.PropagateHeightToParent(heightparent);
        }
    }
}
