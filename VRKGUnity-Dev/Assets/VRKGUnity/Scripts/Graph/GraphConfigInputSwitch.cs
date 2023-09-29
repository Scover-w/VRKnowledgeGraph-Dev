using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allow to switch the <see cref="GraphConfigKey"/> of a <see cref="GraphConfigInputLink"/>. 
/// </summary>
[DefaultExecutionOrder(2)]
public class GraphConfigInputSwitch : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphConfigKey _switchGraphConfigKey;

    [SerializeField]
    List<GraphConfigSwitchItem> _inputsToSwitch;

    InputPropagatorManager _inputPropagatorManager;

    bool _boolValue;


    private void OnEnable()
    {
        Invoke(nameof(DelayedOnEnable), .5f);
    }

    private void OnDisable()
    {
        UnRegisterToGraphConfigManager();
    }

    private void DelayedOnEnable()
    {
        if (_inputPropagatorManager == null)
            _inputPropagatorManager = _referenceHolderSo.InputPropagatorManager;

        RefreshSwitchValue();
        RegisterToGraphConfigManager();
    }



    private void RefreshSwitchValue()
    {
        _boolValue = _inputPropagatorManager.GetValue<bool>(_switchGraphConfigKey);
        RefreshInputsKey();
    }

    private void RegisterToGraphConfigManager()
    {
        if (_inputPropagatorManager == null)
        {
            Debug.Log("_inputPropagatorManager is null on " + gameObject.name);
            return;
        }

        _inputPropagatorManager.Register<bool>(_switchGraphConfigKey, OnChangedFromManager);
    }

    private void UnRegisterToGraphConfigManager()
    {
        if (_inputPropagatorManager == null)
        {
            Debug.Log("_inputPropagatorManager is null on " + gameObject.name);
            return;
        }

        _inputPropagatorManager.UnRegister<bool>(_switchGraphConfigKey, OnChangedFromManager);
    }

    public void OnChangedFromManager<T>(T newValueFromManager)
    {
        switch (newValueFromManager)
        {
            case bool b:
                _boolValue = b;
                RefreshInputsKey();
                break;
        }
    }

    private void RefreshInputsKey()
    {
        if (_inputsToSwitch == null)
            return;

        foreach(GraphConfigSwitchItem item in _inputsToSwitch)
        {
            item.InputLink.SwitchConfigKey(_boolValue ? item.KeyForTrueValue : item.KeyForFalseValue);
        }
    }

    private void OnValidate()
    {
        var keyType = _switchGraphConfigKey.GetConfigValueType();

        if(keyType != GraphConfigValueType.Bool)
        {
            Debug.LogError(_switchGraphConfigKey + " is not a booleanvalue, select another configkey.");
            _switchGraphConfigKey = GraphConfigKey.GraphMode;
            return;
        }

        if (_inputsToSwitch == null)
            return;

        foreach (GraphConfigSwitchItem item in _inputsToSwitch)
        {
            var inputLink = item.InputLink;

            if (inputLink == null)
                continue;

            item.Name = inputLink.gameObject.name;
        }
    }


    [Serializable]
    public class GraphConfigSwitchItem
    {
        [HideInInspector]
        public string Name;
        public GraphConfigInputLink InputLink;
        public GraphConfigKey KeyForFalseValue;
        public GraphConfigKey KeyForTrueValue;
    }

}


