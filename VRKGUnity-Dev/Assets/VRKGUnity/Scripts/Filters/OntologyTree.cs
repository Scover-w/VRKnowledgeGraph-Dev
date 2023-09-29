using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to then create the <see cref="OntoNodeGroupTree"/>
/// </summary>
public class OntologyTree
{
    public IReadOnlyDictionary<string, OntoNode> OntoNodes => _ontoNodes;

    public OntoNode RootOntoNode { get { return _rootOntoNode; } }

    OntoNode _rootOntoNode;

    readonly Dictionary<string, OntoNode> _ontoNodes;
    readonly string _namespce;


    public OntologyTree(string namespce)
    {
        _namespce = namespce;
        _ontoNodes = new();
    }

    public void AddOntoNode(OntoNode ontoNode)
    {
        _ontoNodes.TryAdd(ontoNode.UID, ontoNode);
    }

    public OntoNode TryGetOrCreateOntoNode(OntoNode ontoNode)
    {
        if (_ontoNodes.TryGetValue(ontoNode.UID, out OntoNode ontoNodeResult))
            return ontoNodeResult;

        _ontoNodes.Add(ontoNode.UID, ontoNode);

        return ontoNode;
    }


    public void SetRootAndDepth()
    {
        if (_ontoNodes.Count == 0)
            return;

        _rootOntoNode = new OntoNode(_namespce + "rootOntologyTree", true);

        foreach(OntoNode node in _ontoNodes.Values)
        {
            if (node.OntoNodeSource.Count > 0)
                continue;

            node.OntoNodeSource.Add(_rootOntoNode);
            _rootOntoNode.OntoNodeTarget.Add(node);
        }

        SetDepth(_rootOntoNode, 0);
    }

    private void SetDepth(OntoNode ontoNode, int depth)
    {
        if (ontoNode.Depth < depth)
            return;

        ontoNode.Depth = depth;
        depth++;


        var nodeTarget = ontoNode.OntoNodeTarget;


        foreach (var ontoNodeChild in nodeTarget)
        {
            SetDepth(ontoNodeChild, depth);
        }
    }

    public OntoNode GetOntoNode(string uid)
    {
        if (!_ontoNodes.TryGetValue(uid, out var ontoNode))
        {
            Debug.LogWarning("OntologyTree : Couldn't get onto node with id");
            return null;
        }

        return ontoNode;
    }

    public bool TryGetOntoNode(string uid, out OntoNode ontoNode)
    {
        ontoNode = null;
        if (!_ontoNodes.TryGetValue(uid, out var oNode))
        {
            return false;
        }

        ontoNode = oNode;
        return true;
    }

    public void DetachNodesFromOntoNodes()
    {
        foreach(var idAndOntoNode in _ontoNodes)
        {
            var ontoNode = idAndOntoNode.Value;
            ontoNode.NodesAttached = new();
        }
    }
}
