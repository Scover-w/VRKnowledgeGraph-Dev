using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgeSelectionManager _selectionManager;

    [SerializeField]
    DynamicFilterManager _dynFilterManager;

    GraphMode _graphMode = GraphMode.Desk;


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

    }

    [ContextMenu("Hide Unselected Node")]
    public void HideUnSelectedNode()
    {
        _dynFilterManager.HideUnSelectedNode();
    }

    [ContextMenu("Hide Propagated Node")]
    public void HidePropagatedNode()
    {
        _dynFilterManager.HidePropagatedNode();
    }

    [ContextMenu("Hide Unpropagated Node")]
    public void HideUnPropagatedNode()
    {
        _dynFilterManager.HideUnPropagatedNode();
    }

    [ContextMenu("Cancel Last Filter")]
    public void CancelLastFilter()
    {
        _dynFilterManager.CancelLastFilter();
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
}
