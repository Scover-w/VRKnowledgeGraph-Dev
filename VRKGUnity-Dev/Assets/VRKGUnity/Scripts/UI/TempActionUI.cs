using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempActionUI : MonoBehaviour
{
    [SerializeField]
    User _user;

    


    [ContextMenu("Switch Graph Mode")]
    public void SwitchGraphMode()
    {
        _user.SwitchGraphMode();
    }

    [ContextMenu("Switch Selection Mode")]
    public void SwitchSelectionMode()
    {
        _user.SwitchSelectionMode();
    }

    [ContextMenu("Hide Selected Node")]
    public void HideSelectedNode()
    {
        _user.HideSelectedNode();
    }

    [ContextMenu("Hide Unselected Node")]
    public void HideUnSelectedNode()
    {
        _user.HideUnSelectedNode();
    }

    [ContextMenu("Hide Propagated Node")]
    public void HidePropagatedNode()
    {
        _user.HidePropagatedNode();
    }

    [ContextMenu("Hide Unpropagated Node")]
    public void HideUnPropagatedNode()
    {
        _user.HideUnPropagatedNode();
    }

    [ContextMenu("Cancel Last Filter")]
    public void CancelLastFilter()
    {
        _user.UndoLastFilter();
    }

    [ContextMenu("Resimulate Graph")]
    public void ResimulateGraph()
    {
        _user.ResimulateGraph();
    }

    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        _user.ResetAll();
    }

}
