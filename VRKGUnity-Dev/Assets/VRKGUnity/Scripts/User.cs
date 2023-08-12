using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgeSelectionManager _selectionManager;

    [SerializeField]
    DynamicFilterManager _dynFilterManager;


    public delegate void ChangeActionBtnState(bool isInteractable);

    GraphMode _graphMode = GraphMode.Desk;

    Dictionary<GraphActionKey, ChangeActionBtnState> _events;

    private void Awake()
    {
        _events = new();
        _referenceHolderSO.User = this;
    }

    public bool GetInteractableState(GraphActionKey actionKey)
    {
        switch (actionKey)
        {
            case GraphActionKey.FilterSelected:
                return true;
            case GraphActionKey.FilterUnselected:
                return true;
            case GraphActionKey.FilterPropagated:
                return true;
            case GraphActionKey.FilterUnpropagated:
                return true;
            case GraphActionKey.UndoFilter:
                return _dynFilterManager.NbFilter != 0;
            case GraphActionKey.RedoFilter:
                return _dynFilterManager.NbRedoFilter != 0;
            case GraphActionKey.Simulate:
                return true;
            case GraphActionKey.SwitchMode:
                return true;
            case GraphActionKey.SelectionMode:
                return true;
        }

        return true;
    }

    public void InitiateNewAction(GraphActionKey actionKey)
    {
        switch (actionKey)
        {
            case GraphActionKey.FilterSelected:
                HideSelectedNode();
                break;
            case GraphActionKey.FilterUnselected:
                HideUnSelectedNode();
                break;
            case GraphActionKey.FilterPropagated:
                HidePropagatedNode();
                break;
            case GraphActionKey.FilterUnpropagated:
                HideUnPropagatedNode();
                break;
            case GraphActionKey.UndoFilter:
                UndoLastFilter();
                break;
            case GraphActionKey.RedoFilter:
                RedoLastFilter();
                break;
            case GraphActionKey.Simulate:
                ResimulateGraph();
                break;
            case GraphActionKey.SwitchMode:
                SwitchGraphMode();
                break;
            case GraphActionKey.SelectionMode:
                SwitchSelectionMode();
                break;
        }
    }

    public void Register(GraphActionKey actionKey, ChangeActionBtnState changeActionBtnToAdd)
    {
        if (!_events.TryGetValue(actionKey, out ChangeActionBtnState changeActionBtnState))
        {
            _events.Add(actionKey, changeActionBtnState);
        }

        changeActionBtnState += changeActionBtnToAdd;
    }

    public void UnRegister(GraphActionKey actionKey, ChangeActionBtnState changeActionBtnToRemove)
    {
        if (!_events.TryGetValue(actionKey, out ChangeActionBtnState changeActionBtnState))
        {
            Debug.LogWarning("GraphAction didn't exist to unregister Delegate");
            return;
        }

        changeActionBtnState -= changeActionBtnToRemove;

        if (changeActionBtnState.GetInvocationList().Length > 0)
            return;

        _events.Remove(actionKey);
    }

    private void TryInvoke(GraphActionKey actionKey, bool isInteractable)
    {
        if (!_events.TryGetValue(actionKey, out ChangeActionBtnState actionBtnState))
            return;

        actionBtnState.Invoke(isInteractable);
    }


    [ContextMenu("Switch Graph Mode")]
    public void SwitchGraphMode()
    {
        if (_graphManager.IsRunningSimulation)
        {
            Debug.Log("Can't switch Mode when running simulation");
            // TODO : Notification can't switch mode in simulation
            return;
        }

        if(_graphMode == GraphMode.Desk)
        {
            Debug.Log("Switch to Immersion Mode");
            _graphMode = GraphMode.Immersion;
            _graphManager.TrySwitchModeToImmersion();
        }
        else
        {
            Debug.Log("Switch to Desk Mode");
            _graphMode = GraphMode.Desk;
            _graphManager.TrySwitchModeToDesk();
        }
    }

    [ContextMenu("Switch Selection Mode")]
    public void SwitchSelectionMode()
    {
        _selectionManager.SwitchSelectionMode();
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
        TryInvoke(GraphActionKey.UndoFilter, _dynFilterManager.NbFilter != 0);
        TryInvoke(GraphActionKey.RedoFilter, _dynFilterManager.NbRedoFilter != 0);
    }
}
