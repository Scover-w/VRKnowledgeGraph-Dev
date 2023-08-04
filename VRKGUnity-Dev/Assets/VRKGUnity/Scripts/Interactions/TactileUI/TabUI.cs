using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class TabUI : MonoBehaviour, ITactileUI
    {
        [SerializeField]
        TabControllerUI _controllerUI;

        [SerializeField]
        GameObject _pageToDisplayGo;

        [SerializeField]
        List<Image> _imgs;

        Transform _touchTf;
        TouchInteractor _touchInter;

        List<ColorStateUI> _currentColorStates;

        InteractionStateUI _interactionState;

        bool _canClick = true;

        private void OnEnable()
        {
            _interactionState = InteractionStateUI.Normal;
        }

        public void Select(List<ColorStateUI> colorStates)
        {
            _currentColorStates = colorStates;
            _pageToDisplayGo.SetActive(true);
            UpdateColor();
        }

        public void UnSelect(List<ColorStateUI> colorStates)
        {
            _currentColorStates = colorStates;
            _pageToDisplayGo.SetActive(false);
            UpdateColor();
        }

        private void UpdateColor()
        {
            int nbImage = _imgs.Count;

            for (int i = 0; i < nbImage; i++)
            {
                Image img = _imgs[i];
                ColorStateUI colorTab = _currentColorStates[i];

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


        public void TriggerEnter(bool isProximity, Collider touchCollider)
        {
            if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
            {
                _touchTf = touchCollider.transform.parent;
                _touchInter = _touchTf.GetComponent<TouchInteractor>();
                _interactionState = InteractionStateUI.InProximity;
                UpdateColor();
            }
            else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
            {
                TryClick();
            }

        }

        public void TriggerExit(bool isProximity, Collider touchCollider)
        {
            if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
            {
                _canClick = true;
                _interactionState = InteractionStateUI.Normal;
                UpdateColor();
            }
            else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
            {
                if (_touchInter != null && !_canClick)
                    _touchInter.ActiveBtn(false, this);

                _interactionState = InteractionStateUI.Normal;
                UpdateColor();
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