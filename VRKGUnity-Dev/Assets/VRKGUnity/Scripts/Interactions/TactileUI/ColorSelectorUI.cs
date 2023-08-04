using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class ColorSelectorUI : MonoBehaviour, ITouchUI, IValueUI<Color>
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

        public Color Value
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
        Image _colorImg;

        [SerializeField]
        Transform _keyboardPositionTf;

        [SerializeField]
        KeyboardAlignment _keyboardAlignment;

        [SerializeField, Space(10)]
        UnityEvent<Color> _onValueChanged;

        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;

        Color _value;
        bool _isActive = false;

        private void OnEnable()
        {
            TrySetNormalInteractionState();
            UpdateInteractionColor();

            _colorImg.color = _value;
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
                    _interactiveImg.color= _interactiveColor.DisabledColor;
                    break;
            }
        }

        public void OnUpdateInput(object input)
        {
            if (input is not Color)
                return;

            _value = (Color)input;
            _colorImg.color = _value;
        }

        public void OnEnterInput(object input)
        {
            if (input is not Color)
                _value = Color.white;
            else
                _value = (Color)input;

            _colorImg.color = _value;

            _isActive = false;
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();

            _onValueChanged?.Invoke(_value);
        }

        private KeyboardUIOptions<Color> CreateKeyboardOptions()
        {

            return new KeyboardUIOptions<Color>(_keyboardPositionTf.position,
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