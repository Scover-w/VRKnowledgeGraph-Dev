using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HistoryFilterManager : MonoBehaviour
{
    public bool CanUndo { get { return _currentBlockHistory.NbSubBlock > 0 || _blocksHistory.Count > 0; } }
    public bool CanRedo { get { return _redoSubBlocks.Count > 0; } }

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgeSelectionManager _nodeSelectionManager;

    [SerializeField]
    StylingManager _stylingManager;

    [SerializeField]
    NodgePool _nodgePool;

    List<BlockHistory> _blocksHistory = new();

    BlockHistory _currentBlockHistory = new();

    List<SubBlockHistory> _redoSubBlocks = new();


    [ContextMenu("Hide Selected Node")]
    public void HideSelectedNode()
    {
        var selectedNodes = _nodeSelectionManager.SelectedNodes;

        if (selectedNodes == null)
            return;

        HashSet<Node> nodesToHide = selectedNodes.ToHashSet();

        _redoSubBlocks = new();
        var graph = _graphManager.Graph;


        SubBlockHistory subBlockHistory = graph.Hide(nodesToHide, out NodgesDicUID hiddenNodgesDicUId);
        _currentBlockHistory.Add(subBlockHistory);


        HashSetNodges nodges = new(hiddenNodgesDicUId);
        _nodeSelectionManager.NodgesHidden(nodges);

    }

    [ContextMenu("Hide Unselected Node")]
    public void HideUnSelectedNode()
    {
        var selectedNodes = _nodeSelectionManager.SelectedNodes;

        if (selectedNodes == null)
            return;


        HashSet<Node> nodesToKeep = selectedNodes.ToHashSet();

        _redoSubBlocks = new();
        var graph = _graphManager.Graph;


        SubBlockHistory subBlockHistory = graph.HideAllExcept(nodesToKeep, out NodgesDicUID hiddenNodgesDicUId);
        _currentBlockHistory.Add(subBlockHistory);

        HashSetNodges nodges = new(hiddenNodgesDicUId);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    [ContextMenu("Hide Propagated Node")]
    public void HidePropagatedNode()
    {
        var selectedNodes = _nodeSelectionManager.SelectedNodes;
        var propagatedNodes = _nodeSelectionManager.PropagatedNodes;

        if (propagatedNodes == null)
            return;


        HashSet<Node> nodesToHide = propagatedNodes.ToHashSet();
        nodesToHide.UnionWith(selectedNodes.ToHashSet());

        _redoSubBlocks = new();
        var graph = _graphManager.Graph;


        SubBlockHistory subBlockHistory = graph.Hide(nodesToHide, out NodgesDicUID hiddenNodgesDicUId);
        _currentBlockHistory.Add(subBlockHistory);


        HashSetNodges nodges = new(hiddenNodgesDicUId);
        _nodeSelectionManager.NodgesHidden(nodges);
    }

    [ContextMenu("Hide Unpropagated Node")]
    public void HideUnPropagatedNode()
    {
        var selectedNodes = _nodeSelectionManager.SelectedNodes;
        var propagatedNodes = _nodeSelectionManager.PropagatedNodes;

        if (propagatedNodes == null)
            return;

        HashSet<Node> nodesToKeep = propagatedNodes.ToHashSet();
        nodesToKeep.UnionWith(selectedNodes.ToHashSet());

        _redoSubBlocks = new();
        var graph = _graphManager.Graph;


        SubBlockHistory subBlockHistory = graph.HideAllExcept(nodesToKeep, out NodgesDicUID hiddenNodgesDicUId);
        _currentBlockHistory.Add(subBlockHistory);

        HashSetNodges nodges = new(hiddenNodgesDicUId);
        _nodeSelectionManager.NodgesHidden(nodges);
    }


    [ContextMenu("Undo Last Filter")]
    public void UndoLastFilter()
    {
        if(_currentBlockHistory.NbSubBlock == 0)
        {
            if(_blocksHistory.Count == 0)
                return; // Can't undo anything

            int idToRetrieve = _blocksHistory.Count - 1;
            _currentBlockHistory = _blocksHistory[idToRetrieve];
            _redoSubBlocks.RemoveAt(idToRetrieve);

            ReapplyCurrentFilters();
            _graphManager.UpdateGraphFromHistoryFilter(GetSPARQLAdditiveBuilder());

            return;
        }

        var subBlockHistory = _currentBlockHistory.CancelLast();
        _redoSubBlocks.Add(subBlockHistory);
        _graphManager.Graph.UndoFilter(subBlockHistory);

        StyleChange styleChange = StyleChange.All; // TODO : Put real styleChange
        _stylingManager.UpdateStyling(styleChange);
    }

    private void ReapplyCurrentFilters()
    {
        var subBlocks = _currentBlockHistory.SubsHistory;

        foreach (var subBlock in subBlocks) 
        {
            _graphManager.Graph.RedoFilter(subBlock);
        }

        StyleChange styleChange = StyleChange.All;
        _stylingManager.UpdateStyling(styleChange);
    }

    [ContextMenu("Redo Last Filter")]
    public void RedoLastFilter()
    {
        if (_redoSubBlocks.Count == 0)
            return;


        int idToCancel = _redoSubBlocks.Count - 1;
        SubBlockHistory subBlockHistory = _redoSubBlocks[idToCancel];
        _redoSubBlocks.RemoveAt(idToCancel);

        _currentBlockHistory.Add(subBlockHistory);
        _graphManager.Graph.RedoFilter(subBlockHistory);

        StyleChange styleChange = StyleChange.All;
        _stylingManager.UpdateStyling(styleChange);
    }

    public void ResetFilters()
    {
        SPARQLAdditiveBuilder additiveBuilder = new();

        var graph = _graphManager.Graph;
        graph.UnhideNodges();

        _blocksHistory = new();
        _currentBlockHistory = new();
        _graphManager.UpdateGraphFromHistoryFilter(additiveBuilder);
    }


    public SPARQLAdditiveBuilder ApplyFilters()
    {
        SPARQLAdditiveBuilder additiveBuilder = new();

        foreach (BlockHistory blockHistory in _blocksHistory)
        {
            blockHistory.AddQueries(additiveBuilder);
        }

        _currentBlockHistory.AddQueries(additiveBuilder);

        var graph = _graphManager.Graph;

        _blocksHistory.Add(_currentBlockHistory);
        graph.ReleaseNodges(_nodgePool);
        _currentBlockHistory = new();

        return additiveBuilder;
    }

    private SPARQLAdditiveBuilder GetSPARQLAdditiveBuilder()
    {
        SPARQLAdditiveBuilder additiveBuilder = new();

        foreach (BlockHistory blockHistory in _blocksHistory)
        {
            blockHistory.AddQueries(additiveBuilder);
        }


        return additiveBuilder;
    }
}