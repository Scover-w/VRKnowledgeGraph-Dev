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
        InteractiveColorUI _colorState;

        InteractionStateUI _interactionStateUI;

        bool _isSelected = false;

        private void OnEnable()
        {
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();
        }

        public void TriggerEnter(bool isProximity, Transform touchTf)
        {

            if (isProximity)
            {
                _touchTf = touchTf;
                _touchInter = _touchTf.GetComponent<TouchInteractor>();
                _interactionStateUI = InteractionStateUI.InProximity;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                TryClick();
            }
        }

        private void TryClick()
        {
            _interactionStateUI = InteractionStateUI.Active;
            UpdateInteractionColor();

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _controller.CloseFromDropdown(_value);
        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                if (_touchInter != null)
                    _touchInter.ActiveBtn(false, this);

                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
        }

        private void UpdateInteractionColor()
        {
            switch (_interactionStateUI)
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
                case InteractionStateUI.Disabled:
                    _label.color = _colorState.DisabledColor;
                    _checkMarkImg.color = _colorState.DisabledColor;
                    break;
            }
        }

        public void ResfreshValue(string value, InteractiveColorUI colorState)
        {
            _colorState = colorState;
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();

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