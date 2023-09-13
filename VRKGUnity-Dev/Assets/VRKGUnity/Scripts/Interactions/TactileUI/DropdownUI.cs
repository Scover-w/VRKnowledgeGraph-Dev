using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AIDEN.TactileUI
{
    public class DropdownUI : BaseTouch, IValueUI<int>
    {
        public int Value
        {
            get
            {
                if (_selectedValue == null)
                    return -1;

                return _selectedValue.Value;
            }
            set
            {
                SetNewValueFromOutside(value);
            }
        }

        [SerializeField]
        GameObject _itemsContainerGo;

        [SerializeField]
        GameObject _itemTemplateGo;

        [SerializeField]
        List<DropDownItemValue> _startItemValues;

        [SerializeField]
        TMP_Text _label;


        [SerializeField, Tooltip("Colliders under the dropdown to hide to prevent interaction")]
        List<Collider> _colliderToHide;

        [SerializeField]
        int _startSelectedValueId = 0;

        [SerializeField, Space(10)]
        UnityEvent<int> _onValueChanged;

        Dictionary<DropDownItemValue, DropdownItemUI> _items;


        DropdownItemUI _selectedItemUI;
        DropDownItemValue _selectedValue;

        RectTransform _itemContainerRectTf;


        bool _isOpen = false;

        float _delayCollider = 1f;
        float _lastEnableColliderTime = 0f;
        float _lastDisableColliderTime = 0f;
        float _heightItem;

        private void Awake()
        {
            _itemContainerRectTf = _itemsContainerGo.GetComponent<RectTransform>();
            _heightItem = _itemTemplateGo.GetComponent<RectTransform>().rect.height;

            CreateItems();
        }

        protected override void OnEnable()
        {
            base.OnEnable();   

            _isOpen = false;
            _itemsContainerGo.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if(_isOpen)
                EnableCollidersToHide(true);
        }

        private void CreateItems()
        {
            _items = new();

            if (_startItemValues == null || _startItemValues.Count == 0)
                return;

            bool hasSelectedOneItem = false;

            int id = -1;

            foreach (DropDownItemValue itemValue in _startItemValues)
            {
                DropdownItemUI itemUI = CreateItemUI(itemValue);

                id++;

                itemValue.Value = id;

                if (id != _startSelectedValueId)
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

        private void SetNewValueFromOutside(int newValue)
        {
            if(!TryGetValueFromOptions(newValue, out KeyValuePair<DropDownItemValue, DropdownItemUI> item))
            {
                Debug.LogWarning("No DropDownValueItem with this value : " + newValue);
                return;
            }

            SelectItem(item.Key, item.Value);
        }

        private bool TryGetValueFromOptions(int value, out KeyValuePair<DropDownItemValue, DropdownItemUI> itemToGet)
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

        public void RemoveOption(int value)
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


        protected override void TryActivate()
        {
            if (!base.CanActivate())
                return;

            base.Activate();

            _isOpen = !_isOpen;
            _itemsContainerGo.SetActive(_isOpen);
            _touchInter.ActivateHaptic();

            EnableCollidersToHide(!_isOpen);
        }

        public void NewValueFromItem(DropDownItemValue itemValue)
        {
            SelectItem(itemValue);

            _isOpen = false;
            _itemsContainerGo.SetActive(false);
            EnableCollidersToHide(true);


            _onValueChanged?.Invoke(itemValue.Value);
        }


        private void EnableCollidersToHide(bool enable) 
        {
            if (_colliderToHide == null)
                return;

            if(enable)
            {
                _lastEnableColliderTime = Time.time; 
                Invoke(nameof(DisplayColliders), _delayCollider);
                return;
            }

            _lastDisableColliderTime = Time.time;

            foreach (Collider collider in _colliderToHide) 
            { 
                collider.enabled = false;
            }
        }

        /// <summary>
        /// Don't use it ! Only available for EnableCollidersToHide fn
        /// </summary>
        void DisplayColliders()
        {
            DebugDev.Log("DisplayColliders " + (_lastEnableColliderTime < _lastDisableColliderTime));
            if (_lastEnableColliderTime < _lastDisableColliderTime)
                return;

            foreach (Collider collider in _colliderToHide)
            {
                collider.enabled = true;
            }
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            OnValidateStartSelectedValue();
        }

        private void OnValidateStartSelectedValue()
        {
            if (_startSelectedValueId < 0)
                _startSelectedValueId = 0;

            if (_startSelectedValueId >= _startItemValues.Count)
            {
                _startSelectedValueId = 0;
            }

            if (_startItemValues.Count != 0)
            {
                _label.text = _startItemValues[_startSelectedValueId].LabelName;
            }
        }
#endif
    }

    [Serializable]
    public class DropDownItemValue
    {
        public string LabelName;
        [HideInInspector]
        public int Value;
    }
}