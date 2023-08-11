using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class ScrollUI : BaseTouch
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

        bool _needScroll;
        bool _isScrolling;

        float _slidingAreaHeight;
        float _maxDeltaHandle;
        float _contentOverflowHeight;
        float _handleHeight;
        float _halfHandleHeight;

        float _positionScroll;

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
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _isScrolling = false;
            _positionScroll = 0f;

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


        private void UpdateContent()
        {
            StartCoroutine(UpdatingContent());
        }

        IEnumerator UpdatingContent()
        {
            _contentRect.ForceUpdateRectTransforms();
            _viewportRect.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_viewportRect);
            yield return null;
            SetParameters();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
            UpdateVisuals();
            yield return null;
            _contentRect.ForceUpdateRectTransforms();
            _viewportRect.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_viewportRect);
            yield return null;
            UpdateColliderInteraction();
            UpdateItemColliders();
        }


        private void SetParameters()
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
                var child = _contentRect.GetChild(i).GetComponent<RectTransform>();

                if(child == null)
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
                _touchInter.ActivateHaptic();
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

        [ContextMenu("UpdateItemColliders")]
        private void UpdateItemColliders()
        {
            foreach (ScrollItem item in _scrollItems)
            {
                item.UpdateColliderState(_viewportRect);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _interactiveGraphics?.TrySetName();

            _contentRect.ForceUpdateRectTransforms();
            _viewportRect.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_viewportRect);

            SetParameters();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
            UpdateVisuals();

            _contentRect.ForceUpdateRectTransforms();
            _viewportRect.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_viewportRect);

            UpdateColliderInteraction();
            UpdateItemColliders();
        }
#endif
    }

    [Serializable]
    public class ScrollItem
    {
        public GameObject Go { get { return _rect.gameObject; } }

        [SerializeField]
        RectTransform _rect;
        [SerializeField]
        List<Collider> _colliders;

        Vector3[] _worldCorners;
        Vector3[] _middleCorners;
        bool _areColliderEnabled = true;

        public ScrollItem(RectTransform rectTransform, List<Collider> colliders)
        {
            _rect = rectTransform;
            _colliders = colliders;

            _worldCorners = new Vector3[4];
            _middleCorners = new Vector3[2];
            _areColliderEnabled = true;
        }

        /// <summary>
        /// For Items directly put in the editor
        /// </summary>
        public void Start()
        {
            _worldCorners = new Vector3[4];
            _middleCorners = new Vector3[2];
            _areColliderEnabled = true;
        }

        public void UpdateColliderState(RectTransform parentMaskRect)
        {
            _rect.GetWorldCorners(_worldCorners);
            CalculateMiddleCorner();

            bool isInParent = IsInParent(parentMaskRect);

            if (isInParent == _areColliderEnabled)
                return;

            _areColliderEnabled = isInParent;
            SetCollider(_areColliderEnabled);
        }

        private void CalculateMiddleCorner()
        {
            _middleCorners[0] = (_worldCorners[0] + _worldCorners[1]) / 2f;
            _middleCorners[1] = (_worldCorners[2] + _worldCorners[3]) / 2f;
        }

        private bool IsInParent(RectTransform parentMaskRect)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 viewportPoint = parentMaskRect.InverseTransformPoint(_middleCorners[i]);

                if (parentMaskRect.rect.Contains(viewportPoint))
                    return true;
            }

            return false;
        }

        private void SetCollider(bool enable)
        {
            foreach (Collider collider in _colliders)
            {
                collider.enabled = enable;
            }
        }
    }
}
