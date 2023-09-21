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
    DynamicFilterManager _dynFilterManager;

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
        _dynFilterManager.HideSelectedNode();
        UpdateHistoryBtn();
    }

    [ContextMenu("Hide Unselected Node")]
    public void HideUnSelectedNode()
    {
        _dynFilterManager.HideUnSelectedNode();
        UpdateHistoryBtn();
    }

    [ContextMenu("Hide Propagated Node")]
    public void HidePropagatedNode()
    {
        _dynFilterManager.HidePropagatedNode();
        UpdateHistoryBtn();
    }

    [ContextMenu("Hide Unpropagated Node")]
    public void HideUnPropagatedNode()
    {
        _dynFilterManager.HideUnPropagatedNode();
        UpdateHistoryBtn();
    }

    [ContextMenu("Undo Last Filter")]
    public void UndoLastFilter()
    {
        _dynFilterManager.UndoLastFilter();
        UpdateHistoryBtn();
    }

    [ContextMenu("Redo Last Filter")]
    public void RedoLastFilter()
    {
        _dynFilterManager.RedoLastFilter();
        UpdateHistoryBtn();
    }


    [ContextMenu("Resimulate Graph")]
    public void ResimulateGraph()
    {
        _graphManager.UpdateGraph();
    }

    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        _graphManager.ResetAll();
    }

    private void UpdateHistoryBtn()
    {
        _inputPropagatorManager.TryInvoke(GraphActionKey.UndoFilter, _dynFilterManager.NbFilter != 0);
        _inputPropagatorManager.TryInvoke(GraphActionKey.RedoFilter, _dynFilterManager.NbRedoFilter != 0);
    }
}
