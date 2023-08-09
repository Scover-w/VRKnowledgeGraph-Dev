using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Wave.Essence.Hand.NearInteraction;

namespace AIDEN.TactileUI
{
    public class ToggleUI : MonoBehaviour, ITouchUI, IValueUI<bool>
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
            }
        }

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

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveGraphicUI> _activeInteractiveGraphics;

        [SerializeField]
        List<InteractiveGraphicUI> _unactiveInteractiveGraphics;

        [SerializeField]
        RectTransform _knobRect;

        [SerializeField]
        GameObject _interactionCollidersGo;

        [SerializeField, Space(10)]
        UnityEvent<bool> _onValueChanged;

        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;

        bool _isEnable = false;
        bool _canSwitch = true;

        float _xDeltaState = 16.9f;

        private void OnEnable()
        {
            TrySetNormalInteractionState();
            UpdateVisuals();
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
                TryClick();
            }
        }

        private void TryClick()
        {
            if (!_canSwitch)
                return;

            _canSwitch = false;
            _isEnable = !_isEnable;
            _interactionStateUI = InteractionStateUI.Active;
            _touchInter.ActivateHaptic();

            UpdateVisuals();

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _onValueChanged?.Invoke(_isEnable);
        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                _canSwitch = true;
                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                if (_touchInter != null && !_canSwitch)
                    _touchInter.ActiveBtn(false, this);

                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
        }

        private void UpdateVisuals()
        {
            UpdateInteractionColor();
            UpdateKnobPosition();
        }

        private void UpdateInteractionColor()
        {
            var interactiveGraphics = _isEnable ? _activeInteractiveGraphics : _unactiveInteractiveGraphics;
            interactiveGraphics.UpdateColor(_interactionStateUI);
        }

        private void UpdateKnobPosition()
        {
            Vector3 position = _knobRect.localPosition;
            position.x = _xDeltaState * (_isEnable ? 1 : -1);
            _knobRect.localPosition = position;
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            _activeInteractiveGraphics?.TrySetName();
            _unactiveInteractiveGraphics?.TrySetName();

            UpdateKnobPosition();
            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
#endif
    }
}