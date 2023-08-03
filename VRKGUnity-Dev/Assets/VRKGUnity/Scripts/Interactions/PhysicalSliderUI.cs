using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using static PhysicalTabControllerUI;

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

    public float MinValue = 0f;
    public float MaxValue = 1f;

    Transform _touchTf;
    TouchInteraction _touchInter;

    public float Value = .5f;
    private bool _isMovingKnob = false;

    private void Start()
    {
        _label.enabled = false;
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
            _label.text = Mathf.Lerp(MinValue, MaxValue, Value).ToString();

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

        float widthSlider = _sliderRectTf.rect.width;

        float xPositionFromLeft = (widthSlider * .5f) + localVector.x;

        float normalizedX = xPositionFromLeft / widthSlider;

        if (normalizedX < 0f)
            normalizedX = 0f;
        else if (normalizedX > 1f)
            normalizedX = 1f;

        Value = normalizedX;

        UpdateValue();
    }

    private void UpdateValue()
    {
        float widthRect = _sliderRectTf.rect.width;
        float widthValue = widthRect * Value;

        _knobRectTf.localPosition = new Vector3(widthValue - widthRect * .5f, 0f, 0f);
        Vector2 sizeDelta = _sliderFilledRectTf.sizeDelta;
        sizeDelta.x = widthValue;
        _sliderFilledRectTf.sizeDelta = sizeDelta;

        _label.text = Mathf.Lerp(MinValue, MaxValue, Value).ToString("0.##");
    }

    private void OnValidate()
    {
        UpdateValue();
        
    }
}
