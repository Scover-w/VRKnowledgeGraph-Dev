using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhysicalBtnTest : MonoBehaviour
{
    [SerializeField]
    List<InteractionColorImg> _frontInteractionColors;

    [SerializeField]
    Image _backImg;

    [SerializeField]
    RectTransform _staticRect;

    [SerializeField]
    RectTransform _hoverableRect;

    [SerializeField, Space(10)]
    UnityEvent _onClick;

    [SerializeField]
    Color _normalColor = ColorPalette.Primary;

    [SerializeField]
    Color _hoveredColor = ColorPalette.Primary_75;

    [SerializeField]
    Color _pressedColor = ColorPalette.Secondary;


    Transform _rectContainerTf;
    Transform _touchTf;

    Vector3 _hoveredPosition;
    Vector3 _smoothVelocity = Vector3.zero;

    float _smoothTime = 0.3f;
    float _touhRadius = Settings.TOUCH_RADIUS;

    bool _isInProximityField = false;
    bool _isInActiveField = false;

    bool _inPlace = true;

    bool _canClick = true;

    private void Start()
    {
        _hoveredPosition = new Vector3(0f, 0f, -250f);

        _rectContainerTf = _staticRect.transform;
    }

    private void Update()
    {
        if (_inPlace && !_isInProximityField && !_isInActiveField)
            return;

        if (!_inPlace && !_isInProximityField && !_isInActiveField)
        {
            UnHoverButton();
            return;
        }

        if (_isInProximityField && !_isInActiveField)
        {
            HoverButton();
            return;
        }

        FollowButton();
    }


    private void UnHoverButton()
    {
        SetSmoothedPosition(Vector3.zero);

        if(_smoothVelocity == Vector3.zero)
        {
            _inPlace = true;
            SetFrontColor(_normalColor);
            SetBackColor(0f);
        }
    }

    private void HoverButton()
    {
        SetSmoothedPosition(_hoveredPosition);
    }

    private void FollowButton()
    {
        float distance = GetDistanceBetweenBtnAndTouch();
        Vector3 localP = _hoverableRect.localPosition;
        _hoverableRect.localPosition = new Vector3(localP.x, localP.y, -distance * 1000);
    }



    float GetDistanceBetweenBtnAndTouch()
    {
        Plane plane = new Plane(_rectContainerTf.forward, _staticRect.position);

        float distance = (-plane.GetDistanceToPoint(_touchTf.position)) - _touhRadius;

        if (distance <= 0f)
        {
            TryClick();
            SetBackColor(0f);
            return 0f;
        }

        Debug.Log("Distance : " + distance);
        Debug.Log(1f - ((distance * 1000) / 250f));

        SetBackColor(1f - ((distance * 1000) / 250f) );

        return distance;
    }


    private void SetSmoothedPosition(Vector3 targetPosition)
    {
        _hoverableRect.localPosition = Vector3.SmoothDamp(_hoverableRect.localPosition, targetPosition, ref _smoothVelocity, _smoothTime);
    }

    private void SetBackColor(float t)
    {
        _backImg.color = Color.Lerp(_normalColor, _pressedColor, t);
    }

    private void TryClick()
    {
        if (!_canClick)
            return;

        _canClick = false;
        SetFrontColor(_pressedColor);
        SetBackColor(0f);

        _onClick?.Invoke();
    }

    public void TriggerEnter(bool isProximity, Collider touchCollider)
    {
        if(isProximity)
        {
            _touchTf = touchCollider.transform;
            _isInProximityField = true;
            _inPlace = false;
            SetFrontColor(_hoveredColor);
        }
        else
        {
            _isInActiveField = true;
        }
    }

    public void TriggerExit(bool isProximity)
    {
        if (isProximity)
        {
            _isInProximityField = false;
            SetFrontColor(_normalColor);
        }
        else
        {
            _isInActiveField = false;
            _canClick = true;
            SetFrontColor(_isInProximityField ? _hoveredColor : _normalColor);
        }
    }

    private void SetFrontColor(Color color)
    {
        foreach(var interactionColor in _frontInteractionColors)
        {

            //frontImg.color = color;
        }
    }

    private void OnValidate()
    {
        if (_frontInteractionColors == null)
            return;

        for(int i = 0; i < _frontInteractionColors.Count; i++) 
        { 
            var itColor = _frontInteractionColors[i];
            var img = itColor.Img;
            itColor.Name = (img == null) ? "None" : img.name;
        }
    }

}


public enum InteractionState
{
    Default,
    InProximity,
    Pushed,
    Active
}

[Serializable]
public class InteractionColorImg
{
    public Image Img { get { return _img; } }

    [HideInInspector]
    public string Name;

    [SerializeField]
    Image _img;

    [SerializeField]
    Color _normalColor;

    [SerializeField]
    Color _hoveredColor;

    [SerializeField]
    Color _pressedColor;

}