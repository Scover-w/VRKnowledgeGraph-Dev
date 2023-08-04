using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerUI : MonoBehaviour
{
    [SerializeField]
    KeyboardControllerUI _controllerUI;

    [SerializeField]
    PhysicalHSVWheelUI _wheelUI;

    [SerializeField]
    Image _oldColorImg;

    [SerializeField]
    Image _newColorImg;

    [SerializeField]
    PhysicalSliderUI _sliderH;

    [SerializeField]
    PhysicalSliderUI _sliderS;

    [SerializeField]
    PhysicalSliderUI _sliderV;

    Color _oldColor;
    Color _newColor;

    float _h;
    float _s;
    float _v;

    public void StartColor(Color oldColor)
    {
        _oldColor = oldColor;
        _newColor = oldColor;

        _oldColorImg.color = _oldColor;
        _newColorImg.color = oldColor;

        Color.RGBToHSV(oldColor, out _h, out _s, out _v);
        _wheelUI.UpdateColor(_h, _s, _v);
    }

    public void SetNewColorFromWheel(float h, float s, float v)
    {
        _h = h;
        _s = s;
        _v = v;

        _newColor = Color.HSVToRGB(h, s, v);

        _newColorImg.color = _newColor;
        _sliderH.SetNewValue(h);
        _sliderS.SetNewValue(s);
        _sliderV.SetNewValue(v);

        _controllerUI.UpdateInputValue(_newColor);
    }

    public void OnNewHSlider(float h)
    {
        _h = h;
        SetNewColorFromSlider();
    }

    public void OnNewSSlider(float s)
    {
        _s = s;
        SetNewColorFromSlider();
    }

    public void OnNewVSlider(float v)
    {
        _v = v;

        _wheelUI.UpdateV(_v);

        SetNewColorFromSlider();
    }

    private void SetNewColorFromSlider()
    {
        _newColor = Color.HSVToRGB(_h, _s, _v);

        _newColorImg.color = _newColor;
        _controllerUI.UpdateInputValue(_newColor);
        _wheelUI.UpdateColor(_h, _s, _v);
    }

    public void Confirm()
    {
        _controllerUI.EnterInputValue(_newColor);
    }

    public void Cancel()
    {
        _controllerUI.EnterInputValue(_oldColor);
    }



}
