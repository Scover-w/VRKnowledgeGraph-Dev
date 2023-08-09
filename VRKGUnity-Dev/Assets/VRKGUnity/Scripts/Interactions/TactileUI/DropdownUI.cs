using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
                if (_selectedValue == null)
                    return "";

                return _selectedValue.Value;
            }
            set
            {
                SetNewValueFromOutside(value);
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveGraphicUI> _interactiveGraphics;

        [SerializeField]
        GameObject _itemsContainerGo;

        [SerializeField]
        GameObject _itemTemplateGo;

        [SerializeField]
        List<DropDownItemValue> _startItemValues;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        GameObject _interactionCollidersGo;

        [SerializeField, Tooltip("Colliders under the dropdown to hide to prevent interaction")]
        List<Collider> _colliderToHide;

        [SerializeField]
        int _startSelectedValue = -1;

        [SerializeField, Space(10)]
        UnityEvent<string> _onValueChanged;

        Dictionary<DropDownItemValue, DropdownItemUI> _items;

        Transform _touchTf;
        TouchInteractor _touchInter;

        DropdownItemUI _selectedItemUI;
        DropDownItemValue _selectedValue;

        RectTransform _itemContainerRectTf;

        InteractionStateUI _interactionStateUI;

        bool _isOpen = false;
        bool _canClick = true;

        float _heightItem;


        private void Awake()
        {
            _itemContainerRectTf = _itemsContainerGo.GetComponent<RectTransform>();
            _heightItem = _itemTemplateGo.GetComponent<RectTransform>().rect.height;

            CreateItems();
        }

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

            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);
        }

        private void CreateItems()
        {
            _items = new();

            if (_startItemValues == null || _startItemValues.Count == 0)
                return;

            bool hasSelectedOneItem = false;

            int nb = -1;

            foreach (DropDownItemValue itemValue in _startItemValues)
            {
                DropdownItemUI itemUI = CreateItemUI(itemValue);

                nb++;

                if (nb != _startSelectedValue)
                    continue;

                hasSelectedOneItem = true;
                SelectItem(itemValue, itemUI);
            }

            _itemTemplateGo.SetActive(false);
            UpdateContentLayout();


            if (hasSelectedOneItem)
                return;

            if (_items.Count == 0)
                return;

            var item = _items.First();
            SelectItem(item.Key, item.Value);

        }

        private DropdownItemUI CreateItemUI(DropDownItemValue itemValue)
        {
            var itemGo = Instantiate(_itemTemplateGo, _itemsContainerGo.transform);
            DropdownItemUI itemUI = itemGo.GetComponent<DropdownItemUI>();
            _items.Add(itemValue, itemUI);
            itemUI.SetDropDownValue(itemValue);
            itemGo.SetActive(true);

            return itemUI;
        }

        private void SelectItem(DropDownItemValue itemValue, DropdownItemUI itemUI = null)
        {
            if(itemUI == null && !_items.TryGetValue(itemValue, out itemUI))
            {
                Debug.LogError("Couldn't select item with the provided itemValue : " + itemValue.Value);
                return;
            }


            UnselectSelected();

            _selectedItemUI = itemUI;
            _selectedValue = itemValue;

            itemUI.IsSelected = true;
            _label.text = itemValue.LabelName;
        }

        private void SetNewValueFromOutside(string newValue)
        {
            if(!TryGetValueFromOptions(newValue, out KeyValuePair<DropDownItemValue, DropdownItemUI> item))
            {
                Debug.LogWarning("No DropDownValueItem with this value : " + newValue);
                return;
            }

            SelectItem(item.Key, item.Value);
        }

        private bool TryGetValueFromOptions(string value, out KeyValuePair<DropDownItemValue, DropdownItemUI> itemToGet)
        {
            foreach(var item in _items)
            {
                if(item.Key.Value == value)
                {
                    itemToGet = item;
                    return true;
                }
            }

            itemToGet = default;

            return false;
        }

        public void AddOption(DropDownItemValue newDdValue)
        {
            if (TryGetValueFromOptions(newDdValue.Value, out var item))
            {
                Debug.LogWarning("DropDownValueItem Value already in newDdValue.Value");
                return;
            }

            CreateItemUI(newDdValue);
            UpdateContentLayout();
        }

        public void RemoveOption(string value)
        {
            if (!TryGetValueFromOptions(value, out var item))
            {
                Debug.LogWarning("No DropDownValueItem with this value : " + value);
                return;
            }

            var itemValue = item.Key;
            var itemUI = item.Value;

            Destroy(itemUI.gameObject);
            _items.Remove(itemValue);

            if (_selectedValue != itemValue)
                return;

            if (_items.Count == 0)
                return;

            var newItem = _items.First();
            SelectItem(newItem.Key, newItem.Value);

            _onValueChanged?.Invoke(newItem.Key.Value);
        }

        private void UpdateContentLayout()
        {
            int nbItem = _items.Count;

            _itemContainerRectTf.sizeDelta = new Vector2(_itemContainerRectTf.sizeDelta.x, _heightItem * nbItem);
        }

        private void UnselectSelected()
        {
            if (_selectedItemUI == null)
                return;

            _selectedItemUI.IsSelected = false;
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
            _touchInter.ActivateHaptic();

            EnableCollidersToHide(false);

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

        public void NewValueFromItem(DropDownItemValue itemValue)
        {
            SelectItem(itemValue);

            _isOpen = false;
            _itemsContainerGo.SetActive(false);
            EnableCollidersToHide(true);


            _onValueChanged?.Invoke(itemValue.Value);
        }

        private void UpdateInteractionColor()
        {
            _interactiveGraphics.UpdateColor(_interactionStateUI);
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            OnValidateStartSelectedValue();

            _interactiveGraphics?.TrySetName();

            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }

        private void OnValidateStartSelectedValue()
        {
            if (_startSelectedValue < 0)
                _startSelectedValue = 0;

            if (_startSelectedValue >= _startItemValues.Count)
            {
                _startSelectedValue = 0;
            }

            if (_startItemValues.Count != 0)
            {
                _label.text = _startItemValues[_startSelectedValue].LabelName;
            }
        }
#endif
    }

    [Serializable]
    public class DropDownItemValue
    {
        public string LabelName;
        public string Value;
    }
}