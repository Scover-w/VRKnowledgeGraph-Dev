using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class TabUI : BaseTouch
    {
        [SerializeField]
        TabControllerUI _controllerUI;

        [SerializeField]
        GameObject _pageToDisplayGo;

        [SerializeField, Header("With TabUI, don't fill the _interactiveGraphics, TabUIController take care of it ")]
        List<Graphic> _interactiveGraphicToFill;

        List<InteractiveColorUI> _interactiveColors;

        protected override void OnEnable()
        {
            _inProximity = false;

            Invoke(nameof(DelayedOnEnable), .2f);   
        }

        private void DelayedOnEnable()
        {
           base.OnEnable();
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

        protected override void UpdateInteractionColor()
        {
            int nbGraphics = _interactiveGraphicToFill.Count;

            for (int i = 0; i < nbGraphics; i++)
            {
                Graphic graphic = _interactiveGraphicToFill[i];
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


        protected override void TryActivate()
        {
            if (!base.CanActivate())
                return;

            base.Activate();

            if (_touchInter != null)
                _touchInter.ActivateHaptic();

            _controllerUI.Select(this);
        }
    }
}