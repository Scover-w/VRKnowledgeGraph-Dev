using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class ButtonUI : MonoBehaviour, ITouchUI
    {
        [SerializeField]
        List<ColorStateUI> _colorStates;

        [SerializeField]
        List<Image> _imgs;

        [SerializeField, Space(10)]
        UnityEvent _onClick;

        Transform _touchTf;
        TouchInteractor _touchInter;

        bool _canClick = true;

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
            if (!_canClick)
                return;

            _canClick = false;
            UpdateColor(InteractionStateUI.Active);

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _onClick?.Invoke();
        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                _canClick = true;
                UpdateColor(InteractionStateUI.Normal);
            }
            else if (!isProximity)
            {
                if (_touchInter != null && !_canClick)
                    _touchInter.ActiveBtn(false, this);

                UpdateColor(InteractionStateUI.Normal);
            }

        }

        private void UpdateColor(InteractionStateUI interactionState)
        {
            int nbImg = _imgs.Count;

            for (int i = 0; i < nbImg; i++)
            {
                Image img = _imgs[i];
                ColorStateUI colorState = _colorStates[i];

                switch (interactionState)
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
                }
            }
        }
    }

}

