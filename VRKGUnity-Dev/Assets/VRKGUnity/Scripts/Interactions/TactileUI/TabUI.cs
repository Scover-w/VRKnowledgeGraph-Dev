using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class TabUI : MonoBehaviour, ITouchUI
    {
        [SerializeField]
        TabControllerUI _controllerUI;

        [SerializeField]
        GameObject _pageToDisplayGo;

        [SerializeField]
        List<Image> _interactiveImgs;

        Transform _touchTf;
        TouchInteractor _touchInter;

        List<InteractiveColorUI> _currentColorStates;

        InteractionStateUI _interactionState;

        bool _canClick = true;

        private void OnEnable()
        {
            _interactionState = InteractionStateUI.Normal;
        }

        public void Select(List<InteractiveColorUI> colorStates)
        {
            _currentColorStates = colorStates;
            _pageToDisplayGo.SetActive(true);
            UpdateInteractionColor();
        }

        public void UnSelect(List<InteractiveColorUI> colorStates)
        {
            _currentColorStates = colorStates;
            _pageToDisplayGo.SetActive(false);
            UpdateInteractionColor();
        }

        private void UpdateInteractionColor()
        {
            int nbImage = _interactiveImgs.Count;

            for (int i = 0; i < nbImage; i++)
            {
                Image img = _interactiveImgs[i];
                InteractiveColorUI colorTab = _currentColorStates[i];

                switch (_interactionState)
                {
                    case InteractionStateUI.Normal:
                        img.color = colorTab.NormalColor;
                        break;
                    case InteractionStateUI.InProximity:
                        img.color = colorTab.ProximityColor;
                        break;
                    case InteractionStateUI.Active:
                        img.color = colorTab.ActivatedColor;
                        break;
                }
            }
        }


        public void TriggerEnter(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                _touchTf = touchTf;
                _touchInter = _touchTf.GetComponent<TouchInteractor>();
                _interactionState = InteractionStateUI.InProximity;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                TryClick();
            }

        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                _canClick = true;
                _interactionState = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                if (_touchInter != null && !_canClick)
                    _touchInter.ActiveBtn(false, this);

                _interactionState = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }

        }

        private void TryClick()
        {
            if (!_canClick)
                return;

            _canClick = false;
            _interactionState = InteractionStateUI.Active;

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _controllerUI.Select(this);
        }
    }
}