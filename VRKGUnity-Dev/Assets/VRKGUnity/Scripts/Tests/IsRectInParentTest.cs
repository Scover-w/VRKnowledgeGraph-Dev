using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class IsRectInParentTest : MonoBehaviour
{
    [SerializeField]
    RectTransform _parentRect;

    [SerializeField]
    RectTransform _childRect;

    public bool IsInParentp;
    Vector3[] worldCorners = new Vector3[4];
    Vector3[] middleCorners = new Vector3[2];

    private void Update()
    {
        _childRect.GetWorldCorners(worldCorners);

        middleCorners[0] = (worldCorners[0] + worldCorners[1]) / 2f;
        middleCorners[1] = (worldCorners[2] + worldCorners[3]) / 2f;

        IsInParentp = IsInParent(_parentRect);

    }


    private bool IsInParent(RectTransform parentMaskRect)
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 viewportPoint = parentMaskRect.InverseTransformPoint(middleCorners[i]);

            if (parentMaskRect.rect.Contains(viewportPoint))
                return true;
        }

        return false;
    }
}
