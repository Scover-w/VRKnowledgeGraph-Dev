using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class InputUI : MonoBehaviour, ITactileUI
    {
        [SerializeField]
        ColorStateUI _color;

        [SerializeField]
        Image _img;

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

        Transform _touchTf;
        TouchInteractor _touchInter;

        string _value = "";

        bool _isActive = false;

        private void OnEnable()
        {
            DisplayValue();
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
            _value = (string)input;
            DisplayValue();

        }

        public void OnEnterInput(object input)
        {
            _value = (string)input;
            DisplayValue();

            _isActive = false;
            UpdateColor(InteractionStateUI.Normal);
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
    }
}