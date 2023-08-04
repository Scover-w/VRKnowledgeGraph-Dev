using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class DropdownItemUI : MonoBehaviour, ITouchUI, IValueUI<string>
    {
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        [SerializeField]
        DropdownUI _controller;

        [SerializeField]
        Image _checkMarkImg;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        string _value;

        GameObject _checkMarkGo;
        Transform _touchTf;
        TouchInteractor _touchInter;
        ColorStateUI _colorState;

        bool _isSelected = false;


        public void TriggerEnter(bool isProximity, Transform touchTf)
        {

            if (isProximity)
            {
                _touchTf = touchTf;
                _touchInter = _touchTf.GetComponent<TouchInteractor>();
                UpdateColor(InteractionStateUI.InProximity);
            }
            else if (!isProximity)
            {
                TryClick();
            }
        }

        private void TryClick()
        {
            UpdateColor(InteractionStateUI.Active);

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _controller.CloseFromDropdown(_value);
        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                UpdateColor(InteractionStateUI.Normal);
            }
            else if (!isProximity)
            {
                if (_touchInter != null)
                    _touchInter.ActiveBtn(false, this);

                UpdateColor(InteractionStateUI.Normal);
            }
        }

        private void UpdateColor(InteractionStateUI interactionState)
        {
            switch (interactionState)
            {
                case InteractionStateUI.Normal:
                    _label.color = _colorState.NormalColor;
                    _checkMarkImg.color = _colorState.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    _label.color = _colorState.ProximityColor;
                    _checkMarkImg.color = _colorState.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    _label.color = _colorState.ActivatedColor;
                    _checkMarkImg.color = _colorState.ActivatedColor;
                    break;
            }
        }

        public void ResfreshValue(string value, ColorStateUI colorState)
        {
            _colorState = colorState;
            UpdateColor(InteractionStateUI.Normal);

            _isSelected = (_value == value);

            if (_checkMarkGo == null)
                _checkMarkGo = _checkMarkImg.gameObject;
            _checkMarkGo.SetActive(_isSelected);
        }

        private void OnValidate()
        {
            if (_label == null)
                return;

            _label.text = _value;
        }
    }
}