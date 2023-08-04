using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class DropdownUI : MonoBehaviour, ITouchUI, IValueUI<string>
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

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveColorUI> _interactiveColors;

        [SerializeField]
        InteractiveColorUI _interactiveColorItems;

        [SerializeField]
        List<Image> _interactiveImgs;

        [SerializeField]
        GameObject _optionsContainerGo;

        [SerializeField]
        List<DropdownItemUI> _dropdowns;

        [SerializeField]
        TMP_Text _label;

        [SerializeField, Tooltip("Colliders under the dropdown to hide to prevent interaction")]
        List<Collider> _colliderToHide;

        [SerializeField]
        string _value;

        [SerializeField, Space(10)]
        UnityEvent<string> _onValueChanged;

        Transform _touchTf;
        TouchInteractor _touchInter;


        InteractionStateUI _interactionStateUI;

        bool _isOpen = false;
        bool _canClick = true;


        private void OnEnable()
        {
            _isOpen = false;
            _optionsContainerGo.SetActive(_isOpen);

            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }

        private void OnDisable()
        {
            if(_isOpen)
                EnableCollidersToHide(true);
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
            if (!_canClick)
                return;

            _canClick = false;
            _isOpen = !_isOpen;
            _optionsContainerGo.SetActive(_isOpen);

            EnableCollidersToHide(false);

            if (_isOpen)
                RefreshValues();

            _interactionStateUI = InteractionStateUI.Active;
            UpdateInteractionColor();

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);
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

        public void CloseFromDropdown(string value)
        {
            _label.text = value;
            _value = value;

            RefreshValues();

            _isOpen = false;
            _optionsContainerGo.SetActive(false);
            EnableCollidersToHide(true);


            _onValueChanged?.Invoke(_value);
        }

        private void UpdateInteractionColor()
        {
            int nbImg = _interactiveImgs.Count;

            for (int i = 0; i < nbImg; i++)
            {
                Image img = _interactiveImgs[i];
                InteractiveColorUI colorState = _interactiveColors[i];

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

        private void RefreshValues()
        {
            foreach (var dropdown in _dropdowns)
            {
                dropdown.ResfreshValue(_value, _interactiveColorItems);
            }
        }

        private void EnableCollidersToHide(bool enable) 
        {
            if (_colliderToHide == null)
                return;

            foreach (Collider collider in _colliderToHide) 
            { 
                collider.enabled = enable;
            }
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