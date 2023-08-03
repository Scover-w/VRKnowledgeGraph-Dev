using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;

public class PhysicalHSVWheelUI : MonoBehaviour, IPhysicalUI
{
    [SerializeField]
    ColorPickerUI _colorPickerUI;

    [SerializeField]
    Image _hsvWheelImg;

    [SerializeField]
    ColorStateUI _colorStateUI;

    [SerializeField]
    Image _borderImg;

    [SerializeField]
    RectTransform _cursorRectTf;

    Transform _touchTf;
    TouchInteraction _touchInter;

    RectTransform _wheelRecTf;
    Vector2 _localVector2;
    Vector2 _rightV2;

    float _h;
    float _s;
    float _v = 1f;

    bool _isMovingCursor = false;

    private void Start()
    {
        _wheelRecTf = _hsvWheelImg.GetComponent<RectTransform>();
        _rightV2 = Vector2.right;
    }

    public void Display(float h, float s, float v)
    {
        _h = h;
        _s = s;
        _v = v;

        PlaceCursor(h, s);
    }

    public void NewVColor(float v)
    {
        _v = v;
        _hsvWheelImg.color = Color.white * v;
    }

    public void TriggerEnter(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _touchTf = touchCollider.transform.parent;
            _touchInter = _touchTf.GetComponent<TouchInteraction>();
            UpdateColor(InteractionStateUI.InProximity);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            UpdateColor(InteractionStateUI.Active);

            _isMovingCursor = true;

            if (_touchInter != null)
                _touchInter.ActiveBtn(true, this);

            StartCoroutine(MovingCursor());
        }
    }

    public void TriggerExit(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            UpdateColor(InteractionStateUI.Normal);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.InteractionUI))
        {
            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);

            _isMovingCursor = false;
            UpdateColor(InteractionStateUI.Normal);

            // TODO : Refresh value
        }
    }

    private void UpdateColor(InteractionStateUI interactionState)
    {
        switch (interactionState)
        {
            case InteractionStateUI.Normal:
                _borderImg.color = _colorStateUI.NormalColor;
                break;
            case InteractionStateUI.InProximity:
                _borderImg.color = _colorStateUI.ProximityColor;
                break;
            case InteractionStateUI.Active:
                _borderImg.color = _colorStateUI.ActivatedColor;
                break;
        }
    }

    IEnumerator MovingCursor()
    {
        while (_isMovingCursor)
        {
            RetrieveLocalVector();
            ConvertToHSV();
            yield return null;
        }
    }

    private void RetrieveLocalVector()
    {
        Vector3 sliderWorldPosition = _wheelRecTf.position;
        Plane plane = new(_wheelRecTf.forward, sliderWorldPosition);
        Vector3 worldProjectedPoint = plane.ClosestPointOnPlane(_touchTf.position);

        Vector3 localVector3 = _wheelRecTf.InverseTransformPoint(worldProjectedPoint);

        _cursorRectTf.localPosition = localVector3;
        _localVector2 = new Vector2(localVector3.x / 512f, localVector3.y / 512f);

    }

    private void ConvertToHSV()
    {
        float angle = Vector2.SignedAngle(_rightV2, _localVector2.normalized);

        if(angle < 0)
            angle = 180f + (180f - Mathf.Abs(angle));

        float distance = _localVector2.magnitude;

        if(distance > 1f)
            distance = 1f;

        _colorPickerUI.SetNewColorFromWheel(angle, distance, _v);
    }


    private void PlaceCursor(float h, float s)
    {
        Vector2 directionCursor = _rightV2 * s;

        directionCursor = directionCursor * h * 512f;
        _cursorRectTf.localPosition = directionCursor;
    }
}
