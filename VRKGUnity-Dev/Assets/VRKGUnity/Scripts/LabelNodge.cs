using TMPro;
using UnityEngine;

public class LabelNodge
{
    public Transform Transform { get { return _canvasTf; } }
    public static Transform CamTf;

    public string Text
    {
        set
        {
            _labelTxt.text = value;
        }
    }

    private LabelNodgeType _type;

    private Transform _followTf;
    private Transform _follow2Tf;

    private Transform _canvasTf;
    private TMP_Text _labelTxt;

    public LabelNodge(Transform canvasTf, TMP_Text labelTxt)
    {
        _canvasTf = canvasTf;
        _labelTxt = labelTxt;
    }

    public void SetFollow(Transform followTf, Transform follow2Tf = null) 
    {
        _type = (follow2Tf == null) ? LabelNodgeType.Node : LabelNodgeType.Edge;
        _followTf = followTf;
        _follow2Tf = follow2Tf;
    }

    public void UpdateTransform()
    {
        _canvasTf.position = ( (_type == LabelNodgeType.Node)? _followTf.position + new Vector3(0f, .2f, 0f) : (_followTf.position + _follow2Tf.position) * 0.5f);

        Vector3 direction = _canvasTf.position - CamTf.position;
        _canvasTf.rotation = Quaternion.LookRotation(direction);
    }

    public void SetActive(bool active)
    {
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