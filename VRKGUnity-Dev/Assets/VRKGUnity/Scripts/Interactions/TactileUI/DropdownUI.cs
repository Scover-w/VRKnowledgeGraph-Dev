using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class DropdownUI : MonoBehaviour, ITactileUI
    {
        [SerializeField]
        List<ColorStateUI> _colorDropdown;

        [SerializeField]
        ColorStateUI _colorItem;

        [SerializeField]
        List<Image> _imgs;

        [SerializeField]
        GameObject _optionsContainerGo;

        [SerializeField]
        List<DropdownItemUI> _dropdowns;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        string _selectedValue;

        Transform _touchTf;
        TouchInteractor _touchInter;

        bool _isOpen = false;
        bool _canClick = true;


        private void OnEnable()
        {
            _isOpen = false;
            _optionsContainerGo.SetActive(_isOpen);
            UpdateColor(InteractionStateUI.Normal);
        }

        public void TriggerEnter(bool isProximity, Collider touchCollider)
        {
            if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
            {
                _touchTf = touchCollider.transform.parent;
                _touchInter = _touchTf.GetComponent<TouchInteractor>();
                UpdateColor(InteractionStateUI.InProximity);
            }
            else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
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

            if (_isOpen)
                RefreshValues();

            UpdateColor(InteractionStateUI.Active);

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);
        }

        public void TriggerExit(bool isProximity, Collider touchCollider)
        {
            if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
            {
                _canClick = true;
                UpdateColor(InteractionStateUI.Normal);
            }
            else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
            {
                if (_touchInter != null && !_canClick)
                    _touchInter.ActiveBtn(false, this);

                UpdateColor(InteractionStateUI.Normal);
            }
        }

        public void CloseFromDropdown(string value)
        {
            _label.text = value;
            _selectedValue = value;

            RefreshValues();

            _isOpen = false;
            _optionsContainerGo.SetActive(false);


            // TODO : Link the the true datas
            Debug.Log("Select " + value);
        }

        private void UpdateColor(InteractionStateUI interactionState)
        {
            int nbImg = _imgs.Count;

            for (int i = 0; i < nbImg; i++)
            {
                Image img = _imgs[i];
                ColorStateUI colorState = _colorDropdown[i];

                switch (interactionState)
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
                }
            }
        }

        private void RefreshValues()
        {
            foreach (var dropdown in _dropdowns)
            {
                dropdown.ResfreshValue(_selectedValue, _colorItem);
            }
        }
    }
}