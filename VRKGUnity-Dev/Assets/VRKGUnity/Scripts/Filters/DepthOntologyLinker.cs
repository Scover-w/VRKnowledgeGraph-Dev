using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// With inference at true, all parent subClassOf are link to nodes.
/// This script allow to select the deepest subClassOf, the only one when inference is at false
/// </summary>
public class DepthOntologyLinker
{
    readonly GraphDbRepositoryNamespaces _repoNamespaces;

    /// <summary>
    /// string is a node uri
    /// </summary>
    readonly Dictionary<string, DepthOntologyLink> _depthOntologyLinks;

    public DepthOntologyLinker(GraphDbRepositoryNamespaces repoNamespaces)
    {
        _repoNamespaces = repoNamespaces;
        _depthOntologyLinks = new();
    }


    /// <summary>
    /// Try to link a Node to an OntoNode
    /// </summary>
    public bool TryEstablishLink(Node candidateDefinedNode, Node candidateOntoNode)
    {
        if(!_repoNamespaces.DoesOntoNodeExistInOntologyTree(candidateOntoNode, out OntoNode ontoNode))
            return false;

        var uri = candidateDefinedNode.Uri;

        // Check if a DeepOntologyLink has already been created for this node
        if(!_depthOntologyLinks.TryGetValue(uri, out DepthOntologyLink deepOntologyLink))
        {
            deepOntologyLink = new(candidateDefinedNode, ontoNode);
            _depthOntologyLinks.Add(uri, deepOntologyLink);
            return true;
        }
         
        deepOntologyLink.TrySetDeeperOntoNode(ontoNode);
        return true;
    }

    public void AttachNodesToOntoNodes()
    {
        foreach(DepthOntologyLink deepOntologyLink in _depthOntologyLinks.Values)
        {
            deepOntologyLink.AttachNodeToOntoNode();
        }
    }

}

/// <summary>
/// Allow to select the deepest OntoNode for the Node to link
/// </summary>
public class DepthOntologyLink
{
    Node _nodeToLink;
    OntoNode _currentOntoNodeToAttach;

    public DepthOntologyLink(Node node, OntoNode currentOntoNodeToAttach)
    {
        _nodeToLink = node;
        _currentOntoNodeToAttach = currentOntoNodeToAttach;
    }

    public void TrySetDeeperOntoNode(OntoNode newOntoNode)
    {        
        // Check if the new OntoNode is deeper that the already present one
        if (newOntoNode.Depth <= _currentOntoNodeToAttach.Depth)
            return;

        _currentOntoNodeToAttach = newOntoNode;
    }

    public void AttachNodeToOntoNode()
    {
        _currentOntoNodeToAttach.NodesAttached.Add(_nodeToLink);
    }

}