using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class ColorPickerUI : MonoBehaviour
    {
        [SerializeField]
        MultiInputController _controllerUI;

        [SerializeField]
        HSVWheelUI _wheelUI;

        [SerializeField]
        Image _oldColorImg;

        [SerializeField]
        Image _newColorImg;

        [SerializeField]
        SliderUI _sliderH;

        [SerializeField]
        SliderUI _sliderS;

        [SerializeField]
        SliderUI _sliderV;

        Color _oldColor;
        Color _newColor;

        HSVColor _hsvColor;

        public void StartColor(Color oldColor)
        {
            _oldColor = oldColor;
            _newColor = oldColor;

            _oldColorImg.color = _oldColor;
            _newColorImg.color = oldColor;

            _hsvColor = HSVColor.RGBToHSV(oldColor);
            UpdateSliderValue();
            _wheelUI.UpdateColor(_hsvColor);
        }

        public void OnNewColorFromWheel(HSVColor hsvColor)
        {
            _hsvColor = hsvColor;

            _newColor = _hsvColor.ToRGB();

            _newColorImg.color = _newColor;
            UpdateSliderValue();

            _controllerUI.UpdateInputValue(_newColor);
        }

        private void UpdateSliderValue()
        {
            _sliderH.Value = _hsvColor.h;
            _sliderS.Value = _hsvColor.s;
            _sliderV.Value = _hsvColor.v;
        }

        public void OnNewHSlider(float h)
        {
            _hsvColor.h = h;
            UpdateColorAfterSlider();
        }

        public void OnNewSSlider(float s)
        {
            _hsvColor.s = s;
            UpdateColorAfterSlider();
        }

        public void OnNewVSlider(float v)
        {
            _hsvColor.v = v;
            UpdateColorAfterSlider();
        }

        private void UpdateColorAfterSlider()
        {
            _newColor = _hsvColor.ToRGB();

            _newColorImg.color = _newColor;
            _controllerUI.UpdateInputValue(_newColor);

            _wheelUI.UpdateColor(_hsvColor);
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
}