using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class LongToggleUI : MonoBehaviour, ITouchUI, IValueUI<bool>
    {
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

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveColorUI> _interactiveImgs;

        [SerializeField]
        List<Image> _interactiveColors;

        [SerializeField]
        RectTransform _toggleRect;

        [SerializeField]
        RectTransform _knobRect;

        [SerializeField]
        TMP_Text _text;

        [SerializeField]
        string _disabledValue;

        [SerializeField]
        string _enabledValue;

        [SerializeField, Space(10)]
        UnityEvent<bool> _onValueChanged;

        RectTransform _rectText;
        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;

        float _xDeltaKnob;
        float _xDeltaText;

        bool _isEnable = false;
        bool _canSwitch = true;


        private void OnEnable()
        {
            float knobWidth = _knobRect.rect.width;
            float toggleWidth = _toggleRect.rect.width;

            _xDeltaKnob = (toggleWidth - 8f - knobWidth * .5f);
            _xDeltaText = (toggleWidth - 4f - knobWidth) * .5f;

            _xDeltaKnob -= (toggleWidth * .5f);
            _xDeltaText -= (toggleWidth * .5f);

            _rectText = _text.GetComponent<RectTransform>();

            TrySetNormalInteractionState();

            UpdateKnobPosition();
            UpdateInteractionColor();
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
            int nbImg = _interactiveColors.Count;

            for (int i = 0; i < nbImg; i++)
            {
                Image img = _interactiveColors[i];
                InteractiveColorUI colorState = _interactiveImgs[i];

                switch (_interactionStateUI)
                {
                    case InteractionStateUI.Normal:
                        img.color = colorState.NormalColor;
                        break;
                    case InteractionStateUI.InProximity:
                        img.color = colorState.ProximityColor;
                        break;
                    case InteractionStateUI.Active:
                        img.color = colorState.ActivatedColor;
                        break;
                    case InteractionStateUI.Disabled:
                        img.color = colorState.DisabledColor;
                        break;
                }
            }
        }

        private void UpdateKnobPosition()
        {
            Vector3 posA = _knobRect.localPosition;
            posA.x = _xDeltaKnob * (_isEnable ? 1 : -1);
            _knobRect.localPosition = posA;

            _text.text = _isEnable ? _enabledValue : _disabledValue;
            Vector3 posB = _rectText.localPosition;
            posB.x = _xDeltaText * (_isEnable ? 1 : -1);
            _rectText.localPosition = posB;
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