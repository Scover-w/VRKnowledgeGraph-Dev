using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Expressions.Functions.XPath.String;
using VDS.RDF.Writing;

public class OntologyTree
{
    public IReadOnlyDictionary<int, OntoNode> OntoNodes => _ontoNodes;

    public OntoNode RootOntoNode { get { return _rootOntoNode; } }

    OntoNode _rootOntoNode;

    Dictionary<int, OntoNode> _ontoNodes;

    string _namespce;


    public OntologyTree(string namespce)
    {
        _namespce = namespce;
        _ontoNodes = new();
    }

    public void AddOntoNode(OntoNode ontoNode)
    {
        _ontoNodes.TryAdd(ontoNode.Id, ontoNode);
    }

    public OntoNode TryGetOrCreateOntoNode(OntoNode ontoNode)
    {
        if (_ontoNodes.TryGetValue(ontoNode.Id, out OntoNode ontoNodeResult))
            return ontoNodeResult;

        _ontoNodes.Add(ontoNode.Id, ontoNode);

        return ontoNode;
    }


    public void SetRootAndDepth()
    {
        if (_ontoNodes.Count == 0)
            return;

        _rootOntoNode = new OntoNode(_namespce + "rootOntologyTree");

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

    public OntoNode GetOntoNode(int id)
    {
        if (!_ontoNodes.TryGetValue(id, out var ontoNode))
        {
            Debug.LogWarning("OntologyTree : Couldn't get onto node with id");
            return null;
        }

        return ontoNode;
    }

    public bool TryGetOntoNode(int id, out OntoNode ontoNode)
    {
        ontoNode = null;
        if (!_ontoNodes.TryGetValue(id, out var oNode))
        {
            return false;
        }

        ontoNode = oNode;
        return true;
    }

    public void ResetDefinedNodes()
    {
        foreach(var idAndOntoNode in _ontoNodes)
        {
            var ontoNode = idAndOntoNode.Value;
            ontoNode.NodesDefined = new();
        }
    }
}
