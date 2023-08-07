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

                UpdateColliderActivation();
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
                _colorImg.color = _value;
            }
        }


        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveGraphicUI> _interactiveGraphics;

        [SerializeField]
        Image _colorImg;

        [SerializeField]
        Transform _keyboardPositionTf;

        [SerializeField]
        GameObject _interactionCollidersGo;

        [SerializeField]
        KeyboardAlignment _keyboardAlignment;

        [SerializeField]
        Color _value;

        [SerializeField, Space(10)]
        UnityEvent<Color> _onValueChanged;

        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;

        bool _isActive = false;

        private void OnEnable()
        {
            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();

            _colorImg.color = _value;
        }

        private void OnDisable()
        {
            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);
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
            _colorImg.color = _value;

            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
    }
}