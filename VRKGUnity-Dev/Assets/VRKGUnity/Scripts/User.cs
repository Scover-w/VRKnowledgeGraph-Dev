using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    InputPropagatorManager _inputPropagatorManager;

    [SerializeField]
    NodgeSelectionManager _selectionManager;

    [SerializeField]
    HistoryFilterManager _historyFilterManager;

    [ContextMenu("Switch Graph Mode")]
    public bool SwitchGraphMode()
    {
        if(_graphManager.GraphMode == GraphMode.Desk)
            return _graphManager.TrySwitchModeToImmersion();

        return _graphManager.TrySwitchModeToDesk();
    }

    [ContextMenu("Hide Selected Node")]
    public void HideSelectedNode()
    {
        _historyFilterManager.HideSelectedNode();
        UpdateHistoryBtn();

        if(_graphManager.GraphConfiguration.RecalculateMetricsOnFilter)
            _graphManager.RecalculateMetrics();
    }

    [ContextMenu("Hide Unselected Node")]
    public void HideUnSelectedNode()
    {
        _historyFilterManager.HideUnSelectedNode();
        UpdateHistoryBtn();

        if (_graphManager.GraphConfiguration.RecalculateMetricsOnFilter)
            _graphManager.RecalculateMetrics();
    }

    [ContextMenu("Hide Propagated Node")]
    public void HidePropagatedNode()
    {
        _historyFilterManager.HidePropagatedNode();
        UpdateHistoryBtn();

        if (_graphManager.GraphConfiguration.RecalculateMetricsOnFilter)
            _graphManager.RecalculateMetrics();
    }

    [ContextMenu("Hide Unpropagated Node")]
    public void HideUnPropagatedNode()
    {
        _historyFilterManager.HideUnPropagatedNode();
        UpdateHistoryBtn();

        if (_graphManager.GraphConfiguration.RecalculateMetricsOnFilter)
            _graphManager.RecalculateMetrics();
    }

    [ContextMenu("Undo Last Filter")]
    public void UndoLastFilter()
    {
        _historyFilterManager.UndoLastFilter();
        UpdateHistoryBtn();

        if (_graphManager.GraphConfiguration.RecalculateMetricsOnFilter)
            _graphManager.RecalculateMetrics();
    }

    [ContextMenu("Redo Last Filter")]
    public void RedoLastFilter()
    {
        _historyFilterManager.RedoLastFilter();
        UpdateHistoryBtn();

        if (_graphManager.GraphConfiguration.RecalculateMetricsOnFilter)
            _graphManager.RecalculateMetrics();
    }

    [ContextMenu("Reset Filters")]
    public void ResetFilters()
    {
        _historyFilterManager.ResetFilters();
        UpdateHistoryBtn();
    }

    [ContextMenu("Recalculate Metrics")]
    public void RecalculateMetrics()
    {
        _graphManager.RecalculateMetrics();
    }


    [ContextMenu("Resimulate Graph")]
    public void ResimulateGraph()
    {
        _graphManager.ResimulateGraph();
    }

    private void UpdateHistoryBtn()
    {
        _inputPropagatorManager.TryInvoke(GraphActionKey.UndoFilter, _historyFilterManager.CanUndo);
        _inputPropagatorManager.TryInvoke(GraphActionKey.RedoFilter, _historyFilterManager.CanRedo);
    }
}
