using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class ColorSelectorUI : BaseTouch, IValueUI<Color>
    {
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
        Image _colorImg;

        [SerializeField]
        Transform _keyboardPositionTf;


        [SerializeField]
        KeyboardAlignment _keyboardAlignment;

        [SerializeField]
        Color _value;

        [SerializeField, Space(10)]
        UnityEvent<Color> _onValueChanged;


        bool _isActive = false;


        protected override void OnEnable()
        {
            base.OnEnable();

            _isActive = false;
            _colorImg.color = _value;
        }


        protected override void TryActivate()
        {
            if (!base.CanActivate())
                return;

            if (_isActive)
                return;

            _touchInter.ActivateHaptic();
            var options = CreateKeyboardOptions();
            bool succeedUsingKeyboard = MultiInputController.Display(options);

            if (!succeedUsingKeyboard)
                return;

            base.Activate();
            _isActive = true;
        }


        protected override void UpdateInteractionColor()
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


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _colorImg.color = _value;
        }
#endif
    }
}