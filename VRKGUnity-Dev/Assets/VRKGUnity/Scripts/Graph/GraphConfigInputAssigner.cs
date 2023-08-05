using AIDEN.TactileUI;
using Codice.CM.SEIDInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wave.Essence.Events;

public class GraphConfigInputAssigner : MonoBehaviour
{
    [SerializeField]
    GraphConfigurationManager _graphManager;

    [SerializeField]
    GraphConfigurationKey _graphConfigurationKey;

    [SerializeField]
    MonoBehaviour _tactileUIScript;

    GraphConfigValueType _valueType;

    IValueUI<string> _iValueStringUI;
    IValueUI<float> _iValueFloatUI;
    IValueUI<bool> _iValueBoolUI;
    IValueUI<Color> _iValueColorUI;

    private void Start()
    {
        RetrieveInputType();
        RetrieveInterface();
    }

    private void OnEnable()
    {
        SetValueOnInput();
    }

    private void OnDisable()
    {

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
                var valueString = _graphManager.GetStringValue(_graphConfigurationKey);
                _iValueStringUI.Value = valueString;
                break;
            case GraphConfigValueType.Float:
                var valueFloat = _graphManager.GetFloatValue(_graphConfigurationKey);
                _iValueFloatUI.Value = valueFloat;
                break;
            case GraphConfigValueType.Bool:
                var valueBool = _graphManager.GetBoolValue(_graphConfigurationKey);
                _iValueBoolUI.Value = valueBool;
                break;
            case GraphConfigValueType.Color:
                var valueColor = _graphManager.GetColorValue(_graphConfigurationKey);
                _iValueColorUI.Value = valueColor;
                break;
        }

    }


    public void OnValueChanged(string newValueFromInput)
    {
        _graphManager.SetNewValue(_graphConfigurationKey, newValueFromInput);
    }

    public void OnValueChanged(float newValueFromInput)
    {
        _graphManager.SetNewValue(_graphConfigurationKey, newValueFromInput);
    }

    public void OnValueChanged(bool newValueFromInput)
    {
        _graphManager.SetNewValue(_graphConfigurationKey, newValueFromInput);
    }

    public void OnValueChanged(Color newValueFromInput)
    {
        _graphManager.SetNewValue(_graphConfigurationKey, newValueFromInput);
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

        bool isGoodType = _graphConfigurationKey.IsGoodType(genericType);

        if (!isGoodType)
        {
            _tactileUIScript = null;
            Debug.LogError("The script does not implement the good IValueUI generic type, wanted : " + genericType);
            return;
        }
    }

}


