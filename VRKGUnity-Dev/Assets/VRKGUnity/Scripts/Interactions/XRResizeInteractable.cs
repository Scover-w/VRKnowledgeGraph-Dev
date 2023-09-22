using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRResizeInteractable : XRGrabInteractable
{
    [SerializeField]
    RectTransform _toResizeTf;

    Vector3 _initialScale;
    float _initialDistance;

    bool _isResizing = false;

    Transform _handTfA;
    Transform _handTfB;

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        var handTf = args.interactorObject.transform;

        Debug.Log("OnSelectEnter : " + handTf.name);

        if (_handTfA != null && _handTfA != handTf) 
        {
            _handTfB = handTf;
            _initialScale = _toResizeTf.localScale;
            _initialDistance = (_handTfB.position - _handTfA.position).magnitude;
            _isResizing = true;


            DebugDev.Log("Resizing");
            StartCoroutine(Resizing());
            return;
        }


        _handTfA = handTf;
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        var handTf = args.interactorObject.transform;

        Debug.Log("OnSelectExit : " + handTf.name);

        if (_handTfA == handTf)
            _handTfA = null;
        else if (_handTfB == handTf)
            _handTfB = null;

        _isResizing = false;
    }

    IEnumerator Resizing()
    {
        float currentDistance = 0f;
        float scaleMult = 0f;

        while (_isResizing)
        {
            UpdateScaleMult();
            DebugDev.Log("---");
            DebugDev.Log(_toResizeTf.localScale.x);
            DebugDev.Log(scaleMult);
            _toResizeTf.localScale = _initialScale * scaleMult;
            DebugDev.Log(_toResizeTf.localScale.x);
            yield return null;
        }


        void UpdateScaleMult()
        {
            currentDistance = (_handTfB.position - _handTfA.position).magnitude;
            scaleMult = currentDistance / _initialDistance;
            DebugDev.Log(scaleMult);
        }
    }  
}
