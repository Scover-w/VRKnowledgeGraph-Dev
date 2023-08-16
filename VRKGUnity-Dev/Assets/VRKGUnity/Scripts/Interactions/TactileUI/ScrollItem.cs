using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    [Serializable]
    public class ScrollItem
    {
        public GameObject Go { get { return _rect.gameObject; } }


        RectTransform _rect;
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


        public void RebuildLayout() 
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
            UpdateCollidersSize();
        }

        private void UpdateCollidersSize()
        {
            Vector2 size = _rect.sizeDelta;

            foreach (var collider in _colliders)
            {
                if (collider is not BoxCollider)
                    continue;

                BoxCollider box = collider as BoxCollider;
                var boxSize = box.size;
                boxSize.y = size.y;
                box.size = boxSize;
            }
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
