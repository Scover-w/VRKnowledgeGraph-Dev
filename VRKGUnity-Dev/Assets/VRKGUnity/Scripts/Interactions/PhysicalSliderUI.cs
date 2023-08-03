using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhysicalSliderUI : MonoBehaviour, IPhysicalUI
{
    [SerializeField]
    List<ColorStateUI> _colors;

    [SerializeField]
    List<Image> _imgs;

    [SerializeField]
    RectTransform _sliderRectTf;

    [SerializeField]
    RectTransform _sliderFilledRectTf;

    [SerializeField]
    RectTransform _knobRectTf;

    [SerializeField]
    TMP_Text _label;

    [SerializeField]
    SliderType _sliderType;

    [SerializeField]
    bool _alwaysDisplayValue = false;

    [SerializeField]
    [Range(0f, 1f)]
    float _value = .5f;

    [SerializeField]
    float _minValue = 0f;
    [SerializeField]
    float _maxValue = 1f;

    [SerializeField,Space(10)]
    UnityEvent<float> _onChangedValue;
    

    Transform _touchTf;
    TouchInteraction _touchInter;

    private bool _isMovingKnob = false;
    bool _isWidth;
    float _lengthSlider;

    private void Start()
    {
        _label.enabled = _alwaysDisplayValue;
        _isWidth = _sliderType == SliderType.Horizontal;

        _lengthSlider = _isWidth ? _sliderRectTf.rect.width : _sliderRectTf.rect.height;
    }

    public void TriggerEnter(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _touchTf = touchCollider.transform.parent;
            _touchInter = _touchTf.GetComponent<TouchInteraction>();
            UpdateColor(InteractionStateUI.InProximity);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            UpdateColor(InteractionStateUI.Active);

            _isMovingKnob = true;
            _label.enabled = true;
            _label.text = Mathf.Lerp(_minValue, _maxValue, _value).ToString();

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            StartCoroutine(MovingSlider());
        }
    }

    public void TriggerExit(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            UpdateColor(InteractionStateUI.Normal);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);

            _isMovingKnob = false;

            if(!_alwaysDisplayValue)
                _label.enabled = false;

            UpdateColor(InteractionStateUI.Normal);

            // TODO : Refresh value
        }
    }

    private void UpdateColor(InteractionStateUI interactionState)
    {
        int nbImage = _imgs.Count;

        for (int i = 0; i < nbImage; i++)
        {
            Image img = _imgs[i];
            ColorStateUI colorState = _colors[i];

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

    IEnumerator MovingSlider()
    {
        while(_isMovingKnob)
        {
            RetrieveValue();
            UpdateValue();
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
    }

    private void UpdateValue()
    {
        float positionFromVirtualAnchor = _lengthSlider * _value;

        if(_isWidth)
            _knobRectTf.localPosition = new Vector3(positionFromVirtualAnchor - _lengthSlider * .5f, 0f, 0f);
        else
            _knobRectTf.localPosition = new Vector3(0f, positionFromVirtualAnchor - _lengthSlider * .5f, 0f);

        Vector2 sizeDelta = _sliderFilledRectTf.sizeDelta;

        if (_isWidth)
            sizeDelta.x = positionFromVirtualAnchor;
        else
            sizeDelta.y = positionFromVirtualAnchor;

        _sliderFilledRectTf.sizeDelta = sizeDelta;

        float uNormalizedValue = Mathf.Lerp(_minValue, _maxValue, _value);

        _label.text = uNormalizedValue.ToString("0.##");

        _onChangedValue?.Invoke(uNormalizedValue);
    }

    public void SetNewValue(float normalizedValue)
    {
        _value = normalizedValue;
        UpdateValue();
    }

    private void OnValidate()
    {

        _isWidth = _sliderType == SliderType.Horizontal;
        _lengthSlider = _isWidth ? _sliderRectTf.rect.width : _sliderRectTf.rect.height;

        UpdateValue();
        
    }

    public enum SliderType
    {
        Horizontal,
        Vertical
    }
}
