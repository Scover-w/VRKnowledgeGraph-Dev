using System;
using System.Collections.Generic;

public class OntoNode
{
    /// <summary>
    /// Frozen OntoNode is a ontoNode that don't exist in the KG because it has been added when creating Trees for layout structure
    /// </summary>
    public bool IsFrozen { get { return _isFrozen; } }


    public string UID => Value;

    public string Value { get; }

    public int Depth = int.MaxValue;

    public List<OntoNode> OntoNodeSource;
    public List<OntoNode> OntoNodeTarget;

    public List<Node> NodesAttached;

    public OntoNodeGroup OntoNodeGroup;

    bool _isFrozen;

    public OntoNode(string value, bool isFrozen = false)
    {
        Value = value;

        OntoNodeSource = new();
        OntoNodeTarget = new();

        NodesAttached = new();
        _isFrozen = isFrozen;
    }

    public void AddOntoNodeSource(OntoNode ontoNode)
    {
        if (OntoNodeSource.Contains(ontoNode))
            return;

        OntoNodeSource.Add(ontoNode);
    }

    public void AddOntoNodeTarget(OntoNode ontoNode)
    {
        if (OntoNodeTarget.Contains(ontoNode))
            return;

        OntoNodeTarget.Add(ontoNode);
    }


    public bool CreateGroupIfOwnAttachedNode(out OntoNodeGroup ontoNodeGroup)
    {
        ontoNodeGroup = null;

        if (NodesAttached.Count == 0)
            return false;

        OntoNodeGroup = new OntoNodeGroup(this);
        ontoNodeGroup = OntoNodeGroup;

        foreach (Node node in NodesAttached)
        {
            OntoNodeGroup.AddNode(node);
        }

        return true;
    }


    public void CreateOntoGroupTree(OntoNodeGroup parentOntoGroup)
    {
        if(parentOntoGroup != null && OntoNodeGroup != null)
        {
            parentOntoGroup.LinkTo(OntoNodeGroup);
        }

        var currentparentOntoGroup = (OntoNodeGroup != null) ? OntoNodeGroup : parentOntoGroup;

        foreach (OntoNode ontoNode in OntoNodeTarget)
        {
            ontoNode.CreateOntoGroupTree(currentparentOntoGroup);
        }
    }
}
