using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProjectionPlaneTest : MonoBehaviour
{
    public Transform TouchTf;

    public RectTransform RectTf;

    public RectTransform RectToPlaceTf;
    public RectTransform RectFlatToPlaceTf;

    public RectTransform LeftStartRect;


    public RectTransform RectFlatTf;

    // Update is called once per frame
    void Update()
    {
        Plane plane = new Plane(RectTf.forward, RectTf.position);

        Vector3 projectedPoint = plane.ClosestPointOnPlane(TouchTf.position);

        Debug.DrawLine(TouchTf.position, projectedPoint, Color.red);


        Vector3 relativeVector = projectedPoint - RectTf.position;
        var planeRotation = RectTf.rotation;


        Vector3 localVector = RectTf.InverseTransformPoint(projectedPoint);

        RectToPlaceTf.localPosition = new Vector3(localVector.x, 0f, 0f);

        float width = RectTf.rect.width;

        float xWidth = (width * .5f) + localVector.x;
        
        float normalizedX = xWidth / width;

        if(normalizedX < 0f)
            normalizedX = 0f;
        else if(normalizedX > 1f)
            normalizedX = 1f;

        Debug.Log(normalizedX);


        LeftStartRect.sizeDelta = new Vector2(xWidth, 5f);
    }
}
