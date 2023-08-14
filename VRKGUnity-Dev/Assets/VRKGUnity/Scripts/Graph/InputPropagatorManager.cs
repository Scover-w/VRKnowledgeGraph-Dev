using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


public partial class InputPropagatorManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    DynamicFilterManager _dynFilterManager;

    [SerializeField]
    StylingManager _stylingManager;

    [SerializeField]
    NodgeSelectionManager _selectionManager;

    [SerializeField]
    User _user;

    GraphConfiguration _graphConfiguration;

    public delegate void ChangeActionBtnState(bool isInteractable);

    Dictionary<GraphConfigKey, GraphConfigEvent> _parametersEvents;
    Dictionary<GraphActionKey, ChangeActionBtnState> _actionsEvents;


    private async void Awake()
    {
        _actionsEvents = new();
        _parametersEvents = new();
        _referenceHolderSO.InputPropagatorManager = this;

        await GraphConfiguration.Load();
        _graphConfiguration = GraphConfiguration.Instance;
    }

    #region OnEnableUI
    public T GetValue<T>(GraphConfigKey key)
    {
        return key.GetValue<T>(_graphConfiguration);
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

    public void Register<T>(GraphConfigKey key, ValueChanged<T> valueChanged)
    {
        if (!_parametersEvents.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            graphConfigEvent = new GraphConfigEvent();
            _parametersEvents.Add(key, graphConfigEvent);
        }

        graphConfigEvent.Register(valueChanged);
    }
    public void UnRegister<T>(GraphConfigKey key, ValueChanged<T> valueChanged)
    {
        if (!_parametersEvents.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            Debug.LogWarning("GraphEvent didn't exist to unregister Delegate");
            return;
        }

        if (graphConfigEvent.UnRegister(valueChanged))
            return;

        _parametersEvents.Remove(key);
    }

    public void Register(GraphActionKey actionKey, ChangeActionBtnState changeActionBtnToAdd)
    {
        if (!_actionsEvents.TryGetValue(actionKey, out ChangeActionBtnState changeActionBtnState))
        {
            _actionsEvents.Add(actionKey, changeActionBtnState);
        }

        changeActionBtnState += changeActionBtnToAdd;
    }

    public void UnRegister(GraphActionKey actionKey, ChangeActionBtnState changeActionBtnToRemove)
    {
        if (!_actionsEvents.TryGetValue(actionKey, out ChangeActionBtnState changeActionBtnState))
        {
            Debug.LogWarning("GraphAction didn't exist to unregister Delegate");
            return;
        }

        changeActionBtnState -= changeActionBtnToRemove;

        if (changeActionBtnState.GetInvocationList().Length > 0)
            return;

        _actionsEvents.Remove(actionKey);
    }

    #endregion


    #region ChangeFromUI
    public void SetNewValue<T>(GraphConfigKey key, T newValue)
    {
        if (!_graphConfiguration.TrySetValue(key, newValue))
            return;

        UpdateStyling(key);
        TryInvoke(key, newValue);
    }

    public void InitiateNewAction(GraphActionKey actionKey)
    {
        switch (actionKey)
        {
            case GraphActionKey.FilterSelected:
                _user.HideSelectedNode();
                break;
            case GraphActionKey.FilterUnselected:
                _user.HideUnSelectedNode();
                break;
            case GraphActionKey.FilterPropagated:
                _user.HidePropagatedNode();
                break;
            case GraphActionKey.FilterUnpropagated:
                _user.HideUnPropagatedNode();
                break;
            case GraphActionKey.UndoFilter:
                _user.UndoLastFilter();
                break;
            case GraphActionKey.RedoFilter:
                _user.RedoLastFilter();
                break;
            case GraphActionKey.Simulate:
                _user.ResimulateGraph();
                break;
        }
    }

    public void TryInvoke<T>(GraphConfigKey key, T newValue)
    {
        if (!_parametersEvents.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
            return;

        graphConfigEvent.Invoke(newValue);
    }

    public void TryInvoke(GraphActionKey actionKey, bool isInteractable)
    {
        if (!_actionsEvents.TryGetValue(actionKey, out ChangeActionBtnState actionBtnState))
            return;

        actionBtnState.Invoke(isInteractable);
    }
    #endregion

    private void UpdateStyling(GraphConfigKey key)
    {
        var styleChange = StyleChangeBuilder.Get(key);
        _stylingManager.UpdateStyling(styleChange);
    }



    
}

public delegate void ValueChanged<T>(T value);