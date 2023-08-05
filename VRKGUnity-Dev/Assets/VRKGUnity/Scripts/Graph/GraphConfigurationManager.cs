using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphConfigurationManager : MonoBehaviour
{
    [SerializeField]
    StylingManager _stylingManager;

    GraphConfiguration _graphConfiguration;

    //Dictionary<GraphConfigurationKey, GraphConfigurationEvent> _


    private void Start()
    {
        _graphConfiguration = GraphConfiguration.Instance;
    }

    public void SetNewValue(GraphConfigurationKey key, string newValue)
    {
        if (!_graphConfiguration.SetValue(key, newValue))
            return;

        var styleChange = StyleChangeBuilder.Get(key);
        _stylingManager.UpdateStyling(styleChange);

    }

    public void SetNewValue(GraphConfigurationKey key, float newValue)
    {
        if (!_graphConfiguration.SetValue(key, newValue))
            return;

        var styleChange = StyleChangeBuilder.Get(key);
        _stylingManager.UpdateStyling(styleChange);
    }

    public void SetNewValue(GraphConfigurationKey key, bool newValue)
    {
        if (!_graphConfiguration.SetValue(key, newValue))
            return;

        var styleChange = StyleChangeBuilder.Get(key);
        _stylingManager.UpdateStyling(styleChange);
    }

    public void SetNewValue(GraphConfigurationKey key, Color newValue)
    {
        if (!_graphConfiguration.SetValue(key, newValue))
            return;

        var styleChange = StyleChangeBuilder.Get(key);
        _stylingManager.UpdateStyling(styleChange);
    }

    public string GetStringValue(GraphConfigurationKey key)
    {
        return key.GetStringValue(_graphConfiguration);
    }

    public float GetFloatValue(GraphConfigurationKey key)
    {
        return key.GetFloatValue(_graphConfiguration);
    }

    public bool GetBoolValue(GraphConfigurationKey key)
    {
        return key.GetBoolValue(_graphConfiguration);
    }

    public Color GetColorValue(GraphConfigurationKey key)
    {
        return key.GetColorValue(_graphConfiguration);
    }



    private class GraphConfigurationEvent
    {

        //public ValueStringChanged  

        //public GraphConfigurationEvent(GraphConfigValueType type)
        //{

        //}

    }

}


public enum GraphConfigValueType
{
    String,
    Float,
    Bool,
    Color
}

public delegate void ValueStringChanged(string value);
public delegate void ValueFloatChanged(float value);
public delegate void ValueBoolChanged(bool value);
public delegate void ValueColorChanged(Color value);