using UnityEngine;

/// <summary>
/// Event used to store <see cref="GraphConfigInputLink.OnChangedFromManager"/> methods.
/// </summary>
public class GraphConfigEvent
{
    ValueChanged<int> _onIntChanged;
    ValueChanged<float> _onFloatChanged;
    ValueChanged<bool> _onBoolChanged;
    ValueChanged<Color> _onColorChanged;

    InteractableStateChanged _onInteractableStateChanged;

    public void Register<T>(ValueChanged<T> valueChanged, InteractableStateChanged interactableStateChanged = null)
    {
        if (interactableStateChanged != null)
            _onInteractableStateChanged += interactableStateChanged;


        if (typeof(T) == typeof(int))
            _onIntChanged += valueChanged as ValueChanged<int>;

        else if (typeof(T) == typeof(float))
            _onFloatChanged += valueChanged as ValueChanged<float>;

        else if (typeof(T) == typeof(bool))
            _onBoolChanged += valueChanged as ValueChanged<bool>;

        else if (typeof(T) == typeof(Color))
            _onColorChanged += valueChanged as ValueChanged<Color>;   
    }

    public bool UnRegister<T>(ValueChanged<T> valueChanged, InteractableStateChanged interactableStateChanged = null)
    {
        if (interactableStateChanged != null)
            _onInteractableStateChanged += interactableStateChanged;


        if (typeof(T) == typeof(int))
        {
            _onIntChanged -= valueChanged as ValueChanged<int>;
            return _onIntChanged != null;
        }
        else if (typeof(T) == typeof(float))
        {
            _onFloatChanged -= valueChanged as ValueChanged<float>;
            return _onFloatChanged != null;
        }
        else if (typeof(T) == typeof(bool))
        {
            _onBoolChanged -= valueChanged as ValueChanged<bool>;
            return _onBoolChanged != null;
        }

        else if (typeof(T) == typeof(Color))
        {
            _onColorChanged -= valueChanged as ValueChanged<Color>;
            return _onColorChanged != null;
        }

        return true;
    }

    public void InvokeValueChanged<T>(T value)
    {
        switch (value)
        {
            case int i:
                _onIntChanged?.Invoke(i);
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

    public void InvokeInteractableStateChanged(bool isInteractable)
    {
        _onInteractableStateChanged?.Invoke(isInteractable);
    }

}
