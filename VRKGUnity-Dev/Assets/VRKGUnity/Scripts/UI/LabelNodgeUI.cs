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

    public void UpdateTransform(Vector3 hmdPosition, float nodeSize)
    {
        Vector3 newCanvasPosition = Vector3.zero;

        if(_type == LabelNodgeType.Node)
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

        Vector3 directionb = newCanvasPosition - hmdPosition;
        _canvasTf.rotation = Quaternion.LookRotation(directionb);
    }


    public void SetAll(bool active, Vector2 sizeCanvas, float fontSize)
    {
        _canvasTf.gameObject.SetActive(active);

        _canvasRect.sizeDelta = sizeCanvas;
        _labelRect.sizeDelta = sizeCanvas;

        _labelTxt.fontSize = fontSize;
    }

    public void SetSize(Vector2 sizeCanvas, float fontSize)
    {
        _canvasRect.sizeDelta = sizeCanvas;
        _labelRect.sizeDelta = sizeCanvas;

        _labelTxt.fontSize = fontSize;
    }

    public void SetActive(bool active)
    {
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