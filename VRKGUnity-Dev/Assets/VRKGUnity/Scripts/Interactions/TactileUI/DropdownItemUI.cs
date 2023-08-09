using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class DropdownItemUI : MonoBehaviour, ITouchUI
    {

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                UpdateSelectionEffect();
            }
        }

        [SerializeField]
        DropdownUI _controller;

        [SerializeField]
        List<InteractiveGraphicUI> _interactiveGraphics;

        [SerializeField]
        List<InteractiveGraphicUI> _selectedInteractiveGraphics;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        GameObject _checkMarkGo;

        DropDownItemValue _value;

        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;

        bool _isSelected = false;

        private void OnEnable()
        {
            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();
        }

        private void OnDisable()
        {
            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);
        }

        public void SetDropDownValue(DropDownItemValue ddValue)
        {
            _value = ddValue;
            _label.text = ddValue.LabelName;

            UpdateSelectionEffect();
        }

        private void UpdateSelectionEffect()
        {
            _checkMarkGo.SetActive(_isSelected);
            UpdateInteractionColor();
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
            _interactionStateUI = InteractionStateUI.Active;
            _touchInter.ActivateHaptic();

            UpdateInteractionColor();

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _controller.NewValueFromItem(_value);
        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                if (_touchInter != null)
                    _touchInter.ActiveBtn(false, this);

                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
        }

        private void UpdateInteractionColor()
        {
            if(_isSelected)
                _interactiveGraphics.UpdateColor(_interactionStateUI);
            else
                _selectedInteractiveGraphics.UpdateColor(_interactionStateUI);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _interactiveGraphics?.TrySetName();
        }
#endif
    }
}