using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphConfigInputSwitch : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphConfigKey _switchGraphConfigKey;

    [SerializeField]
    List<GraphConfigSwitchItem> _inputsToSwitch;

    InputPropagatorManager _inputPropagatorManager;
    GraphConfigValueType _valueType;

    int _intValue;
    float _floatValue;
    bool _boolValue;
    Color _colorValue;

    private void Awake()
    {
        _valueType = _switchGraphConfigKey.GetConfigValueType();
    }


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

        switch (_valueType)
        {
            case GraphConfigValueType.Int:
                _intValue = _inputPropagatorManager.GetValue<int>(_switchGraphConfigKey);
                break;
            case GraphConfigValueType.Float:
                _floatValue = _inputPropagatorManager.GetValue<float>(_switchGraphConfigKey);
                break;
            case GraphConfigValueType.Bool:
                _boolValue = _inputPropagatorManager.GetValue<bool>(_switchGraphConfigKey);
                break;
            case GraphConfigValueType.Color:
                _colorValue = _inputPropagatorManager.GetValue<Color>(_switchGraphConfigKey);
                break;
        }
    }

    private void RegisterToGraphConfigManager()
    {
        if (_inputPropagatorManager == null)
        {
            Debug.Log("_inputPropagatorManager is null on " + gameObject.name);
            return;
        }

        switch (_valueType)
        {
            case GraphConfigValueType.Int:
                _inputPropagatorManager.Register<int>(_switchGraphConfigKey, OnChangedFromManager);
                break;
            case GraphConfigValueType.Float:
                _inputPropagatorManager.Register<float>(_switchGraphConfigKey, OnChangedFromManager);
                break;
            case GraphConfigValueType.Bool:
                _inputPropagatorManager.Register<bool>(_switchGraphConfigKey, OnChangedFromManager);
                break;
            case GraphConfigValueType.Color:
                _inputPropagatorManager.Register<Color>(_switchGraphConfigKey, OnChangedFromManager);
                break;
        }
    }

    private void UnRegisterToGraphConfigManager()
    {
        if (_inputPropagatorManager == null)
        {
            Debug.Log("_inputPropagatorManager is null on " + gameObject.name);
            return;
        }

        switch (_valueType)
        {
            case GraphConfigValueType.Int:
                _inputPropagatorManager.UnRegister<int>(_switchGraphConfigKey, OnChangedFromManager);
                break;
            case GraphConfigValueType.Float:
                _inputPropagatorManager.UnRegister<float>(_switchGraphConfigKey, OnChangedFromManager);
                break;
            case GraphConfigValueType.Bool:
                _inputPropagatorManager.UnRegister<bool>(_switchGraphConfigKey, OnChangedFromManager);
                break;
            case GraphConfigValueType.Color:
                _inputPropagatorManager.UnRegister<Color>(_switchGraphConfigKey, OnChangedFromManager);
                break;
        }
    }

    public void OnChangedFromManager<T>(T newValueFromManager)
    {
        switch (newValueFromManager)
        {
            case int i:
                _intValue = i;
                break;
            case float f:
                _floatValue = f;
                break;
            case bool b:
                _boolValue = b;
                break;
            case Color c:
                _colorValue = c;
                break;
        }
    }

    private void RefreshInputsKey()
    {

    }

    private void OnValidate()
    {
        
    }


    [Serializable]
    public class GraphConfigSwitchItem
    {
        public GraphConfigInputLink InputLink;
        public List<GraphConfigSwitchValue> Values;
    }

    [Serializable]
    public class GraphConfigSwitchValue
    {
        public GraphConfigValueType GraphConfigValueType;
        public int IntSwitchConfigKeyValue;
        public float FloatSwitchConfigKeyValue;
        public bool BoolSwitchConfigKeyValue;
        public Color ColorSwitchConfigKeyValue;
        public GraphConfigKey GraphConfigKeyToSet;
    }

}


