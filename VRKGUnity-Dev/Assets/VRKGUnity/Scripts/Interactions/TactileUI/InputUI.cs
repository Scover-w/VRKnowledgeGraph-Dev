using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class InputUI : MonoBehaviour, ITouchUI, IValueUI<string>
    {
        public bool Interactable
        {
            get
            {
                return _interactable;
            }
            set
            {
                _interactable = value;

                TrySetNormalInteractionState();
                UpdateInteractionColor();
            }
        }

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
        bool _interactable = true;

        [SerializeField]
        InteractiveColorUI _interactiveColor;

        [SerializeField]
        Image _interactiveImg;

        [SerializeField]
        TMP_Text _inputText;

        [SerializeField]
        Color _noValueTextColor;

        [SerializeField]
        Color _normalTextColor;

        [SerializeField]
        Transform _keyboardPositionTf;

        [SerializeField]
        KeyboardAlignment _keyboardAlignment;

        [SerializeField]
        string _noValueInfoText = "";

        [SerializeField, Space(10)]
        UnityEvent<string> _onValueChanged;

        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;

        string _value = "";

        bool _isActive = false;

        private void OnEnable()
        {
            TrySetNormalInteractionState();
            UpdateInteractionColor();
            DisplayValue();
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
                TryActivate();
            }
        }

        private void TryActivate()
        {
            if (_isActive)
                return;

            var options = CreateKeyboardOptions();
            bool succeedUsingKeyboard = MultiInputController.Display(options);

            if (!succeedUsingKeyboard)
                return;

            _isActive = true;
            _interactionStateUI = InteractionStateUI.Active;
            UpdateInteractionColor();

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);
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
                if (_touchInter != null && _isActive)
                    _touchInter.ActiveBtn(false, this);

                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
        }

        private void UpdateInteractionColor()
        {
            if (_isActive)
            {
                _interactiveImg.color = _interactiveColor.ActivatedColor;
                return;
            }

            switch (_interactionStateUI)
            {
                case InteractionStateUI.Normal:
                    _interactiveImg.color = _interactiveColor.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    _interactiveImg.color = _interactiveColor.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    _interactiveImg.color = _interactiveColor.ActivatedColor;
                    break;
                case InteractionStateUI.Disabled:
                    _interactiveImg.color = _interactiveColor.DisabledColor;
                    break;
            }
        }

        public void OnUpdateInput(object input)
        {
            _value = (string)input;
            DisplayValue();

        }

        public void OnEnterInput(object input)
        {
            _value = (string)input;
            DisplayValue();

            _isActive = false;
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();

            _onValueChanged?.Invoke(_value);
        }

        private void DisplayValue()
        {
            if (_value == null || _value.Length == 0 || _value == _noValueInfoText)
            {
                _inputText.text = _noValueInfoText;
                _inputText.color = _noValueTextColor;
                return;
            }

            _inputText.text = _value;
            _inputText.color = _normalTextColor;
        }

        private KeyboardUIOptions<string> CreateKeyboardOptions()
        {
            return new KeyboardUIOptions<string>(_keyboardPositionTf.position,
                                            _keyboardAlignment,
                                            OnUpdateInput,
                                            OnEnterInput,
                                            _value);
        }

        private void TrySetNormalInteractionState()
        {
            if (_interactable)
                _interactionStateUI = InteractionStateUI.Normal;
            else
                _interactionStateUI = InteractionStateUI.Disabled;
        }
    }
}