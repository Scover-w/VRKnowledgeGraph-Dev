using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class HSVWheelUI : MonoBehaviour, ITouchUI
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
        TouchInteractor _touchInter;

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

        public void UpdateColor(float h, float s, float v)
        {
            _h = h;
            _s = s;
            _v = v;

            Color white = Color.white * _v;
            white.a = 1f;
            _hsvWheelImg.color = white;

            PlaceCursor(h, s);
        }

        public void UpdateV(float v)
        {
            _v = v;

            Color white = Color.white * _v;
            white.a = 1f;
            _hsvWheelImg.color = white;
        }

        public void TriggerEnter(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                _touchTf = touchTf;
                _touchInter = _touchTf.GetComponent<TouchInteractor>();
                UpdateColor(InteractionStateUI.InProximity);
            }
            else if (!isProximity)
            {
                UpdateColor(InteractionStateUI.Active);

                _isMovingCursor = true;

                if (_touchInter != null)
                    _touchInter.ActiveBtn(true, this);

                StartCoroutine(MovingCursor());
            }
        }

        public void TriggerExit(bool isProximity, Transform touchTf)
        {
            if (isProximity)
            {
                UpdateColor(InteractionStateUI.Normal);
            }
            else if (!isProximity)
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

            float magnitude = localVector3.magnitude;

            if (magnitude > 256f)
            {
                localVector3 *= (256f / magnitude);
            }

            _cursorRectTf.localPosition = localVector3;
            _localVector2 = new Vector2(localVector3.x / 256f, localVector3.y / 256f);

        }

        private void ConvertToHSV()
        {
            float angle = Vector2.SignedAngle(Vector3.left, _localVector2.normalized);

            if (angle < 0)
                angle = 180f + (180f - Mathf.Abs(angle));

            Debug.Log(angle);

            float distance = _localVector2.magnitude;

            _colorPickerUI.SetNewColorFromWheel(angle / 360f, distance, _v);
        }

        private void PlaceCursor(float h, float s)
        {
            float angle = h * Mathf.PI * 2f;

            Vector2 directionCursor = Vector2.left * s;

            directionCursor = Rotate(directionCursor, angle);

            directionCursor = directionCursor * 256f;
            _cursorRectTf.localPosition = directionCursor;
        }



        public Vector2 Rotate(Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

    }
}