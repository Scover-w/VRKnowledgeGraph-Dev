using TMPro;
using UnityEngine;

public class LabelNodgeUI : MonoBehaviour
{
    public Transform Transform { get { return _canvasTf; } }

    public string Text
    {
        set
        {
            _labelTxt.text = value;
        }
    }

    [SerializeField]
    RectTransform _canvasRect;

    [SerializeField]
    RectTransform _labelRect;

    [SerializeField]
    TMP_Text _labelTxt;

    float _scaleSize;
    bool _inRelativeMode = false;

    Transform _canvasTf;

    private LabelNodgeType _type;

    private Transform _followTf;
    private Transform _follow2Tf;


    private void Start()
    {
        _canvasTf = _canvasRect.transform;
    }

    public void SetFollow(Transform followTf, Transform follow2Tf = null) 
    {
        _type = (follow2Tf == null) ? LabelNodgeType.Node : LabelNodgeType.Edge;

        _followTf = followTf;
        _follow2Tf = follow2Tf;
    }

    public void UpdateTransform(Vector3 hmdPosition, float nodeSize = 0f)
    {
        Vector3 directionb = Vector3.zero;

        UpdateRotation();
        UpdateSize();


        void UpdateRotation()
        {
            Vector3 newCanvasPosition = Vector3.zero;

            if (_type == LabelNodgeType.Node)
            {
                Vector3 positionNode = _followTf.position;
                Vector3 direction = (hmdPosition - positionNode).normalized;

                newCanvasPosition = positionNode + direction * (nodeSize * 1.1f);
                _canvasTf.position = newCanvasPosition;
            }
            else
            {
                newCanvasPosition = (_followTf.position + _follow2Tf.position) * 0.5f;
                _canvasTf.position = newCanvasPosition;
            }

            directionb = newCanvasPosition - hmdPosition;
            _canvasTf.rotation = Quaternion.LookRotation(directionb);
        }

        void UpdateSize()
        {

            float distance = directionb.magnitude;

            float baseFontSize = Settings.BASE_FONT_SIZE_LABEL;
            Vector2 baseCanvasLabel = Settings.BASE_SIZE_LABEL_CANVAS;
            float minReadableFontSize = Settings.MIN_READABLE_FONT_SIZE;

            float atomicFontSize = (baseFontSize * _scaleSize) / distance;

            if (atomicFontSize < minReadableFontSize) // Realtive Canvas Size Mode : Need to be readable even far
            {
                _inRelativeMode = true;
                float relativeScale = (minReadableFontSize * distance) / baseFontSize;

                Vector2 sizeConvasA = baseCanvasLabel * relativeScale;
                float fontSizeA = baseFontSize * relativeScale;
                SetSize(_scaleSize, sizeConvasA, fontSizeA);
                return;
            }

            // Normal Size
            if (!_inRelativeMode)
                return;


            _inRelativeMode = false;
            Vector2 sizeConvasB = baseCanvasLabel * _scaleSize;
            float fontSizeB = baseFontSize * _scaleSize;
            SetSize(_scaleSize, sizeConvasB, fontSizeB);

        }
    }


    public void SetAll(bool active, float scaleSize, Vector2 sizeCanvas, float fontSize)
    {
        _scaleSize = scaleSize;

        _canvasRect.sizeDelta = sizeCanvas;
        _labelRect.sizeDelta = sizeCanvas;
        _labelTxt.fontSize = fontSize;

        _canvasTf.gameObject.SetActive(active);
    }

    public void SetSize(float scaleSize, Vector2 sizeCanvas, float fontSize)
    {
        _scaleSize = scaleSize;

        _canvasRect.sizeDelta = sizeCanvas;
        _labelRect.sizeDelta = sizeCanvas;

        _labelTxt.fontSize = fontSize;
    }

    public void SetActive(bool active)
    {
        if(_canvasTf == null)
            _canvasTf = _canvasRect.transform;
        _canvasTf.gameObject.SetActive(active);
    }

    public void Destroy()
    {
        Object.Destroy(_canvasTf.gameObject);
    }
}

public enum LabelNodgeType
{
    Node,
    Edge
}