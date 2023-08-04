using AngleSharp.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class KeyUI : MonoBehaviour, ITactileUI
    {
        [SerializeField]
        ColorStateUI _color;

        [SerializeField]
        Image _img;

        [SerializeField]
        KeyboardUI _keyboardUI;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        char _value;

        [SerializeField, Space(10)]
        UnityEvent _onClick;

        [SerializeField]
        UnityEvent<char> _onKey;

        Transform _touchTf;
        TouchInteractor _touchInter;

        bool _canClick = true;

        public void SetValue(char value)
        {
            _value = value;

            if (_label != null)
                _label.text = value.ToString();
        }

        public void ToLower()
        {
            if (_value.IsLetter())
                _value = char.ToLower(_value);
        }

        public void ToUpper()
        {
            if (_value.IsLetter())
                _value = char.ToUpper(_value);
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
            UpdateColor(InteractionStateUI.Active);

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _onClick?.Invoke();
            _onKey?.Invoke(_value);
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

        private void UpdateColor(InteractionStateUI interactionState)
        {
            switch (interactionState)
            {
                case InteractionStateUI.Normal:
                    _img.color = _color.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    _img.color = _color.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    _img.color = _color.ActivatedColor;
                    break;
            }
        }
    }
}