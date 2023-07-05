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

    [SerializeField]
    GraphStyling _graphStyling;

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


        HashSet<Node> nodesToKeep = new();

        foreach (Node node in selectedNodes)
        {
            nodesToKeep.Add(node);
        }

        var graph = _graphManager.Graph;

        DynamicFilter filter = graph.HideAllExcept(nodesToKeep);
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


        HashSet<Node> nodesToKeep = new();

        foreach (Node node in propagatedNodes)
        {
            nodesToKeep.Add(node);
        }

        var graph = _graphManager.Graph;

        DynamicFilter filter = graph.HideAllExcept(nodesToKeep);
        _filters.Add(filter);

        HashSetNodges nodges = new(filter.HiddenNodes, filter.HiddenEdges);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    [ContextMenu("Cancel Last Filter")]
    public void CancelLastFilter()
    {
        if (_filters.Count == 0)
            return;

        var filterIdToCancel = _filters.Count - 1;
        var filterToCancel = _filters[filterIdToCancel];
        _filters.RemoveAt(filterIdToCancel);

        _graphManager.Graph.CancelFilter(filterToCancel);

        StyleChange styleChange = new StyleChange().Add(StyleChangeType.All);
        _graphStyling.StyleGraph(styleChange, _graphManager.GraphMode);
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