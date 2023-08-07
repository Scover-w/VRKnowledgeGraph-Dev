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

                UpdateColliderActivation();
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
                DisplayValue();
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveGraphicUI> _interactiveGraphics;

        [SerializeField]
        TMP_Text _inputText;

        [SerializeField]
        Color _noValueTextColor;

        [SerializeField]
        Color _normalTextColor;

        [SerializeField]
        Transform _keyboardPositionTf;

        [SerializeField]
        GameObject _interactionCollidersGo;

        [SerializeField]
        KeyboardAlignment _keyboardAlignment;

        [SerializeField]
        string _noValueInfoText = "";

        [SerializeField]
        string _value = "";


        [SerializeField, Space(10)]
        UnityEvent<string> _onValueChanged;

        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;


        bool _isActive = false;

        private void OnEnable()
        {
            UpdateColliderActivation();
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
                _interactiveGraphics.UpdateColor(InteractionStateUI.Active);
                return;
            }

            _interactiveGraphics.UpdateColor(_interactionStateUI);
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

        private void UpdateColliderActivation()
        {
            _interactionCollidersGo.SetActive(_interactable);
        }

        private void TrySetNormalInteractionState()
        {
            if (_interactable)
                _interactionStateUI = InteractionStateUI.Normal;
            else
                _interactionStateUI = InteractionStateUI.Disabled;
        }

        private void OnValidate()
        {
            _interactiveGraphics?.TrySetName();

            DisplayValue();
            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
    }
}