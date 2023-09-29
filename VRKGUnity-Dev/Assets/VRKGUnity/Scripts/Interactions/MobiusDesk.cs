using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


/// <summary>
/// Manage the rotation/translation of the desk when the user interact with it
/// </summary>
public class MobiusDesk : MonoBehaviour
{
    [SerializeField]
    Transform _deskTf;

    [SerializeField]
    GameObject _tooltipGo;

    [SerializeField]
    Transform _mainGraphTf;

    [SerializeField]
    float _distanceCheckIntent = .025f;


    bool _isDeskSelected = false;
    Vector3 _startGrabPos;

    Transform _handTf;
    
    public void SelectEntered(SelectEnterEventArgs args)
    {
        _isDeskSelected = true;


        _handTf = args.interactorObject.transform;
        DebugDev.Log("SelectEntered : " + _handTf.gameObject.name);

        _startGrabPos = _handTf.position;

        StartCoroutine(InteractingWithDesk());
    }

    public void SelectExited(SelectExitEventArgs args)
    {
        _isDeskSelected = false;
        DebugDev.Log("SelectExited");
    }

    IEnumerator InteractingWithDesk()
    {
        Vector3 deskPosition = _deskTf.position;

        Vector3 vFromDesk = (_handTf.position - deskPosition).normalized;
        vFromDesk.y = 0f;
        vFromDesk *= 0.318f;

        Transform tooltipTf = _tooltipGo.transform;

        tooltipTf.position = _handTf.position;//deskPosition + vFromDesk;
        tooltipTf.LookAt(deskPosition);
        _tooltipGo.SetActive(true);

        while ((_handTf.position - _startGrabPos).magnitude < _distanceCheckIntent && _isDeskSelected)
        {
            yield return null;
        }

        _tooltipGo.SetActive(false);

        Vector3 vFromStartGrab = (_handTf.position - _startGrabPos);

        float y = Mathf.Abs(vFromStartGrab.y);

        vFromStartGrab.y = 0f;

        float xz = vFromStartGrab.magnitude;


        if (xz > y)
            StartCoroutine(RotatingDesk());
        else
            StartCoroutine(TranslatingDesk());
    }

    IEnumerator RotatingDesk()
    {
        Vector3 vFromDesk = (_handTf.position - _deskTf.position);
        Vector2 originalV = new Vector2(vFromDesk.x, vFromDesk.z).normalized;

        Quaternion originalDeskRot = _deskTf.rotation;
        Quaternion originalGraphRot = _mainGraphTf.rotation;

        while(_isDeskSelected)
        {
            yield return null;

            Vector2 newV = GetCurentVector();
            float angle = Vector2.SignedAngle(originalV, newV);

            Quaternion deltaRot = Quaternion.Euler(0f, -angle, 0f);

            _deskTf.rotation = originalDeskRot * deltaRot;
            _mainGraphTf.rotation = originalGraphRot * deltaRot;
        }

        Vector2 GetCurentVector()
        {
            Vector3 vFromDesk = (_handTf.position - _deskTf.position);
            return new Vector2(vFromDesk.x, vFromDesk.z).normalized;
        }
    }

    IEnumerator TranslatingDesk()
    {
        Vector3 deskPos = _deskTf.position;
        Vector3 graphPos = _mainGraphTf.position;

        float originalGrabY = _startGrabPos.y;


        while (_isDeskSelected)
        {
            yield return null;
            float deltaY = (_handTf.position.y - originalGrabY);
            Vector3 delta = new Vector3(0f, deltaY, 0f);

            _deskTf.position = deskPos + delta;
            _mainGraphTf.position = graphPos + delta;
        }
    }

}
