using AIDEN.TactileUI;
using System;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class GraphConfigInputLink : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;


    [SerializeField]
    GraphConfigKey _graphConfigKey;

    [SerializeField]
    MonoBehaviour _tactileUIScript;

    [SerializeField]
    bool _updateInteractableState = false;

    InputPropagatorManager _inputPropagatorManager;
    GraphConfigValueType _valueType;

    IValueUI<string> _iValueStringUI;
    IValueUI<float> _iValueFloatUI;
    IValueUI<bool> _iValueBoolUI;
    IValueUI<Color> _iValueColorUI;

    ITouchUI _iTouchUI;

    float _debounceTime;
    float _debounceDelay = 0.25f;

    #region LifeCycle
    private void Awake()
    {
        RetrieveInputType();
        RetrieveInterfaces();
    }

    private void OnEnable()
    {
        Invoke(nameof(DelayedOnEnable), .25f);
    }

    private void OnDisable()
    {
        UnRegisterToGraphConfigManager();
    }

    private void DelayedOnEnable()
    {
        if(_inputPropagatorManager == null)
            _inputPropagatorManager = _referenceHolderSo.InputPropagatorManager;

        if (_inputPropagatorManager == null)
            Debug.Log("_inputPropagatorManager is null");

        RefreshValueOnInput();

        if (_updateInteractableState)
            RefreshInteractableStateOnInput();

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

        if (genericType == typeof(string))
            _valueType = GraphConfigValueType.String;
        else if (genericType == typeof(float))
            _valueType = GraphConfigValueType.Float;
        else if (genericType == typeof(bool))
            _valueType = GraphConfigValueType.Bool;
        else if (genericType == typeof(Color))
            _valueType = GraphConfigValueType.Color;
    }

    private void RetrieveInterfaces()
    {
        if(_tactileUIScript == null)
        {
            Debug.LogWarning("_tactileUIScript is null on " + gameObject.name);
            return;
        }

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

        _iTouchUI = _tactileUIScript.GetComponent<ITouchUI>();
    }

    private void RefreshValueOnInput()
    {

        switch (_valueType)
        {
            case GraphConfigValueType.String:
                string valueString = _inputPropagatorManager.GetValue<string>(_graphConfigKey);
                _iValueStringUI.Value = valueString;
                break;
            case GraphConfigValueType.Float:
                var valueFloat = _inputPropagatorManager.GetValue<float>(_graphConfigKey);
                _iValueFloatUI.Value = valueFloat;
                break;
            case GraphConfigValueType.Bool:
                var valueBool = _inputPropagatorManager.GetValue<bool>(_graphConfigKey);
                _iValueBoolUI.Value = valueBool;
                break;
            case GraphConfigValueType.Color:
                var valueColor = _inputPropagatorManager.GetValue<Color>(_graphConfigKey);
                _iValueColorUI.Value = valueColor;
                break;
        }

    }

    private void RefreshInteractableStateOnInput()
    {
        bool isInteractable = _inputPropagatorManager.GetInteractableState(_graphConfigKey);
        _iTouchUI.Interactable = isInteractable;
    }

    private void RegisterToGraphConfigManager()
    {
        if(_inputPropagatorManager == null)
        {
            Debug.Log("_inputPropagatorManager is null on " + gameObject.name);
            return;
        }

        switch (_valueType)
        {
            case GraphConfigValueType.String:
                _inputPropagatorManager.Register<string>(_graphConfigKey, OnChangedFromManager, _updateInteractableState? OnInteractableStateChanged : null);
                break;
            case GraphConfigValueType.Float:
                _inputPropagatorManager.Register<float>(_graphConfigKey, OnChangedFromManager, _updateInteractableState ? OnInteractableStateChanged : null);
                break;
            case GraphConfigValueType.Bool:
                _inputPropagatorManager.Register<bool>(_graphConfigKey, OnChangedFromManager, _updateInteractableState ? OnInteractableStateChanged : null);
                break;
            case GraphConfigValueType.Color:
                _inputPropagatorManager.Register<Color>(_graphConfigKey, OnChangedFromManager, _updateInteractableState ? OnInteractableStateChanged : null);
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
            case GraphConfigValueType.String:
                _inputPropagatorManager.UnRegister<string>(_graphConfigKey, OnChangedFromManager, _updateInteractableState ? OnInteractableStateChanged : null);
                break;
            case GraphConfigValueType.Float:
                _inputPropagatorManager.UnRegister<float>(_graphConfigKey, OnChangedFromManager, _updateInteractableState ? OnInteractableStateChanged : null);
                break;
            case GraphConfigValueType.Bool:
                _inputPropagatorManager.UnRegister<bool>(_graphConfigKey, OnChangedFromManager, _updateInteractableState ? OnInteractableStateChanged : null);
                break;
            case GraphConfigValueType.Color:
                _inputPropagatorManager.UnRegister<Color>(_graphConfigKey, OnChangedFromManager, _updateInteractableState ? OnInteractableStateChanged : null);
                break;
        }
    }
    #endregion


    public void OnInteractableStateChanged(bool isInteractable)
    {
        _iTouchUI.Interactable = isInteractable;
    }

    #region OnChangeFrom

    #region OnChangedFromInput
    public void OnChangedFromInput<T>(T newValueFromInput)
    {
        _debounceTime = Time.unscaledTime + _debounceDelay;
        _inputPropagatorManager.SetNewValue(_graphConfigKey, newValueFromInput);
    }

    public void OnStringChangedFromInput(string newValueFromInput) => OnChangedFromInput<string>(newValueFromInput);
    public void OnFloatChangedFromInput(float newValueFromInput) => OnChangedFromInput<float>(newValueFromInput);
    public void OnBoolChangedFromInput(bool newValueFromInput) => OnChangedFromInput<bool>(newValueFromInput);
    public void OnColorChangedFromInput(Color newValueFromInput) => OnChangedFromInput<Color>(newValueFromInput);
    #endregion


    public void OnChangedFromManager<T>(T newValueFromManager)
    {
        if (Time.unscaledTime < _debounceTime)
            return;

        switch (newValueFromManager)
        {
            case string s:
                _iValueStringUI.Value = s;
                break;
            case float f:
                _iValueFloatUI.Value = f;
                break;
            case bool b:
                _iValueBoolUI.Value = b;
                break;
            case Color c:
                _iValueColorUI.Value = c;
                break;
        } 
    }
    #endregion


    #region OnValidate
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_tactileUIScript == null)
            return;


        _iTouchUI = _tactileUIScript.GetComponent<ITouchUI>();
        if(_iTouchUI == null)
        {
            _tactileUIScript = null;
            Debug.LogError("The script does not implement ITouchUI on " + gameObject.name);
            return;
        }


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
            Debug.LogError("The script does not implement IValueUI on " + gameObject.name);
            return;
        }

        bool isGoodType = _graphConfigKey.IsGoodType(genericType);

        if (!isGoodType)
        {
            _tactileUIScript = null;
            Debug.LogError("The script does not implement the good IValueUI generic type, wanted : " + genericType + " on " + gameObject.name);
            return;
        }
    }
#endif
    #endregion
}


