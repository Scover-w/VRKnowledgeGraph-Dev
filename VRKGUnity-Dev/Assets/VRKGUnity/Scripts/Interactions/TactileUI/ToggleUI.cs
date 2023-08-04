using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
            }
        }


        [SerializeField]
        List<ColorStateUI> _enabledColorStates;

        [SerializeField]
        List<ColorStateUI> _disabledColorStates;

        [SerializeField]
        List<Image> _imgs;

        [SerializeField]
        RectTransform _knobRect;

        [SerializeField, Space(10)]
        UnityEvent<bool> _onValueChanged;

        Transform _touchTf;
        TouchInteractor _touchInter;

        bool _isEnable = false;
        bool _canSwitch = true;

        float _xDeltaState = 16.9f;

        private void Start()
        {
            UpdateKnobPosition();
            UpdateColor(InteractionStateUI.Normal);
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
                TryClick();
            }
        }

        private void TryClick()
        {
            if (!_canSwitch)
                return;

            _canSwitch = false;
            _isEnable = !_isEnable;
            UpdateKnobPosition();
            UpdateColor(InteractionStateUI.Active);

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _onValueChanged?.Invoke(_isEnable);
        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                _canSwitch = true;
                UpdateColor(InteractionStateUI.Normal);
            }
            else if (!isProximity)
            {
                if (_touchInter != null && !_canSwitch)
                    _touchInter.ActiveBtn(false, this);

                UpdateColor(InteractionStateUI.Normal);
            }
        }

        private void UpdateColor(InteractionStateUI interactionState)
        {
            var colorStates = _isEnable ? _enabledColorStates : _disabledColorStates;

            int nbImg = _imgs.Count;

            for (int i = 0; i < nbImg; i++)
            {
                Image img = _imgs[i];
                ColorStateUI colorstate = colorStates[i];

                switch (interactionState)
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
                }
            }
        }

        private void UpdateKnobPosition()
        {
            Vector3 position = _knobRect.localPosition;
            position.x = _xDeltaState * (_isEnable ? 1 : -1);
            _knobRect.localPosition = position;
        }
    }
}