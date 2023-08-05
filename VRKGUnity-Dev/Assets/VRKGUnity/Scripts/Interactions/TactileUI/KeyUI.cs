using TMPro;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class KeyUI : MonoBehaviour, ITouchUI, IValueUI<char>
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

        public char Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _label.text = value.ToString();
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        InteractiveColorUI _interactiveColor;

        [SerializeField]
        Image _interactiveImg;

        [SerializeField]
        KeyboardUI _keyboardUI;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        GameObject _interactionCollidersGo;

        [SerializeField]
        char _value;

        [SerializeField]
        UnityEvent<char> _onKey;

        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;

        bool _canClick = true;

        private void OnEnable()
        {
            UpdateColliderActivation();
            TrySetNormalInteractionState();

            ToLower();
            UpdateInteractionColor();
        }

        public void ToLower()
        {
            _label.fontStyle = FontStyles.LowerCase;
        }

        public void ToUpper()
        {
            _label.fontStyle = FontStyles.UpperCase;
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
            _interactionStateUI = InteractionStateUI.Active;
            UpdateInteractionColor();

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            _onKey?.Invoke(_value);
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

        private void UpdateInteractionColor()
        {
            switch (_interactionStateUI)
            {
                case InteractionStateUI.Normal:
                    _interactiveImg.color = _interactiveColor.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    _interactiveImg.color = _interactiveColor.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    _interactiveImg.color = _interactiveColor.ActivatedColor;
                    break;
                case InteractionStateUI.Disabled:
                    _interactiveImg.color = _interactiveColor.DisabledColor;
                    break;
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
            _label.text = _value.ToString();

            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
    }
}