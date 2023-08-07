using AIDEN.TactileUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class GraphConfigInputLink : MonoBehaviour
{
    [SerializeField]
    GraphConfigManager _graphManager;

    [SerializeField]
    GraphConfigKey _graphConfigKey;

    [SerializeField]
    MonoBehaviour _tactileUIScript;

    GraphConfigValueType _valueType;

    IValueUI<string> _iValueStringUI;
    IValueUI<float> _iValueFloatUI;
    IValueUI<bool> _iValueBoolUI;
    IValueUI<Color> _iValueColorUI;

    float _debounceTime;
    float _debounceDelay = 0.25f;

    private void Awake()
    {
        RetrieveInputType();
        RetrieveInterface();
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
        SetValueOnInput();
        RegisterToGraphConfigManager();

        _debounceTime = Time.unscaledTime;
    }

    private void RetrieveInputType()
    {
        var interfaces = _tactileUIScript.GetType().GetInterfaces();
        Type genericType = null;

        foreach (Type interfaceType in interfaces)
        {
            if (!(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IValueUI<>)))
                continue;

            genericType = interfaceType.GetGenericArguments()[0];
            break;
        }

        if(genericType == typeof(string))
            _valueType = GraphConfigValueType.String;
        else if(genericType == typeof(float))
            _valueType = GraphConfigValueType.Float;
        else if (genericType == typeof(bool))
            _valueType = GraphConfigValueType.Bool;
        else if (genericType == typeof(Color))
            _valueType = GraphConfigValueType.Color;
    }

    private void RetrieveInterface()
    {
        switch (_valueType)
        {
            case GraphConfigValueType.String:
                _iValueStringUI = _tactileUIScript.GetComponent<IValueUI<string>>();
                break;
            case GraphConfigValueType.Float:
                _iValueFloatUI = _tactileUIScript.GetComponent<IValueUI<float>>();
                break;
            case GraphConfigValueType.Bool:
                _iValueBoolUI = _tactileUIScript.GetComponent<IValueUI<bool>>();
                break;
            case GraphConfigValueType.Color:
                _iValueColorUI = _tactileUIScript.GetComponent<IValueUI<Color>>();
                break;
        }
    }

    private void SetValueOnInput()
    {

        switch (_valueType)
        {
            case GraphConfigValueType.String:
                var valueString = _graphManager.GetStringValue(_graphConfigKey);
                _iValueStringUI.Value = valueString;
                break;
            case GraphConfigValueType.Float:
                var valueFloat = _graphManager.GetFloatValue(_graphConfigKey);
                _iValueFloatUI.Value = valueFloat;
                break;
            case GraphConfigValueType.Bool:
                var valueBool = _graphManager.GetBoolValue(_graphConfigKey);
                _iValueBoolUI.Value = valueBool;
                break;
            case GraphConfigValueType.Color:
                var valueColor = _graphManager.GetColorValue(_graphConfigKey);
                _iValueColorUI.Value = valueColor;
                break;
        }

    }

    private void RegisterToGraphConfigManager()
    {
        switch (_valueType)
        {
            case GraphConfigValueType.String:
                _graphManager.Register(_graphConfigKey, OnColorChangedFromManager);
                break;
            case GraphConfigValueType.Float:
                _graphManager.Register(_graphConfigKey, OnFloatChangedFromManager);
                break;
            case GraphConfigValueType.Bool:
                _graphManager.Register(_graphConfigKey, OnBoolChangedFromManager);
                break;
            case GraphConfigValueType.Color:
                _graphManager.Register(_graphConfigKey, OnColorChangedFromManager);
                break;
        }
    }

    private void UnRegisterToGraphConfigManager()
    {
        switch (_valueType)
        {
            case GraphConfigValueType.String:
                _graphManager.UnRegister(_graphConfigKey, OnColorChangedFromManager);
                break;
            case GraphConfigValueType.Float:
                _graphManager.UnRegister(_graphConfigKey, OnFloatChangedFromManager);
                break;
            case GraphConfigValueType.Bool:
                _graphManager.UnRegister(_graphConfigKey, OnBoolChangedFromManager);
                break;
            case GraphConfigValueType.Color:
                _graphManager.UnRegister(_graphConfigKey, OnColorChangedFromManager);
                break;
        }
    }


    public void OnStringChangedFromInput(string newValueFromInput)
    {
        _debounceTime = Time.unscaledTime + _debounceDelay;
        _graphManager.SetNewValue(_graphConfigKey, newValueFromInput);
    }

    public void OnFloatChangedFromInput(float newValueFromInput)
    {
        _debounceTime = Time.unscaledTime + _debounceDelay;
        _graphManager.SetNewValue(_graphConfigKey, newValueFromInput);
    }

    public void OnBoolChangedFromInput(bool newValueFromInput)
    {
        _debounceTime = Time.unscaledTime + _debounceDelay;
        _graphManager.SetNewValue(_graphConfigKey, newValueFromInput);
    }

    public void OnColorChangedFromInput(Color newValueFromInput)
    {
        _debounceTime = Time.unscaledTime + _debounceDelay;
        _graphManager.SetNewValue(_graphConfigKey, newValueFromInput);
    }

    public void OnStringChangedFromManager(string newValueFromManager)
    {
        if (Time.unscaledTime < _debounceTime)
            return;

        _iValueStringUI.Value = newValueFromManager;
    }

    public void OnFloatChangedFromManager(float newValueFromManager)
    {
        if (Time.unscaledTime < _debounceTime)
            return;

        _iValueFloatUI.Value = newValueFromManager;
    }

    public void OnBoolChangedFromManager(bool newValueFromManager)
    {
        if (Time.unscaledTime < _debounceTime)
            return;

        _iValueBoolUI.Value = newValueFromManager;
    }

    public void OnColorChangedFromManager(Color newValueFromManager)
    {
        if (Time.unscaledTime < _debounceTime)
            return;

        _iValueColorUI.Value = newValueFromManager;
    }


    private void OnValidate()
    {
        if (_tactileUIScript == null)
            return;

        bool implementsIValueUI = false;
        Type genericType = null;
        var interfaces = _tactileUIScript.GetType().GetInterfaces();

        foreach (Type interfaceType in interfaces)
        {
            if (!(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IValueUI<>)))
                continue;

            implementsIValueUI = true;
            genericType = interfaceType.GetGenericArguments()[0];
            break;
        }

        if (!implementsIValueUI)
        {
            _tactileUIScript = null;
            Debug.LogError("The script does not implement IValueUI !");
            return;
        }

        bool isGoodType = _graphConfigKey.IsGoodType(genericType);

        if (!isGoodType)
        {
            _tactileUIScript = null;
            Debug.LogError("The script does not implement the good IValueUI generic type, wanted : " + genericType);
            return;
        }
    }

}


