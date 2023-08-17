using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollbarUI : BaseTouch
{
    public float ContentYOffset { get { return _contentYOffset; } }

    [SerializeField]
    List<InteractiveGraphicUI> _uninteractiveGraphics;

    [SerializeField]
    ScrollViewUI _scrollViewUI;

    [SerializeField]
    RectTransform _viewportRect;

    [SerializeField]
    RectTransform _contentRect;

    [SerializeField]
    RectTransform _handleRect;

    [SerializeField]
    RectTransform _slidingAreaRect;

    bool _needScroll;
    bool _isScrolling;

    float _slidingAreaHeight;
    float _maxDeltaHandle;
    float _contentOverflowHeight;
    float _handleHeight;
    float _halfHandleHeight;
    float _positionScroll;

    float _contentYOffset = 0f;

    readonly float _hapticDelay = .25f;
    float _hapticTime;

    private void Awake()
    {
        _rebounceDelay = 0f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _isScrolling = false;
        _positionScroll = 0f;
        _hapticTime = Time.time;

        UpdateVisuals();
    }


    protected override void TryActivate()
    {
        if (!CanActivate())
            return;

        base.Activate();

        _isScrolling = true;
        StartCoroutine(Scrolling());
    }

    public override void TriggerExit(bool isProximity, Transform touchTf)
    {
        base.TriggerExit(isProximity, touchTf);

        if (!isProximity)
        {
            _isScrolling = false;
        }
    }

    protected override void UpdateInteractionColor()
    {
        if (_needScroll)
            _interactiveGraphics.UpdateColor(_interactionStateUI);
        else
            _uninteractiveGraphics.UpdateColor(_interactionStateUI);
    }

    protected override void UpdateColliderInteraction()
    {
        _interactionCollidersGo.SetActive(_needScroll && _interactable);
    }

    IEnumerator Scrolling()
    {
        while (_isScrolling)
        {
            CalculatePositionScroll();
            UpdateVisuals();
            TryActivateHaptic();
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
        float newPositionScroll = (positionFromVirtualAnchor - _halfHandleHeight) / (_slidingAreaHeight - _handleHeight);

        _positionScroll = Mathf.Lerp(_positionScroll, newPositionScroll, 10f * Time.deltaTime);
    }

    private void UpdateVisuals()
    {
        // Handle Position
        float yPositionInArea = Mathf.Lerp(_halfHandleHeight, _maxDeltaHandle, _positionScroll) - _halfHandleHeight;
        _handleRect.anchoredPosition = new Vector2(0f, -yPositionInArea);

        // Content Position
        _contentYOffset = Mathf.Lerp(0f, _contentOverflowHeight, _positionScroll);
        _contentRect.anchoredPosition = new Vector2(0f, _contentYOffset);
    }

    public void ResetScrollPosition()
    {
        _positionScroll = 0f;
        UpdateVisuals();
    }

    private void TryActivateHaptic()
    {
        if (Time.time < _hapticTime)
            return;

        _hapticTime = Time.time + _hapticDelay;
        _touchInter.ActivateHaptic(.001f, .001f);
    }

    private float GetContentHeight()
    {
        int nbChild = _contentRect.childCount;
        float height = 0f;

        for (int i = 0; i < nbChild; i++)
        {
            if (!_contentRect.GetChild(i).TryGetComponent<RectTransform>(out var child))
                continue;

            height += child.rect.height;
        }

        return height;
    }

    private void UpdateParameters()
    {
        Vector2 sizeArea = _slidingAreaRect.sizeDelta;


        float heightViewport = _viewportRect.rect.height;
        float heightContent = _contentRect.rect.height;//GetContentHeight();


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

    public void UpdateHandleHeight()
    {
        UpdateParameters();
        UpdateVisuals();
        UpdateInteractionColor();
        UpdateColliderInteraction();
    }


#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (_scrollViewUI.Interactable != Interactable)
            Interactable = _scrollViewUI.Interactable;
    }
#endif

}
