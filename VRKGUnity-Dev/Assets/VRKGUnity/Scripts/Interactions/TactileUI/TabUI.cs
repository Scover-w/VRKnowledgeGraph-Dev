using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wave.Essence.Hand.NearInteraction;

namespace AIDEN.TactileUI
{
    public class TabUI : MonoBehaviour, ITouchUI
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

                UpdateColliderActivation();
                TrySetNormalInteractionState();
                UpdateInteractionColor();
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        TabControllerUI _controllerUI;

        [SerializeField]
        GameObject _pageToDisplayGo;

        [SerializeField]
        List<Image> _interactiveImgs;

        [SerializeField]
        GameObject _interactionCollidersGo;

        Transform _touchTf;
        TouchInteractor _touchInter;

        List<InteractiveColorUI> _currentColorStates;

        InteractionStateUI _interactionStateUI;

        bool _canClick = true;

        private void OnEnable()
        {
            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
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

                switch (_interactionStateUI)
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
                _interactionStateUI = InteractionStateUI.InProximity;
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
                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                if (_touchInter != null && !_canClick)
                    _touchInter.ActiveBtn(false, this);

                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }

        }

        private void TryClick()
        {
            if (!_canClick)
                return;

            _canClick = false;
            _interactionStateUI = InteractionStateUI.Active;

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _controllerUI.Select(this);
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

        private void OnValidate()
        {
            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
    }
}