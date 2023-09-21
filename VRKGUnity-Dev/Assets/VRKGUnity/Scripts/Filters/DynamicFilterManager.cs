using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicFilterManager : MonoBehaviour
{
    public int NbFilter { get { return _filters.Count; } }
    public int NbRedoFilter { get { return _redoFilters.Count; } }

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgeSelectionManager _nodeSelectionManager;

    [SerializeField]
    StylingManager _stylingManager;

    [SerializeField]
    NodgePool _nodgePool;


    HashSet<Node> _displayedNodes;
    HashSet<Node> _hiddenNodes = new();

    List<DynamicFilter> _filters = new();
    List<DynamicFilter> _redoFilters = new();


    public void SetNodes(NodgesDicUID nodgesDic)
    {
        if(_displayedNodes != null)
        {
            DebugDev.LogWarning("DynamicFilterManager : SetNodes already called");
            return;
        }

        _displayedNodes = new();
        var nodesDic = nodgesDic.NodesDicUID;

        foreach(var kvp in nodesDic)
        {
            Node node = kvp.Value;
            _displayedNodes.Add(node);
        }
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
        _redoFilters = new();

        HashSetNodges nodges = new(filter.HiddenNodes, filter.HiddenEdges);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    [ContextMenu("Hide Propagated Node")]
    public void HidePropagatedNode()
    {
        var selectedNodes = _nodeSelectionManager.SelectedNodes;
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
        _redoFilters = new();

        HashSetNodges nodges = new(filter.HiddenNodes, filter.HiddenEdges);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    [ContextMenu("Hide Unpropagated Node")]
    public void HideUnPropagatedNode()
    {
        var selectedNodes = _nodeSelectionManager.SelectedNodes;
        var propagatedNodes = _nodeSelectionManager.PropagatedNodes;

        if (propagatedNodes == null)
            return;


        HashSet<Node> nodesToKeep = new();

        foreach (Node node in propagatedNodes)
        {
            nodesToKeep.Add(node);
        }

        HashSet<Node> nodesToFilter = new();

        foreach (Node node in selectedNodes)
        {
            nodesToFilter.Add(node);
        }

        var graph = _graphManager.Graph;

        DynamicFilter filter = graph.HideAllExcept(nodesToKeep, nodesToFilter);
        _filters.Add(filter);
        _redoFilters = new();

        HashSetNodges nodges = new(filter.HiddenNodes, filter.HiddenEdges);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    [ContextMenu("Undo Last Filter")]
    public void UndoLastFilter()
    {
        if (_filters.Count == 0)
            return;

        var filterIdToCancel = _filters.Count - 1;
        var filterToCancel = _filters[filterIdToCancel];
        _filters.RemoveAt(filterIdToCancel);

        _redoFilters.Add(filterToCancel);

        _graphManager.Graph.UndoFilter(filterToCancel);

        StyleChange styleChange = StyleChange.All; // TODO : Put real styleChange
        _stylingManager.UpdateStyling(styleChange);
    }

    [ContextMenu("Redo Last Filter")]
    public void RedoLastFilter()
    {
        if (_redoFilters.Count == 0)
            return;

        var filterIdToCancel = _redoFilters.Count - 1;
        var filterToCancel = _redoFilters[filterIdToCancel];
        _redoFilters.RemoveAt(filterIdToCancel);

        _filters.Add(filterToCancel);
        _graphManager.Graph.RedoFilter(filterToCancel);

        StyleChange styleChange = StyleChange.All;
        _stylingManager.UpdateStyling(styleChange);
    }

    public void ResetFilters()
    {
        // TODO : resetFilters
    }

    public List<SPARQLQuery> ApplyFilters()
    {
        List<SPARQLQuery> sparqlQueries = new();

        foreach(DynamicFilter filter in _filters)
        {
            sparqlQueries.Add(filter.Query);
            filter.ReleaseNodges(_nodgePool);
        }

        _filters = new();
        _redoFilters = new();

        return sparqlQueries;
    }
}