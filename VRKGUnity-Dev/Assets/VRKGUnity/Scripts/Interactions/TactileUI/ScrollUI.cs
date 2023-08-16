using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class ScrollUI : BaseTouch, ILayoutGroup
    {
        public RectTransform ItemContainer { get { return _contentRect; } }


        [SerializeField]
        List<InteractiveGraphicUI> _uninteractiveGraphics;

        [SerializeField, Space(5)]
        RectTransform _viewportRect;

        [SerializeField]
        RectTransform _contentRect;

        [SerializeField]
        RectTransform _handleRect;

        [SerializeField]
        RectTransform _slidingAreaRect;


        [SerializeField, Space(5)]
        List<ScrollItem> _scrollItems;

        RectTransform _scrollUIRect;

        bool _needScroll;
        bool _isScrolling;

        float _slidingAreaHeight;
        float _maxDeltaHandle;
        float _contentOverflowHeight;
        float _handleHeight;
        float _halfHandleHeight;

        float _positionScroll;
        float _hapticDelay = .25f;
        float _hapticTime;

        private void Awake()
        {
            if(_scrollItems == null)
                _scrollItems = new List<ScrollItem>();
            else
            {
                foreach (ScrollItem item in _scrollItems)
                {
                    item.Start();
                }
            }

            _rebounceDelay = 0f;
            _scrollUIRect = GetComponent<RectTransform>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _isScrolling = false;
            _positionScroll = 0f;
            _hapticTime = Time.time;

            UpdateContent();
        }
 
        public void AddItem(ScrollItem scrollItem)
        {
            if(_scrollItems.Contains(scrollItem))
            {
                Debug.LogWarning("ScrollUI.AddItem already contain this scrollItem.");
                return;
            }

            _scrollItems.Add(scrollItem);
            scrollItem.RebuildLayout();
            UpdateContent();
        }

        public void AddItems(List<ScrollItem> scrollItems)
        {

            foreach(ScrollItem scrollItem in scrollItems) 
            {
                if (_scrollItems.Contains(scrollItem))
                {
                    Debug.LogWarning("ScrollUI.AddItem already contain this scrollItem.");
                    continue;
                }

                _scrollItems.Add(scrollItem);
                scrollItem.RebuildLayout();
            }

            UpdateContent();
        }

        public void RemoveItem(ScrollItem scrollItem, bool destroyGo = true)
        {
            if (!_scrollItems.Contains(scrollItem))
            {
                Debug.LogWarning("ScrollUI.RemoveItem doesn't contain this scrollItem.");
                return;
            }

            _scrollItems.Remove(scrollItem);

            if (destroyGo)
                Destroy(scrollItem.Go);

            UpdateContent();
        }

        public void RemoveItems(List<ScrollItem> scrollItems, bool destroyGo = true)
        {

            foreach(ScrollItem scrollItem in scrollItems) 
            {
                if (!_scrollItems.Contains(scrollItem))
                {
                    Debug.LogWarning("ScrollUI.RemoveItem doesn't contain this scrollItem.");
                    continue;
                }

                _scrollItems.Remove(scrollItem);

                if (destroyGo)
                    Destroy(scrollItem.Go);
            }

            UpdateContent();
        }


        public void UpdateContent()
        {
            // If only one ForceRebuild, colliders aren't disabled if the
            // item is outside the viewport
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);

            // Allow to SetLayoutVertical be called
            LayoutRebuilder.MarkLayoutForRebuild(_scrollUIRect);
        }

        private void UpdateParameters()
        {
            Vector2 sizeArea = _slidingAreaRect.sizeDelta;


            float heightViewport = _viewportRect.rect.height;
            float heightContent = GetContentHeight();


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

        private float GetContentHeight()
        {
            int nbChild = _contentRect.childCount;
            float height = 0f;

            for (int i = 0; i < nbChild; i++)
            {
                if(!_contentRect.GetChild(i).TryGetComponent<RectTransform>(out var child))
                    continue;

                height += child.rect.height;
            }

            return height;
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
                UpdateItemColliders();
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
            _contentRect.anchoredPosition = new Vector2(0f, Mathf.Lerp(0f, _contentOverflowHeight, _positionScroll));
        }

        private void TryActivateHaptic()
        {
            if (Time.time < _hapticTime)
                return;

            _hapticTime = Time.time + _hapticDelay;
            _touchInter.ActivateHaptic(.001f, .001f);
        }

        [ContextMenu("UpdateItemColliders")]
        private void UpdateItemColliders()
        {
            foreach (ScrollItem item in _scrollItems)
            {
                item.UpdateColliderState(_viewportRect);
            }
        }


        public void SetLayoutHorizontal() { }


        public void SetLayoutVertical()
        {
            Debug.Log("SetLayoutVertical");
            LayoutRebuilder.ForceRebuildLayoutImmediate(_viewportRect);
            UpdateParameters();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
            UpdateVisuals();
            UpdateColliderInteraction();
            UpdateItemColliders();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _interactiveGraphics?.TrySetName();
            UpdateContent();
        }  
#endif
    }
}
