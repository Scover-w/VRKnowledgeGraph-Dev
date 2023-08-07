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
        List<Graphic> _interactiveGraphics;

        List<InteractiveColorUI> _interactiveColors;

        [SerializeField]
        GameObject _interactionCollidersGo;

        Transform _touchTf;
        TouchInteractor _touchInter;

        

        InteractionStateUI _interactionStateUI;

        bool _canClick = true;

        private void OnEnable()
        {
            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }

        private void OnDisable()
        {
            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);
        }

        public void Select(List<InteractiveColorUI> colorStates)
        {
            _interactiveColors = colorStates;
            _pageToDisplayGo.SetActive(true);
            UpdateInteractionColor();
        }

        public void UnSelect(List<InteractiveColorUI> colorStates)
        {
            _interactiveColors = colorStates;
            _pageToDisplayGo.SetActive(false);
            UpdateInteractionColor();
        }

        private void UpdateInteractionColor()
        {
            int nbGraphics = _interactiveGraphics.Count;

            for (int i = 0; i < nbGraphics; i++)
            {
                Graphic graphic = _interactiveGraphics[i];
                InteractiveColorUI colorTab = _interactiveColors[i];

                switch (_interactionStateUI)
                {
                    case InteractionStateUI.Normal:
                        graphic.color = colorTab.NormalColor;
                        break;
                    case InteractionStateUI.InProximity:
                        graphic.color = colorTab.ProximityColor;
                        break;
                    case InteractionStateUI.Active:
                        graphic.color = colorTab.ActivatedColor;
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