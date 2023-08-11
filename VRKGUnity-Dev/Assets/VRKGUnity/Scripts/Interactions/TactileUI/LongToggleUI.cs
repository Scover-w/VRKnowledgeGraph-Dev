using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class LongToggleUI : BaseTouch, IValueUI<bool>
    {
        public bool Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                UpdateVisual();
            }
        }


        [SerializeField]
        RectTransform _toggleRect;

        [SerializeField]
        RectTransform _knobRect;


        [SerializeField]
        TMP_Text _text;

        [SerializeField]
        string _disabledValueLabel;

        [SerializeField]
        string _enabledValueLabel;

        [SerializeField]
        bool _value = false;

        [SerializeField, Space(10)]
        UnityEvent<bool> _onValueChanged;

        RectTransform _rectText;

        float _xDeltaKnob;
        float _xDeltaText;

        protected override void OnEnable()
        {
            base.OnEnable();

            SetDefaultValues();
            UpdateVisual();
        }


        private void SetDefaultValues()
        {
            float knobWidth = _knobRect.rect.width;
            float toggleWidth = _toggleRect.rect.width;

            _xDeltaKnob = (toggleWidth - 8f - knobWidth * .5f);
            _xDeltaText = (toggleWidth - 4f - knobWidth) * .5f;

            _xDeltaKnob -= (toggleWidth * .5f);
            _xDeltaText -= (toggleWidth * .5f);

            _rectText = _text.GetComponent<RectTransform>();
        }


        protected override void TryActivate()
        {
            if (!base.CanActivate())
                return;

            _value = !_value;
            _interactionStateUI = InteractionStateUI.Active;
            _touchInter.ActivateHaptic();

            UpdateVisuals();

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _onValueChanged?.Invoke(_value);
        }

        private void UpdateVisuals()
        {
            UpdateInteractionColor();
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            Vector3 posKnob = _knobRect.localPosition;
            posKnob.x = _xDeltaKnob * (_value ? 1 : -1);
            _knobRect.localPosition = posKnob;

            _text.text = _value ? _enabledValueLabel : _disabledValueLabel;
            Vector3 posLabel = _rectText.localPosition;
            posLabel.x = _xDeltaText * (_value ? 1 : -1);
            _rectText.localPosition = posLabel;
        }



#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetDefaultValues();
            UpdateVisual();
        }
#endif
    }
}