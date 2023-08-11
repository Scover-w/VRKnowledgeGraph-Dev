using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;


public partial class GraphConfigManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    StylingManager _stylingManager;

    GraphConfiguration _graphConfiguration;

    Dictionary<GraphConfigKey, GraphConfigEvent> _events;


    private async void Awake()
    {
        _events = new();
        _referenceHolderSO.GraphConfigManager = this;

        await GraphConfiguration.Load();
        _graphConfiguration = GraphConfiguration.Instance;
    }

    public void SetNewValue(GraphConfigKey key, string newValue)
    {
        if (!_graphConfiguration.SetValue(key, newValue))
            return;

        UpdateStyling(key);
        TryInvoke(key, newValue);
    }

    public void SetNewValue(GraphConfigKey key, float newValue)
    {
        if (!_graphConfiguration.SetValue(key, newValue))
            return;

        UpdateStyling(key);
        TryInvoke(key, newValue);
    }

    public void SetNewValue(GraphConfigKey key, bool newValue)
    {
        if (!_graphConfiguration.SetValue(key, newValue))
            return;

        UpdateStyling(key);
        TryInvoke(key, newValue);
    }

    public void SetNewValue(GraphConfigKey key, Color newValue)
    {
        if (!_graphConfiguration.SetValue(key, newValue))
            return;

        UpdateStyling(key);
        TryInvoke(key, newValue);
    }

    private void UpdateStyling(GraphConfigKey key)
    {
        var styleChange = StyleChangeBuilder.Get(key);
        _stylingManager.UpdateStyling(styleChange);
    }

    private void TryInvoke(GraphConfigKey key, string newValue)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
            return;

        graphConfigEvent.Invoke(newValue);
    }

    private void TryInvoke(GraphConfigKey key, float newValue)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
            return;

        graphConfigEvent.Invoke(newValue);
    }

    private void TryInvoke(GraphConfigKey key, bool newValue)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
            return;

        graphConfigEvent.Invoke(newValue);
    }

    private void TryInvoke(GraphConfigKey key, Color newValue)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
            return;

        graphConfigEvent.Invoke(newValue);
    }

    public string GetStringValue(GraphConfigKey key)
    {
        return key.GetStringValue(_graphConfiguration);
    }

    public float GetFloatValue(GraphConfigKey key)
    {
        return key.GetFloatValue(_graphConfiguration);
    }

    public bool GetBoolValue(GraphConfigKey key)
    {
        if(key == GraphConfigKey.SelectedMode)
        {
            return true;
        }


        return key.GetBoolValue(_graphConfiguration);
    }

    public Color GetColorValue(GraphConfigKey key)
    {
        return key.GetColorValue(_graphConfiguration);
    }

    public void Register(GraphConfigKey key, StringChanged stringChanged)
    {
        if(!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            graphConfigEvent = new GraphConfigEvent(key.GetConfigValueType());
            _events.Add(key, graphConfigEvent);
        }

        graphConfigEvent.Register(stringChanged);
    }

    public void Register(GraphConfigKey key, FloatChanged floatChanged)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            graphConfigEvent = new GraphConfigEvent(key.GetConfigValueType());
            _events.Add(key, graphConfigEvent);
        }

        graphConfigEvent.Register(floatChanged);
    }

    public void Register(GraphConfigKey key, BoolChanged boolChanged)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            graphConfigEvent = new GraphConfigEvent(key.GetConfigValueType());
            _events.Add(key, graphConfigEvent);
        }

        graphConfigEvent.Register(boolChanged);
    }

    public void Register(GraphConfigKey key, ColorChanged colorChanged)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            Debug.LogWarning("GraphEvent didn't exist to unregister Delegate");
            return;
        }

        graphConfigEvent.Register(colorChanged);
    }

    public void UnRegister(GraphConfigKey key, StringChanged stringChanged)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            Debug.LogWarning("GraphEvent didn't exist to unregister Delegate");
            return;
        }

        if (graphConfigEvent.UnRegister(stringChanged))
            return;

        _events.Remove(key);
    }

    public void UnRegister(GraphConfigKey key, FloatChanged floatChanged)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            Debug.LogWarning("GraphEvent didn't exist to unregister Delegate");
            return;
        }

        if (graphConfigEvent.UnRegister(floatChanged))
            return;

        _events.Remove(key);
    }

    public void UnRegister(GraphConfigKey key, BoolChanged boolChanged)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            Debug.LogWarning("GraphEvent didn't exist to unregister Delegate");
            return;
        }

        if (graphConfigEvent.UnRegister(boolChanged))
            return;

        _events.Remove(key);
    }

    public void UnRegister(GraphConfigKey key, ColorChanged colorChanged)
    {
        if (!_events.TryGetValue(key, out GraphConfigEvent graphConfigEvent))
        {
            Debug.LogWarning("GraphEvent didn't exist to unregister Delegate");
            return;
        }

        if (graphConfigEvent.UnRegister(colorChanged))
            return;

        _events.Remove(key);
    }
}

public delegate void StringChanged(string value);
public delegate void FloatChanged(float value);
public delegate void BoolChanged(bool value);
public delegate void ColorChanged(Color value);