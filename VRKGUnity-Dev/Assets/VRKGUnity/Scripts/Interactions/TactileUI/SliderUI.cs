using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace AIDEN.TactileUI
{
    public class SliderUI : BaseTouch, IValueUI<float>
    {
        public float Value
        {
            get
            {
                float v = Mathf.Lerp(_minValue, _maxValue, _value);

                if(_numericType == NumericType.Int)
                    v = Mathf.Round(v);

                return v;
            }
            set
            {
                _value = Mathf.InverseLerp(_minValue, _maxValue, value);
                UpdateVisuals();
            }
        }


        [SerializeField]
        RectTransform _sliderRectTf;

        [SerializeField]
        RectTransform _fillRectTf;

        [SerializeField]
        RectTransform _knobRectTf;

        [SerializeField]
        TMP_Text _label;

        [SerializeField]
        SliderType _sliderType;

        [SerializeField]
        bool _alwaysDisplayValue = false;

        [SerializeField]
        float _minValue = 0f;

        [SerializeField]
        float _maxValue = 1f;

        [SerializeField]
        NumericType _numericType;

        [SerializeField]
        float _startValue;

        [SerializeField, Space(10)]
        UnityEvent<float> _onValueChanged;

        float _value;

        bool _isMovingKnob;
        bool _isHorizontal;
        float _lengthSlider;
        float _hapticTime;

        private void Awake()
        {
            InitializeParameters();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _label.enabled = _alwaysDisplayValue;
            _isMovingKnob = false;
        }

        private void InitializeParameters()
        {
            _isHorizontal = _isHorizontal = (_sliderType == SliderType.LeftToRight || _sliderType == SliderType.RightToLeft);
            _lengthSlider = _isHorizontal ? _sliderRectTf.rect.width : _sliderRectTf.rect.height;

            _rebounceDelay = 0f;
        }

 
        protected override void TryActivate()
        {
            if (!base.CanActivate())
                return;

            base.Activate();


            _isMovingKnob = true;
            _label.enabled = true;

            _touchInter.ActivateHaptic(.05f, .08f);
            _hapticTime = Time.time + .5f;
            StartCoroutine(MovingSlider());
        }


        public override void TriggerExit(bool isProximity, Transform touchTf)
        {
            base.TriggerEnter(isProximity, touchTf);

            if(!isProximity)
            {
                _isMovingKnob = false;

                if (!_alwaysDisplayValue)
                    _label.enabled = false;
            }
        }

        IEnumerator MovingSlider()
        {
            while (_isMovingKnob)
            {
                RetrieveValueFromTouchPosition();
                UpdateVisuals();

                TryActivateHaptic();

                _onValueChanged?.Invoke(Mathf.Lerp(_minValue, _maxValue, Value));
                yield return null;
            }
        }

        private void RetrieveValueFromTouchPosition()
        {
            Vector3 sliderWorldPosition = _sliderRectTf.position;
            Plane plane = new(_sliderRectTf.forward, sliderWorldPosition);
            Vector3 worldProjectedPoint = plane.ClosestPointOnPlane(_touchTf.position);

            Vector3 localVector = _sliderRectTf.InverseTransformPoint(worldProjectedPoint);


            float positionFromVirtualAnchor = (_lengthSlider * .5f) + (_isHorizontal ? localVector.x : localVector.y);

            float value = Mathf.Clamp(positionFromVirtualAnchor / _lengthSlider, 0f, 1f);

            if (_sliderType == SliderType.RightToLeft || _sliderType == SliderType.TopToBottom)
                value = 1f - value;

            _value = value;
        }

        private void UpdateVisuals()
        {
            float positionFromVirtualAnchor = _lengthSlider * _value;

            if (_isHorizontal)
            {
                if(_sliderType == SliderType.LeftToRight)
                    _knobRectTf.anchoredPosition = new Vector3(positionFromVirtualAnchor, 0f);
                else
                    _knobRectTf.anchoredPosition = new Vector3(-positionFromVirtualAnchor, 0f);
            }
            else
            {
                if(_sliderType == SliderType.TopToBottom)
                    _knobRectTf.anchoredPosition = new Vector3(0f, -positionFromVirtualAnchor);
                else
                    _knobRectTf.anchoredPosition = new Vector3(0f, positionFromVirtualAnchor);
            }

            Vector2 sizeDelta = _fillRectTf.sizeDelta;

            if (_isHorizontal)
                sizeDelta.x = positionFromVirtualAnchor;
            else
                sizeDelta.y = positionFromVirtualAnchor;

            _fillRectTf.sizeDelta = sizeDelta;

            _label.text = Value.ToString("0.##");
        }

        private void TryActivateHaptic()
        {
            if (_hapticTime < Time.time)
                return;

            _touchInter.ActivateHaptic(.05f, .08f);
            _hapticTime = Time.time + .5f;
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            OnValidateSetSliderLayout();
            InitializeParameters();

            

            if (_numericType == NumericType.Int)
            {
                _minValue = Mathf.Round(_minValue);
                _maxValue = Mathf.Round(_maxValue);
            }

            Value = _startValue;
            _startValue = Value;

        }

        private void OnValidateSetSliderLayout()
        {
            switch (_sliderType)
            {
                case SliderType.LeftToRight:
                    SetNewAnchor(new Vector2(0, 0.5f));
                    break;
                case SliderType.RightToLeft:
                    SetNewAnchor(new Vector2(1, 0.5f));
                    break;
                case SliderType.BottomToTop:
                    SetNewAnchor(new Vector2(0.5f, 0));
                    break;
                case SliderType.TopToBottom:
                    SetNewAnchor(new Vector2(0.5f, 1));
                    break;
            }

            var size = _sliderRectTf.sizeDelta;

            if(_sliderType == SliderType.LeftToRight || _sliderType == SliderType.RightToLeft)
            {
                if (size.x > size.y)
                    return;
            }
            else
            {
                if (size.x < size.y)
                    return;  
            }

            (size.x, size.y) = (size.y, size.x);
            _sliderRectTf.sizeDelta = size;
            _fillRectTf.sizeDelta = size;

            _sliderRectTf.ForceUpdateRectTransforms();

            void SetNewAnchor(Vector2 anchor)
            {
                _fillRectTf.anchorMin = anchor;
                _fillRectTf.anchorMax = anchor;
                _fillRectTf.pivot = anchor;

                _knobRectTf.anchorMin = anchor;
                _knobRectTf.anchorMax = anchor;
            }
        }
#endif

        public enum SliderType
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom
        }


    }
}