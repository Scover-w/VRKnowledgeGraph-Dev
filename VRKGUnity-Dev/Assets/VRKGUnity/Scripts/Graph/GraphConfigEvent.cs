using AngleSharp.Html;
using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;


public class GraphConfigEvent
{
    ValueChanged<string> _onStringChanged;
    ValueChanged<float> _onFloatChanged;
    ValueChanged<bool> _onBoolChanged;
    ValueChanged<Color> _onColorChanged;

    public void Register<T>(ValueChanged<T> valueChanged)
    {
        if (typeof(T) == typeof(string))
            _onStringChanged += valueChanged as ValueChanged<string>;

        else if (typeof(T) == typeof(float))
            _onFloatChanged += valueChanged as ValueChanged<float>;

        else if (typeof(T) == typeof(bool))
            _onBoolChanged += valueChanged as ValueChanged<bool>;

        else if (typeof(T) == typeof(Color))
            _onColorChanged += valueChanged as ValueChanged<Color>;
    }

    public bool UnRegister<T>(ValueChanged<T> valueChanged)
    {
        if (typeof(T) == typeof(string))
        {
            _onStringChanged -= valueChanged as ValueChanged<string>;
            return _onStringChanged.GetInvocationList().Length > 0;
        }
        else if (typeof(T) == typeof(float))
        {
            _onFloatChanged -= valueChanged as ValueChanged<float>;
            return _onFloatChanged.GetInvocationList().Length > 0;
        }
        else if (typeof(T) == typeof(bool))
        {
            _onBoolChanged -= valueChanged as ValueChanged<bool>;
            return _onBoolChanged.GetInvocationList().Length > 0;
        }

        else if (typeof(T) == typeof(Color))
        {
            _onColorChanged -= valueChanged as ValueChanged<Color>;
            return _onColorChanged.GetInvocationList().Length > 0;
        }

        return true;
    }

    public void Invoke<T>(T value)
    {
        switch (value)
        {
            case string s:
                _onStringChanged?.Invoke(s);
                break;
            case float f:
                _onFloatChanged?.Invoke(f);
                break;
            case bool b:
                _onBoolChanged?.Invoke(b);
                break;
            case Color c:
                _onColorChanged?.Invoke(c);
                break;
            default:
                Debug.LogError("No event with " + typeof(T) + " is handled");
                break;
        }
    }

}
