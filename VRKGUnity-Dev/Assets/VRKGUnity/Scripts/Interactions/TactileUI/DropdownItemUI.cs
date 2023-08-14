using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AIDEN.TactileUI
{
    public class DropdownItemUI : BaseTouch
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
        List<InteractiveGraphicUI> _selectedInteractiveGraphics;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        GameObject _checkMarkGo;


        DropDownItemValue _value;

        bool _isSelected = false;

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

        protected override void TryActivate()
        {
            if(!CanActivate()) 
                return;

            _touchInter.ActivateHaptic();
            _controller.NewValueFromItem(_value);
        }


        protected override void UpdateInteractionColor()
        {
            if(_isSelected)
                _interactiveGraphics.UpdateColor(_interactionStateUI);
            else
                _selectedInteractiveGraphics.UpdateColor(_interactionStateUI);
        }
    }
}