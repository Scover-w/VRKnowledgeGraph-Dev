using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Wave.Essence.Hand.NearInteraction;

public class ScrollUI : MonoBehaviour, ITouchUI
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

    [SerializeField]
    bool _interactable = true;

    [SerializeField]
    List<InteractiveGraphicUI> _interactiveGraphics;

    [SerializeField]
    List<InteractiveGraphicUI> _uninteractiveGraphics;

    [SerializeField,Space(5)]
    RectTransform _viewportRect;

    [SerializeField]
    RectTransform _contentRect;

    [SerializeField]
    RectTransform _handleRect;

    [SerializeField]
    RectTransform _slidingAreaRect;

    [SerializeField]
    GameObject _interactionCollidersGo;

    Transform _touchTf;
    TouchInteractor _touchInter;

    InteractionStateUI _interactionStateUI;

    bool _needScroll;
    bool _isScrolling;


    float _slidingAreaHeight;
    float _maxDeltaHandle;
    float _contentOverflowHeight;
    float _handleHeight;
    float _halfHandleHeight;

    float _positionScroll;

    private void OnEnable()
    {
        _isScrolling = false;

        Invoke(nameof(DelayedOnEnable), .5f);
    }

    private void OnDisable()
    {
        if (_touchInter != null)
            _touchInter.ActiveBtn(false, this);
    }

    [ContextMenu("UpdateContent")]
    public void UpdateContent()
    {
        Invoke(nameof(DelayedUpdateContent), .1f);
    }

    private void DelayedUpdateContent()
    {
        SetParameters();
        UpdateVisuals();
    }

    private void DelayedOnEnable()
    {
        SetParameters();
        TrySetNormalInteractionState();
        UpdateInteractionColor();
        UpdateColliderActivation();
    }
    private void SetParameters()
    {
        Vector2 sizeArea = _slidingAreaRect.sizeDelta;


        float heightViewport = _viewportRect.rect.height;
        float heightContent = _contentRect.rect.height;

        _slidingAreaHeight = _slidingAreaRect.rect.height;

        _needScroll = heightContent > heightViewport;

        if (!_needScroll)
        {
            _handleRect.sizeDelta = sizeArea;
            return;
        }

        sizeArea.y *= heightViewport / heightContent;
        _handleRect.sizeDelta = sizeArea;
        _maxDeltaHandle = _slidingAreaHeight - sizeArea.y * .5f;
        _handleHeight = sizeArea.y;
        _halfHandleHeight = sizeArea.y * .5f;


        _contentOverflowHeight = heightContent - heightViewport;
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

            _isScrolling = true;

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            StartCoroutine(Scrolling());
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

            _isScrolling = false;


            _interactionStateUI = InteractionStateUI.Normal;
            UpdateInteractionColor();
        }
    }

    private void UpdateInteractionColor()
    {
        if(_needScroll)
            _interactiveGraphics.UpdateColor(_interactionStateUI);
        else
            _uninteractiveGraphics.UpdateColor(_interactionStateUI);
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
        _interactionCollidersGo.SetActive(_needScroll && _interactable);
    }

    IEnumerator Scrolling()
    {
        while(_isScrolling) 
        {
            CalculatePositionScroll();
            UpdateVisuals();
            yield return null;
        }
    }

    private void CalculatePositionScroll()
    {
        Vector3 scrollWorldPosition = _slidingAreaRect.position;
        Plane plane = new(_slidingAreaRect.forward, scrollWorldPosition);

        Vector3 worldProjectedPoint = plane.ClosestPointOnPlane(_touchTf.position);

        Vector3 localVector = _slidingAreaRect.InverseTransformPoint(worldProjectedPoint);

        float positionFromVirtualAnchor = Mathf.Clamp((_slidingAreaHeight * .5f) - localVector.y, 0f, _slidingAreaHeight); // 0f to x
        positionFromVirtualAnchor = Mathf.Clamp(positionFromVirtualAnchor, _halfHandleHeight, _maxDeltaHandle);
        _positionScroll = (positionFromVirtualAnchor - _halfHandleHeight) / (_slidingAreaHeight - _handleHeight);
    }

    private void UpdateVisuals()
    {
        // Handle Position
        float yPositionInArea = Mathf.Lerp(_halfHandleHeight, _maxDeltaHandle, _positionScroll) - _halfHandleHeight;
        _handleRect.anchoredPosition = new Vector2(0f, -yPositionInArea);

        // Content Position
        _contentRect.anchoredPosition = new Vector2(0f, Mathf.Lerp(0f, _contentOverflowHeight, _positionScroll));
    }

    private void OnValidate()
    {


        SetParameters();
        TrySetNormalInteractionState();
        UpdateInteractionColor();
        UpdateColliderActivation();
    }
}
