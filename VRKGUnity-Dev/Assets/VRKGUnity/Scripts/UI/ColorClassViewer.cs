using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorClassViewer : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    List<Image> _imgs;

    InputPropagatorManager _inputPropagatorManager;

    int _nbColor;
    float _saturation;
    float _brightness;

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
        if (_inputPropagatorManager == null)
            _inputPropagatorManager = _referenceHolderSo.InputPropagatorManager;

        if (_inputPropagatorManager == null)
            Debug.Log("_inputPropagatorManager is null");

        RetrieveValues();
        RegisterToGraphConfigManager();
        RefreshColors();
    }

    private void RetrieveValues()
    {
        _nbColor = (int)_inputPropagatorManager.GetValue<float>(GraphConfigKey.NbOntologyColor);
        _saturation = _inputPropagatorManager.GetValue<float>(GraphConfigKey.SaturationOntologyColor);
        _brightness = _inputPropagatorManager.GetValue<float>(GraphConfigKey.LuminosityOntologyColor);
    }

    private void RegisterToGraphConfigManager()
    {
        if (_inputPropagatorManager == null)
        {
            Debug.Log("_inputPropagatorManager is null on " + gameObject.name);
            return;
        }

        _inputPropagatorManager.Register<float>(GraphConfigKey.NbOntologyColor, OnChangedNbColorFromManager);
        _inputPropagatorManager.Register<float>(GraphConfigKey.SaturationOntologyColor, OnChangedSaturationFromManager);
        _inputPropagatorManager.Register<float>(GraphConfigKey.LuminosityOntologyColor, OnChangedLuminosityFromManager);
    }

    private void UnRegisterToGraphConfigManager()
    {
        if (_inputPropagatorManager == null)
        {
            Debug.Log("_inputPropagatorManager is null on " + gameObject.name);
            return;
        }

        _inputPropagatorManager.UnRegister<float>(GraphConfigKey.NbOntologyColor, OnChangedNbColorFromManager);
        _inputPropagatorManager.UnRegister<float>(GraphConfigKey.SaturationOntologyColor, OnChangedSaturationFromManager);
        _inputPropagatorManager.UnRegister<float>(GraphConfigKey.LuminosityOntologyColor, OnChangedLuminosityFromManager);
    }

    public void OnChangedNbColorFromManager<T>(T newValueFromManager)
    {
        Debug.Log("OnChangedNbColorFromManager");
        switch (newValueFromManager)
        {
            case float f:
                _nbColor = (int)f;
                RefreshColors();
                break;
        }
    }

    public void OnChangedSaturationFromManager<T>(T newValueFromManager)
    {
        switch (newValueFromManager)
        {
            case float f:
                _saturation = f;
                RefreshColors();
                break;
        }
    }

    public void OnChangedLuminosityFromManager<T>(T newValueFromManager)
    {
        switch (newValueFromManager)
        {
            case float f:
                _brightness = f;
                RefreshColors();
                break;
        }
    }
    private void RefreshColors()
    {
        float deltaHue = 1f / _nbColor;

        for (int i = 0; i < _nbColor; i++)
        {
            _imgs[i].color = (Color.HSVToRGB((deltaHue * i) % 1f, _saturation, _brightness));
        }

        for (int i = _nbColor; i < _imgs.Count; i++)
        {
            _imgs[i].color = Color.clear;
        }

    }
}
