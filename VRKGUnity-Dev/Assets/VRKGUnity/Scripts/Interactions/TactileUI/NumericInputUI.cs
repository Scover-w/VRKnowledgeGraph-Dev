using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class NumericInputUI : MonoBehaviour, ITactileUI
    {
        [SerializeField]
        ColorStateUI _color;

        [SerializeField]
        Image _img;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        Transform _keyboardPositionTf;

        [SerializeField]
        KeyboardAlignment _keyboardAlignment;

        Transform _touchTf;
        TouchInteractor _touchInter;

        float _value;

        bool _isActive = false;

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
            if (input is not string)
                return;

            _label.text = input.ToString();
        }

        public void OnEnterInput(object input)
        {
            if (input is not float)
                _value = 0f;
            else
                _value = (float)input;

            _label.text = _value.ToString();

            _isActive = false;
            UpdateColor(InteractionStateUI.Normal);
        }

        private KeyboardUIOptions<float> CreateKeyboardOptions()
        {

            return new KeyboardUIOptions<float>(_keyboardPositionTf.position,
                                            _keyboardAlignment,
                                            OnUpdateInput,
                                            OnEnterInput,
                                            _value);
        }

    }
}