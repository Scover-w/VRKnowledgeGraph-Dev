using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class HSVWheelUI : MonoBehaviour, ITouchUI
    {
        public bool Interactable
        {
            get
            {
                return _interactable;
            }
            set
            {
                _interactable = value;

                UpdateColliderActivation();
                TrySetNormalInteractionState();
                UpdateInteractionColor();
            }
        }

        [SerializeField]
        bool _interactable = true;

        [SerializeField]
        ColorPickerUI _colorPickerUI;

        [SerializeField]
        Image _hsvWheelImg;

        [SerializeField]
        List<InteractiveGraphicUI> _interactiveGraphics;

        [SerializeField]
        RectTransform _cursorRectTf;

        [SerializeField]
        GameObject _interactionCollidersGo;

        Transform _touchTf;
        TouchInteractor _touchInter;

        InteractionStateUI _interactionStateUI;

        RectTransform _wheelRecTf;
        Vector2 _localVector2;

        float _h;
        float _s;
        float _v = 1f;

        bool _isMovingCursor = false;

        private void Start()
        {
            _wheelRecTf = _hsvWheelImg.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            _isMovingCursor = false;
            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }

        private void OnDisable()
        {
            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);
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
                _interactionStateUI = InteractionStateUI.InProximity;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                _interactionStateUI = InteractionStateUI.Active;
                UpdateInteractionColor();

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
                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
            else if (!isProximity)
            {
                if (_touchInter != null)
                    _touchInter.ActiveBtn(false, this);

                _isMovingCursor = false;
                _interactionStateUI = InteractionStateUI.Normal;
                UpdateInteractionColor();
            }
        }

        private void UpdateInteractionColor()
        {
            _interactiveGraphics.UpdateColor(_interactionStateUI);
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

        private void UpdateColliderActivation()
        {
            _interactionCollidersGo.SetActive(_interactable);
        }

        private void TrySetNormalInteractionState()
        {
            if (_interactable)
                _interactionStateUI = InteractionStateUI.Normal;
            else
                _interactionStateUI = InteractionStateUI.Disabled;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _interactiveGraphics?.TrySetName();

            UpdateColliderActivation();
            TrySetNormalInteractionState();
            UpdateInteractionColor();
        }
#endif
    }
}