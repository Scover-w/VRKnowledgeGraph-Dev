using System.Collections.Generic;

public class OntoNodeGroup
{
    public string UID { get { return OntoNode.UID; } }
    public int Depth { get { return _depth; } }

    public int NodeCount { get { return Nodes.Count; } }

    public int Height { get { return _height; } }
    public float ColorValue { get { return _colorValue; } }

    public OntoNode OntoNode;

    public List<Node> Nodes;

    public List<OntoNodeGroup> OntoNodeGroupParent; 
    public List<OntoNodeGroup> OntoNodeGroupChild;

    float _colorValue;

    int _height;
    int _depth;

    public OntoNodeGroup(OntoNode ontoNode)
    {
        OntoNode = ontoNode;
        ontoNode.OntoNodeGroup = this;

        Nodes = new List<Node>();
        OntoNodeGroupParent = new();
        OntoNodeGroupChild = new();
        _height = int.MaxValue;
    }
    
    public OntoNode GetUpperOntoNode(bool wantSpreadOut)
    {
        if (OntoNode.IsFrozen)
            return null;


        if(OntoNode.OntoNodeSource.Count == 0)
            return null;

        if (OntoNode.OntoNodeSource.Count == 1)
            return OntoNode.OntoNodeSource[0];


        OntoNode selectedOntoNode = OntoNode.OntoNodeSource[0];
        int minMaxNbNode = wantSpreadOut ? int.MinValue : int.MaxValue;


        // TODO : Mode that select the ontoNode with the less depth value

        foreach(OntoNode ontoNode in OntoNode.OntoNodeSource)
        {
            if(wantSpreadOut) // select the ontoNode with least attachedNode to group
            {
                if (ontoNode.OntoNodeGroup == null)
                    return ontoNode;

                int nbAttachedToGroup = ontoNode.OntoNodeGroup.NodeCount;
                if (nbAttachedToGroup >= minMaxNbNode)
                    continue;

                minMaxNbNode = nbAttachedToGroup;
                selectedOntoNode = ontoNode;

            }
            else // select the ontoNode with the more attachedNode to group
            {
                if (ontoNode.OntoNodeGroup == null)
                    continue;

                int nbAttachedToGroup = ontoNode.OntoNodeGroup.NodeCount;
                if (nbAttachedToGroup <= minMaxNbNode)
                    continue;

                minMaxNbNode = nbAttachedToGroup;
                selectedOntoNode = ontoNode;
            }
        }


        return selectedOntoNode;
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

    public void RemoveFromOntoNode()
    {
        if(OntoNode == null) 
            return;

        OntoNode.OntoNodeGroup = null;
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

    public void ComputeDepthDownward(int depth)
    {
        _depth = depth;

        foreach (OntoNodeGroup ontogroupChild in OntoNodeGroupChild)
        {
            ontogroupChild.ComputeDepthDownward(depth + 1);
        }
    }

    public int ComputeDepthUpward()
    {
        if (_depth == 0) // Root
            return 0;

        int minDepthParent = int.MaxValue;

        foreach(OntoNodeGroup ontogroupParent in OntoNodeGroupParent)
        {
            var depthParent = ontogroupParent.ComputeDepthUpward();

            if (depthParent >= minDepthParent)
                continue;

            minDepthParent = depthParent;
        }

        _depth = minDepthParent + 1;

        return _depth; 
    }

    public float ComputeColorValueAndSetToNode(float value, float deltaValue)
    {
        if(Nodes.Count != 0)
        {
            _colorValue = value;
            value += deltaValue;
        }

        foreach(OntoNodeGroup ontoGroupChild in OntoNodeGroupChild)
        {
            value = ontoGroupChild.ComputeColorValueAndSetToNode(value, deltaValue);
        }

        foreach(Node node in Nodes)
        {
            node.OntoNodeGroup = this;
        }

        return value;
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
