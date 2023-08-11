using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class InputUI : BaseTouch, IValueUI<string>
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
                DisplayValue();
            }
        }

        [SerializeField]
        TMP_Text _inputText;

        [SerializeField]
        Color _noValueTextColor;

        [SerializeField]
        Color _normalTextColor;

        [SerializeField]
        Transform _inputPositionTf;

        [SerializeField]
        InputAlignment _inputAlignment;

        [SerializeField]
        string _noValueInfoText = "";

        [SerializeField]
        string _value = "";


        [SerializeField, Space(10)]
        UnityEvent<string> _onValueChanged;


        bool _isActive = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            _isActive = false;
            DisplayValue();
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
            return new KeyboardUIOptions<string>(_inputPositionTf.position,
                                            _inputAlignment,
                                            OnUpdateInput,
                                            OnEnterInput,
                                            _value);
        }



#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            DisplayValue();
        }
#endif
    }
}