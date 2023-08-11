using UnityEngine;


public class GraphConfigEvent
{
    readonly GraphConfigValueType _type;
    StringChanged _onStringChanged;
    FloatChanged _onFloatChanged;
    BoolChanged _onBoolChanged;
    ColorChanged _onColorChanged;

    public GraphConfigEvent(GraphConfigValueType type)
    {
        _type = type;
    }

    public void Register(StringChanged stringChanged)
    {
        _onStringChanged += stringChanged;
    }

    public void Register(FloatChanged floatChanged)
    {
        _onFloatChanged += floatChanged;
    }

    public void Register(BoolChanged boolChanged)
    {
        _onBoolChanged += boolChanged;
    }

    public void Register(ColorChanged colorChanged)
    {
        _onColorChanged += colorChanged;
    }

    public bool UnRegister(StringChanged stringChanged)
    {
        _onStringChanged -= stringChanged;
        return _onStringChanged.GetInvocationList().Length > 0;
    }

    public bool UnRegister(FloatChanged floatChanged)
    {
        _onFloatChanged -= floatChanged;
        return _onFloatChanged.GetInvocationList().Length > 0;
    }

    public bool UnRegister(BoolChanged boolChanged)
    {
        _onBoolChanged -= boolChanged;
        return _onBoolChanged.GetInvocationList().Length > 0;
    }

    public bool UnRegister(ColorChanged colorChanged)
    {
        _onColorChanged -= colorChanged;
        return _onColorChanged.GetInvocationList().Length > 0;
    }

    public void Invoke(string value)
    {
        _onStringChanged?.Invoke(value);
    }

    public void Invoke(float value)
    {
        _onFloatChanged?.Invoke(value);
    }

    public void Invoke(bool value)
    {
        _onBoolChanged?.Invoke(value);
    }

    public void Invoke(Color value)
    {
        _onColorChanged?.Invoke(value);
    }

}
