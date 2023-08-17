using AIDEN.TactileUI;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static AIDEN.TactileUI.SliderUI;

public class ScrollViewUI : BaseTouch, ILayoutGroup
{
    public override bool Interactable
    {
        get
        {
            return _interactable;
        }
        set 
        { 
            base.Interactable = value;
            _scrollbarUI.Interactable = value;
        }
    }

    public RectTransform ItemContainer { get { return _contentRect; } }

    [SerializeField]
    ScrollbarUI _scrollbarUI;

    [SerializeField, Space(5)]
    RectTransform _viewportRect;

    [SerializeField]
    RectTransform _contentRect;

    [SerializeField]
    bool _areItemsInteractive = true;

    [SerializeField, Space(5)]
    List<ScrollItemUI> _scrollItems;

    RectTransform _scrollUIRect;
    ScrollItemUI _hoveredItemUI;
    ScrollItemUI _selectedItemUI;

    float _touchOffsetY;

    bool _isMovingIn = false;
    float _viewportHeight;
    float _pivotMultiplicator;

    private void Awake()
    {
        if (_scrollItems == null)
            _scrollItems = new List<ScrollItemUI>();

        _scrollUIRect = GetComponent<RectTransform>();

        _viewportHeight = _viewportRect.sizeDelta.y;
        _pivotMultiplicator = 1f - _viewportRect.pivot.y;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _isMovingIn = false;
        UpdateContent();
    }



    public void AddItem(ScrollItemUI scrollItem)
    {
        if (_scrollItems.Contains(scrollItem))
        {
            Debug.LogWarning("ScrollViewUI.AddItem already contain this scrollItem.");
            return;
        }

        _scrollItems.Add(scrollItem);
        scrollItem.RebuildLayout();
        UpdateContent();
    }

    public void AddItems(List<ScrollItemUI> scrollItems)
    {

        foreach (ScrollItemUI scrollItem in scrollItems)
        {
            if (_scrollItems.Contains(scrollItem))
            {
                Debug.LogWarning("ScrollViewUI.AddItem already contain this scrollItem.");
                continue;
            }

            _scrollItems.Add(scrollItem);
            scrollItem.RebuildLayout();
        }

        UpdateContent();
    }

    public void RemoveItem(ScrollItemUI scrollItem, bool destroyGo = true)
    {
        if (!_scrollItems.Contains(scrollItem))
        {
            Debug.LogWarning("ScrollViewUI.RemoveItem doesn't contain this scrollItem.");
            return;
        }

        _scrollItems.Remove(scrollItem);

        if (destroyGo)
            Destroy(scrollItem.Go);

        if(_scrollItems.Count == 0)
            _scrollbarUI.ResetScrollPosition();

        UpdateContent();
    }

    public void RemoveItems(List<ScrollItemUI> scrollItems, bool destroyGo = true)
    {

        foreach (ScrollItemUI scrollItem in scrollItems)
        {
            if (!_scrollItems.Contains(scrollItem))
            {
                Debug.LogWarning("ScrollViewUI.RemoveItem doesn't contain this scrollItem.");
                continue;
            }

            _scrollItems.Remove(scrollItem);

            if (destroyGo)
                Destroy(scrollItem.Go);
        }

        if (_scrollItems.Count == 0)
            _scrollbarUI.ResetScrollPosition();

        UpdateContent();
    }


    public override void TriggerEnter(bool isProximity, Transform touchTf)
    {
        if (!_areItemsInteractive)
            return;

        if (_scrollItems.Count == 0)
            return;


        base.TriggerEnter(isProximity, touchTf);

        if (isProximity)
        {
            HoverFirstItem();
            StartCoroutine(MovingInProximity());
        }
    }

    protected override void TryActivate()
    {
        if (!base.CanActivate())
            return;

        base.Activate();

        if (_hoveredItemUI == null)
            return;


        if (_selectedItemUI == _hoveredItemUI)
            return;

        if (_selectedItemUI != null)
            _selectedItemUI.OnSelect(false);

        _hoveredItemUI.OnSelect(true);
        _selectedItemUI = _hoveredItemUI;

        _touchInter.ActivateHaptic();
    }

    public override void TriggerExit(bool isProximity, Transform touchTf)
    {
        if (!_areItemsInteractive)
            return;

        if (_scrollItems.Count == 0)
            return;

        base.TriggerExit(isProximity, touchTf);

        if(isProximity)
        {
            TryUnHoverItem();
        }
    }

    [ContextMenu("ResetScrollPosition")]
    public void ResetScrollPosition()
    {
        _scrollbarUI.ResetScrollPosition();
    }

    public void UpdateContent()
    {
        // If only one ForceRebuild, colliders aren't disabled if the
        // item is outside the viewport
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_viewportRect);

        // Allow to SetLayoutVertical be called
        LayoutRebuilder.MarkLayoutForRebuild(_scrollUIRect);
    }

    public void SetLayoutHorizontal(){}

    public void SetLayoutVertical()
    {
        Debug.Log("SetLayoutVertical");
        LayoutRebuilder.ForceRebuildLayoutImmediate(_viewportRect);
        UpdateParametersItems();
        _scrollbarUI.UpdateHandleHeight();
        TrySetNormalInteractionState();
        UpdateInteractionColor();
        UpdateColliderInteraction();
    }

    private void UpdateParametersItems()
    {
        float y = 0f;

        foreach (var itemUI in _scrollItems)
        {
            y = itemUI.RecomputeParameters(y);
        }
    }

    private void HoverFirstItem()
    {
        if (_scrollItems.Count == 0)
            return;

        _hoveredItemUI = GetHoveredItem();
        _hoveredItemUI.OnProximity(true);
    }

    private void TryUnHoverItem()
    {
        if (_hoveredItemUI == null)
            return;

        _hoveredItemUI.OnProximity(false);
        _hoveredItemUI = null;
    }

    private ScrollItemUI GetHoveredItem()
    {
        float yPosition = _scrollbarUI.ContentYOffset + _touchOffsetY;


        foreach(var itemUI in _scrollItems)
        {
            if(itemUI.IsInRange(yPosition))
            {
                return itemUI;
            }
        }

        return _scrollItems[0];
    }

    IEnumerator MovingInProximity()
    {
        while(_inProximity)
        {
            UpdateTouchOffsetY();
            UpdateHoveredItem();
            yield return null;
        }
    }

    private void UpdateTouchOffsetY()
    {
        Vector3 sliderWorldPosition = _viewportRect.position;
        Plane plane = new(_viewportRect.forward, sliderWorldPosition);
        Vector3 worldProjectedPoint = plane.ClosestPointOnPlane(_touchTf.position);

        float y = _viewportRect.InverseTransformPoint(worldProjectedPoint).y * -1f;

        y += _pivotMultiplicator * _viewportHeight;


        y = Mathf.Clamp(y, 0f, _viewportHeight);

        _touchOffsetY = y;
    }

    private void UpdateHoveredItem()
    {
        if (_scrollItems.Count == 0)
            return;

        var hoveredItemUI = GetHoveredItem();

        if (hoveredItemUI == _hoveredItemUI)
            return;

        _hoveredItemUI.OnProximity(false);
        _hoveredItemUI = hoveredItemUI;
        hoveredItemUI.OnProximity(true);

    }


#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (_scrollbarUI.Interactable != Interactable)
            _scrollbarUI.Interactable = Interactable;
    }
#endif

}
