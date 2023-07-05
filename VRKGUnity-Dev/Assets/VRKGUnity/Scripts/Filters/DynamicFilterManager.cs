using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicFilterManager : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgeSelectionManager _nodeSelectionManager;


    List<DynamicFilter> _filters;

    private void Start()
    {
        _filters = new();
    }

    [ContextMenu("Hide Selected Node")]
    public void HideSelectedNode()
    {
        var selectedNodes = _nodeSelectionManager.SelectedNodes;

        if (selectedNodes == null)
            return;


        HashSet<Node> nodesToHide = new();

        foreach(Node node in selectedNodes)
        {
            nodesToHide.Add(node);
        }

        var graph = _graphManager.Graph;

        DynamicFilter filter = graph.Hide(nodesToHide);
        _filters.Add(filter);


        HashSetNodges nodges = new(filter.HiddenNodes, filter.HiddenEdges);
        _nodeSelectionManager.NodgesHidden(nodges);

    }

    [ContextMenu("Hide Unselected Node")]
    public void HideUnSelectedNode()
    {
        var selectedNodes = _nodeSelectionManager.SelectedNodes;

        if (selectedNodes == null)
            return;


        HashSet<Node> nodesToHide = new();

        foreach (Node node in selectedNodes)
        {
            nodesToHide.Add(node);
        }

        var graph = _graphManager.Graph;

        DynamicFilter filter = graph.HideAllExcept(nodesToHide);
        _filters.Add(filter);

        HashSetNodges nodges = new(filter.HiddenNodes, filter.HiddenEdges);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    [ContextMenu("Hide Propagated Node")]
    public void HidePropagatedNode()
    {
        var propagatedNodes = _nodeSelectionManager.PropagatedNodes;

        if (propagatedNodes == null)
            return;


        HashSet<Node> nodesToHide = new();

        foreach (Node node in propagatedNodes)
        {
            nodesToHide.Add(node);
        }

        var graph = _graphManager.Graph;

        DynamicFilter filter = graph.Hide(nodesToHide);
        _filters.Add(filter);

        HashSetNodges nodges = new(filter.HiddenNodes, filter.HiddenEdges);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    [ContextMenu("Hide Unpropagated Node")]
    public void HideUnPropagatedNode()
    {
        var propagatedNodes = _nodeSelectionManager.PropagatedNodes;

        if (propagatedNodes == null)
            return;


        HashSet<Node> nodesToHide = new();

        foreach (Node node in propagatedNodes)
        {
            nodesToHide.Add(node);
        }

        var graph = _graphManager.Graph;

        DynamicFilter filter = graph.HideAllExcept(nodesToHide);
        _filters.Add(filter);

        HashSetNodges nodges = new(filter.HiddenNodes, filter.HiddenEdges);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    public List<SPARQLQuery> ApplyFilters()
    {
        List<SPARQLQuery> sparqlQueries = new();

        foreach(DynamicFilter filter in _filters)
        {
            sparqlQueries.Add(filter.Query);
        }

        _filters = new();

        return sparqlQueries;
    }
}