using System.Collections.Generic;


/// <summary>
/// With inference at true, all parent subClassOf are link to nodes.
/// This script allow to select the deepest subClassOf, the only one when inference is at false
/// </summary>
public class DepthOntologyLinker
{

    GraphDbRepositoryNamespaces _repoNamespaces;

    Dictionary<string, DeepOntologyLink> _deepOntologyLinks;

    public DepthOntologyLinker(GraphDbRepositoryNamespaces repoNamespaces)
    {
        _repoNamespaces = repoNamespaces;
        _deepOntologyLinks = new();
    }


    public bool TryLink(Node definedNode, Node simpleOntoNode)
    {
        if(!_repoNamespaces.CanAddNodeToOntoNode(simpleOntoNode, out OntoNode ontoNode))
            return false;

        var value = definedNode.Value;


        if(!_deepOntologyLinks.TryGetValue(value, out DeepOntologyLink deepOntologyLink))
        {
            deepOntologyLink = new(definedNode);
            _deepOntologyLinks.Add(value, deepOntologyLink);
        }

        deepOntologyLink.TryAdd(ontoNode);
        return true;
    }

    public void AttachNodesToOntoNodes()
    {
        foreach(DeepOntologyLink deepOntologyLink in _deepOntologyLinks.Values)
        {
            deepOntologyLink.Attach();
        }
    }

}

public class DeepOntologyLink
{
    Node _node;

    Dictionary<string, OntoNode> _deepOntoNodeLinks;

    public DeepOntologyLink(Node node)
    {
        _node = node;
        _deepOntoNodeLinks = new();
    }

    public void TryAdd(OntoNode ontoNode)
    {
        var namespce = ontoNode.Value.ExtractUri().namespce;
        
        if(!_deepOntoNodeLinks.TryGetValue(namespce, out OntoNode currentOntoNode))
        {
            _deepOntoNodeLinks.Add(namespce, ontoNode);
            return;
        }

        if (ontoNode.Depth <= currentOntoNode.Depth)
            return;


        _deepOntoNodeLinks[namespce] = ontoNode;
    }

    public void Attach()
    {
        foreach(OntoNode ontoNode in _deepOntoNodeLinks.Values)
        {
            ontoNode.NodesAttached.Add(_node);
        }
    }

}