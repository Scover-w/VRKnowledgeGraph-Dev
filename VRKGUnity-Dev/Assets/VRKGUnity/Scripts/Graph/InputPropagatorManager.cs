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


    private void Awake()
    {
        _actionsEvents = new();
        _parametersEvents = new();
        _referenceHolderSO.InputPropagatorManager = this;
    }

    private async void Start()
    {
        _graphManager.OnGraphUpdate += OnGraphUpdated;

        await GraphConfiguration.Load();
        _graphConfiguration = GraphConfiguration.Instance;
    }


    #region OnEnableUI
    public T GetValue<T>(GraphConfigKey key)
    {
        return key.GetValue<T>(_graphConfiguration);
    }

    public bool GetInteractableState(GraphConfigKey configKey)
    {
        if (configKey == GraphConfigKey.GraphMode)
            return _graphManager.CanSwitchMode();


        return true;
    }

    public bool GetInteractableState(GraphActionKey actionKey)
    {
        return actionKey switch
        {
            GraphActionKey.FilterSelected => true,
            GraphActionKey.FilterUnselected => true,
            GraphActionKey.FilterPropagated => true,
            GraphActionKey.FilterUnpropagated => true,
            GraphActionKey.UndoFilter => _dynFilterManager.NbFilter != 0,
            GraphActionKey.RedoFilter => _dynFilterManager.NbRedoFilter != 0,
            GraphActionKey.Simulate => !_graphManager.IsRunningSimulation,
            GraphActionKey.SelectionMode => true,
            _ => true,
        };
    }

    public void Register<T>(GraphConfigKey key, ValueChanged<T> valueChanged, InteractableStateChanged interactableStateChanged = null)
    {
        if (!_parametersEvents.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            graphConfigEvent = new GraphConfigEvent();
            _parametersEvents.Add(key, graphConfigEvent);
        }

        graphConfigEvent.Register(valueChanged, interactableStateChanged);
    }
    public void UnRegister<T>(GraphConfigKey key, ValueChanged<T> valueChanged, InteractableStateChanged interactableStateChanged = null)
    {
        if (!_parametersEvents.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            Debug.LogWarning("GraphEvent didn't exist to unregister Delegate");
            return;
        }

        if (graphConfigEvent.UnRegister(valueChanged, interactableStateChanged))
            return;

        _parametersEvents.Remove(key);
    }

    public void Register(GraphActionKey actionKey, ChangeActionBtnState changeActionBtnToAdd)
    {
        Debug.Log("Register : " + actionKey);

        if (!_actionsEvents.TryGetValue(actionKey, out ChangeActionBtnState changeActionBtnState))
        {
            _actionsEvents.Add(actionKey, changeActionBtnState);
        }

        changeActionBtnState += changeActionBtnToAdd;
        _actionsEvents[actionKey] = changeActionBtnState;
    }

    public void UnRegister(GraphActionKey actionKey, ChangeActionBtnState changeActionBtnToRemove)
    {

        if (!_actionsEvents.TryGetValue(actionKey, out ChangeActionBtnState changeActionBtnState))
        {
            Debug.LogWarning("GraphAction didn't exist to unregister Delegate");
            return;
        }

        changeActionBtnState -= changeActionBtnToRemove;

        if (changeActionBtnState != null)
        {
            _actionsEvents[actionKey] = changeActionBtnState;
            return;
        }

        _actionsEvents.Remove(actionKey);
    }

    #endregion


    #region ChangeFromUI
    public void SetNewValue<T>(GraphConfigKey key, T newValue)
    {
        if (TryProcessSpecialConfigKey(key, newValue))
            return;

        if (!_graphConfiguration.TrySetValue(key, newValue))
            return;

        UpdateStyling(key);
        InvokeValueChanged(key, newValue);
    }

    private bool TryProcessSpecialConfigKey<T>(GraphConfigKey key, T newValue)
    {
        if(key == GraphConfigKey.GraphMode)
        {
            if (!_user.SwitchGraphMode())
                return true;

            if (!_graphConfiguration.TrySetValue(key, newValue))
                return true;

            InvokeValueChanged(key, newValue);
            TryInvokeInteractableStateChanged(key, _graphManager.CanSwitchMode());
            return true;
        }

        return false;
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

    public void InvokeValueChanged<T>(GraphConfigKey key, T newValue)
    {
        if (!_parametersEvents.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
            return;

        graphConfigEvent.InvokeValueChanged(newValue);
    }

    public void TryInvokeInteractableStateChanged(GraphConfigKey key, bool isInteractable)
    {
        if (!_parametersEvents.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
            return;

        graphConfigEvent.InvokeInteractableStateChanged(isInteractable);
    }

    public void TryInvoke(GraphActionKey actionKey, bool isInteractable)
    {
        if (!_actionsEvents.TryGetValue(actionKey, out ChangeActionBtnState actionBtnState))
            return;

        actionBtnState?.Invoke(isInteractable);
    }
    #endregion

    private void UpdateStyling(GraphConfigKey key)
    {
        var styleChange = StyleChangeBuilder.Get(key);
        _stylingManager.UpdateStyling(styleChange);
    }


    public void OnGraphUpdated(GraphUpdateType updateType)
    {
       
        switch (updateType)
        {
            case GraphUpdateType.RetrievingFromDb:
                TryInvokeInteractableStateChanged(GraphConfigKey.GraphMode, false);
                TryInvoke(GraphActionKey.Simulate, false);
                break;
            case GraphUpdateType.BeforeSimulationStart:
                TryInvokeInteractableStateChanged(GraphConfigKey.GraphMode, false);
                TryInvoke(GraphActionKey.Simulate, false);
                break;
            case GraphUpdateType.AfterSimulationHasStopped:
                TryInvokeInteractableStateChanged(GraphConfigKey.GraphMode, true);
                TryInvoke(GraphActionKey.Simulate, true);
                break;
            case GraphUpdateType.BeforeSwitchMode:
                TryInvokeInteractableStateChanged(GraphConfigKey.GraphMode, false);
                TryInvoke(GraphActionKey.Simulate, false);
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                TryInvokeInteractableStateChanged(GraphConfigKey.GraphMode, true);
                TryInvoke(GraphActionKey.Simulate, true);
                break;
            case GraphUpdateType.AfterSwitchModeToImmersion:
                TryInvokeInteractableStateChanged(GraphConfigKey.GraphMode, true);
                TryInvoke(GraphActionKey.Simulate, true);
                break;
        } 
    }
}

public delegate void ValueChanged<T>(T value);
public delegate void InteractableStateChanged(bool isInteractable);