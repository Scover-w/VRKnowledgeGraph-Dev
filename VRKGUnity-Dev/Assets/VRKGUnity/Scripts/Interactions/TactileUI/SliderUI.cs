using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Wave.Essence.Hand.NearInteraction;

namespace AIDEN.TactileUI
{
    public class SliderUI : MonoBehaviour, ITouchUI, IValueUI<float>
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

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = Mathf.Clamp(value, _minValue, _maxValue);

                if(_wholeNumber)
                    _value = Mathf.Round(_value);

                UpdateVisuals();
            }
        }

        private float UnNormalizedValue
        {
            get
            {
                return Mathf.Lerp(_minValue, _maxValue, _value);
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        List<InteractiveColorUI> _interactiveColors;

        [SerializeField]
        List<Image> _interactiveImgs;

        [SerializeField]
        RectTransform _sliderRectTf;

        [SerializeField]
        RectTransform _sliderFilledRectTf;

        [SerializeField]
        RectTransform _knobRectTf;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        GameObject _interactionCollidersGo;

        [SerializeField]
        SliderType _sliderType;

        [SerializeField]
        bool _alwaysDisplayValue = false;

        [SerializeField]
        float _minValue = 0f;

        [SerializeField]
        float _maxValue = 1f;

        [SerializeField]
        bool _wholeNumber = false;

        [SerializeField]
        float _value;

        [SerializeField, Space(10)]
        UnityEvent<float> _onValueChanged;

        InteractionStateUI _interactionStateUI;

        Transform _touchTf;
        TouchInteractor _touchInter;

        private bool _isMovingKnob = false;
        bool _isWidth;
        float _lengthSlider;


        private void OnEnable()
        {
            _label.enabled = _alwaysDisplayValue;
            _isWidth = _sliderType == SliderType.Horizontal;
            _lengthSlider = _isWidth ? _sliderRectTf.rect.width : _sliderRectTf.rect.height;

            UpdateColliderActivation();
            TrySetNormalInteractionState();
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
                _interactionStateUI = InteractionStateUI.Active;
                UpdateInteractionColor();

                _isMovingKnob = true;
                _label.enabled = true;

                if (_touchInter != null)
                    _touchInter.ActiveBtn(true, this);

                StartCoroutine(MovingSlider());
            }
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

                _isMovingKnob = false;

                if (!_alwaysDisplayValue)
                    _label.enabled = false;

                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
        }

        private void UpdateInteractionColor()
        {
            int nbImage = _interactiveImgs.Count;

            for (int i = 0; i < nbImage; i++)
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

        IEnumerator MovingSlider()
        {
            while (_isMovingKnob)
            {
                RetrieveValue();
                UpdateVisuals();

                _onValueChanged?.Invoke(Mathf.Lerp(_minValue, _maxValue, _value));
                yield return null;
            }
        }

        private void RetrieveValue()
        {
            Vector3 sliderWorldPosition = _sliderRectTf.position;
            Plane plane = new(_sliderRectTf.forward, sliderWorldPosition);
            Vector3 worldProjectedPoint = plane.ClosestPointOnPlane(_touchTf.position);

            Vector3 localVector = _sliderRectTf.InverseTransformPoint(worldProjectedPoint);


            float positionFromVirtualAnchor = (_lengthSlider * .5f) + (_isWidth ? localVector.x : localVector.y);

            float value = positionFromVirtualAnchor / _lengthSlider;

            if (value < 0f)
                value = 0f;
            else if (value > 1f)
                value = 1f;

            _value = value;

            if(_wholeNumber)
                _value = Mathf.Round(_value);
        }

        private void UpdateVisuals()
        {
            float positionFromVirtualAnchor = _lengthSlider * _value;

            if (_isWidth)
                _knobRectTf.localPosition = new Vector3(positionFromVirtualAnchor - _lengthSlider * .5f, 0f, 0f);
            else
                _knobRectTf.localPosition = new Vector3(0f, positionFromVirtualAnchor - _lengthSlider * .5f, 0f);

            Vector2 sizeDelta = _sliderFilledRectTf.sizeDelta;

            if (_isWidth)
                sizeDelta.x = positionFromVirtualAnchor;
            else
                sizeDelta.y = positionFromVirtualAnchor;

            _sliderFilledRectTf.sizeDelta = sizeDelta;

            _label.text = UnNormalizedValue.ToString("0.##");
        }

        private void TrySetNormalInteractionState()
        {
            if (_interactable)
                _interactionStateUI = InteractionStateUI.Normal;
            else
                _interactionStateUI = InteractionStateUI.Disabled;
        }

        private void UpdateColliderActivation()
        {
            _interactionCollidersGo.SetActive(_interactable);
        }


        private void OnValidate()
        {
            _isWidth = _sliderType == SliderType.Horizontal;
            _lengthSlider = _isWidth ? _sliderRectTf.rect.width : _sliderRectTf.rect.height;

            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateVisuals();
        }

        public enum SliderType
        {
            Horizontal,
            Vertical
        }


    }
}