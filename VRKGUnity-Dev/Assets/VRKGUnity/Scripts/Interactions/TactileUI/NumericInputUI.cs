using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AIDEN.TactileUI
{
    public class NumericInputUI : BaseTouch, IValueUI<float>
    {

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = Mathf.Clamp(value, _minValue, _maxValue);

                TryRoundValue();

                _label.text = _value.ToString("0.###");
            }
        }

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        Transform _inputPositionTf;

        [SerializeField]
        InputAlignment _inputAlignment;

        [SerializeField, Space(5)]
        NumericType _numericType;

        [SerializeField]
        float _minValue = 0f;

        [SerializeField]
        float _maxValue = 1f;

        [SerializeField]
        float _value;

        [SerializeField, Space(10)]
        UnityEvent<float> _onValueChanged;


        bool _isActive = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            _isActive = false;
        }

        protected override void TryActivate()
        {
            if(!base.CanActivate()) 
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


        public void OnUpdateInput(object input)
        {
            if (input is not string)
                return;

            _label.text = ((float)input).ToString("0.###");
        }

        public void OnEnterInput(object input)
        {
            if (input is not float)
                _value = 0f;
            else
                _value = (float)input;


            TryRoundValue();

            _label.text = _value.ToString("0.###");

            _isActive = false;
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();

            _onValueChanged?.Invoke(_value);
        }

        private KeyboardUIOptions<float> CreateKeyboardOptions()
        {

            return new KeyboardUIOptions<float>(_inputPositionTf.position,
                                            _inputAlignment,
                                            OnUpdateInput,
                                            OnEnterInput,
                                            _value);
        }

        private void TryRoundValue()
        {
            if (_numericType == NumericType.Int)
                _value = Mathf.Round(_value);
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            TryRoundValue();
            

            if (_numericType == NumericType.Int)
            {
                _minValue = Mathf.Round(_minValue);
                _maxValue = Mathf.Round(_maxValue);
            }

            Value = _value;
        }
#endif
    }

    public enum NumericType
    {
        Float,
        Int
    }
}