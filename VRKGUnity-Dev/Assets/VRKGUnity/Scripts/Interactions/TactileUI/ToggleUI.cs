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

                TrySetNormalInteractionState();
                UpdateInteractionColor();
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveColorUI> _activeInteractiveColors;

        [SerializeField]
        List<InteractiveColorUI> _unactiveInteractiveColors;

        [SerializeField]
        List<Image> _interactiveImgs;

        [SerializeField]
        RectTransform _knobRect;

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
            var colorStates = _isEnable ? _activeInteractiveColors : _unactiveInteractiveColors;

            int nbImg = _interactiveImgs.Count;

            for (int i = 0; i < nbImg; i++)
            {
                Image img = _interactiveImgs[i];
                InteractiveColorUI colorstate = colorStates[i];

                switch (_interactionStateUI)
                {
                    case InteractionStateUI.Normal:
                        img.color = colorstate.NormalColor;
                        break;
                    case InteractionStateUI.InProximity:
                        img.color = colorstate.ProximityColor;
                        break;
                    case InteractionStateUI.Active:
                        img.color = colorstate.ActivatedColor;
                        break;
                    case InteractionStateUI.Disabled:
                        img.color = colorstate.DisabledColor;
                        break;
                }
            }
        }

        private void UpdateKnobPosition()
        {
            Vector3 position = _knobRect.localPosition;
            position.x = _xDeltaState * (_isEnable ? 1 : -1);
            _knobRect.localPosition = position;
        }

        private void TrySetNormalInteractionState()
        {
            if (_interactable)
                _interactionStateUI = InteractionStateUI.Normal;
            else
                _interactionStateUI = InteractionStateUI.Disabled;
        }
    }
}