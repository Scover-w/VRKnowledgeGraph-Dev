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
        _user.SwitchGraphMode();
    }

    [ContextMenu("Hide Selected Node")]
    public void HideSelectedNode()
    {
        _user.SwitchGraphMode();
    }

    [ContextMenu("Hide Unselected Node")]
    public void HideUnSelectedNode()
    {
        _user.SwitchGraphMode();
    }

    [ContextMenu("Hide Propagated Node")]
    public void HidePropagatedNode()
    {
        _user.SwitchGraphMode();
    }

    [ContextMenu("Hide Unpropagated Node")]
    public void HideUnPropagatedNode()
    {
        _user.SwitchGraphMode();
    }

    [ContextMenu("Cancel Last Filter")]
    public void CancelLastFilter()
    {
        _user.SwitchGraphMode();
    }

    [ContextMenu("Resimulate Graph")]
    public void ResimulateGraph()
    {
        _user.SwitchGraphMode();
    }

    [ContextMenu("Reset All")]
    public void ResetAll()
    {
        _user.SwitchGraphMode();
    }

}
