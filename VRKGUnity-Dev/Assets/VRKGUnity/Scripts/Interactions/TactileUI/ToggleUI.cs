using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AIDEN.TactileUI
{
    public class ToggleUI : BaseTouch, IValueUI<bool>
    {
        public bool Value
        {
            get
            {
                return _isEnable;
            }
            set
            { 
                _isEnable = value;
                UpdateKnobPosition();
                UpdateInteractionColor();
            }
        }

        [SerializeField]
        List<InteractiveGraphicUI> _unactiveInteractiveGraphics;

        [SerializeField]
        RectTransform _knobRect;


        [SerializeField, Space(10)]
        UnityEvent<bool> _onValueChanged;


        bool _isEnable = false;
        float _xDeltaState = 16.9f;


        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateKnobPosition();
        }

        protected override void TryActivate()
        {
            if (!base.CanActivate())
                return;

            base.Activate();

            _isEnable = !_isEnable;
            _touchInter.ActivateHaptic();

            UpdateKnobPosition();

            _onValueChanged?.Invoke(_isEnable);
        }

        protected override void UpdateInteractionColor()
        {
            var interactiveGraphics = _isEnable ? _interactiveGraphics : _unactiveInteractiveGraphics;
            interactiveGraphics.UpdateColor(_interactionStateUI);
        }

        private void UpdateKnobPosition()
        {
            Vector3 position = _knobRect.localPosition;
            position.x = _xDeltaState * (_isEnable ? 1 : -1);
            _knobRect.localPosition = position;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _unactiveInteractiveGraphics?.TrySetName();
            UpdateKnobPosition();
        }
#endif
    }
}