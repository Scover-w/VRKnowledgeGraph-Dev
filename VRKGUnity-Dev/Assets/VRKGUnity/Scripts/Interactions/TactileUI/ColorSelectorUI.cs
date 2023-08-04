using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class ColorSelectorUI : MonoBehaviour, ITouchUI, IValueUI<Color>
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
            }
        }

        [SerializeField]
        ColorStateUI _color;

        [SerializeField]
        Image _img;

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


        Color _value;
        bool _isActive = false;

        private void OnEnable()
        {
            _colorImg.color = _value;
        }

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
            UpdateColor(InteractionStateUI.Active);

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);
        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                UpdateColor(InteractionStateUI.Normal);
            }
            else if (!isProximity)
            {
                if (_touchInter != null && _isActive)
                    _touchInter.ActiveBtn(false, this);

                UpdateColor(InteractionStateUI.Normal);
            }
        }

        private void UpdateColor(InteractionStateUI interactionState)
        {
            if (_isActive)
            {
                _img.color = _color.ActivatedColor;
                return;
            }

            switch (interactionState)
            {
                case InteractionStateUI.Normal:
                    _img.color = _color.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    _img.color = _color.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    _img.color = _color.ActivatedColor;
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
            UpdateColor(InteractionStateUI.Normal);

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
    }
}