using System;
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

                UpdateColliderActivation();
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
                // TODO : Check if in value list
                _value = value;
                _label.text = _value;
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveGraphicUI> _interactiveGraphics;

        [SerializeField]
        GameObject _itemsContainerGo;

        [SerializeField]
        DropdownItemUI _itemTemplate;

        [SerializeField]
        List<DropDownItem> _dropdowns;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        GameObject _interactionCollidersGo;

        [SerializeField, Tooltip("Colliders under the dropdown to hide to prevent interaction")]
        List<Collider> _colliderToHide;

        [SerializeField]
        string _value;

        [SerializeField, Space(10)]
        UnityEvent<string> _onValueChanged;

        List<DropdownItemUI> _items;

        Transform _touchTf;
        TouchInteractor _touchInter;


        InteractionStateUI _interactionStateUI;

        bool _isOpen = false;
        bool _canClick = true;


        private void OnEnable()
        {
            _isOpen = false;
            _itemsContainerGo.SetActive(_isOpen);

            UpdateColliderActivation();
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
            _itemsContainerGo.SetActive(_isOpen);

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
            _itemsContainerGo.SetActive(false);
            EnableCollidersToHide(true);


            _onValueChanged?.Invoke(_value);
        }

        private void UpdateInteractionColor()
        {
            _interactiveGraphics.UpdateColor(_interactionStateUI);
        }

        private void RefreshValues()
        {
            foreach (var dropdown in _items)
            {
                dropdown.ResfreshValue(_value);
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
            _label.text = _value;

            _interactiveGraphics?.TrySetName();

            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
    }

    [Serializable]
    public class DropDownItem
    {
        public string LabelName;
        public string Value;
    }
}