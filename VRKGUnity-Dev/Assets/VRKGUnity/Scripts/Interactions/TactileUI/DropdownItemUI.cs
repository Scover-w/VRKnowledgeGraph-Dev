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
        List<InteractiveGraphicUI> _interactiveGraphics;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        GameObject _checkMarkGo;

        string _value;

        Transform _touchTf;
        TouchInteractor _touchInter;

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
            _interactiveGraphics.UpdateColor(_interactionStateUI);
        }

        public void ResfreshValue(string value)
        {
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();

            _isSelected = (_value == value);
            _checkMarkGo.SetActive(_isSelected);
        }

        private void OnValidate()
        {
            _interactiveGraphics?.TrySetName();

            if (_label == null)
                return;

            _label.text = _value;
        }
    }
}