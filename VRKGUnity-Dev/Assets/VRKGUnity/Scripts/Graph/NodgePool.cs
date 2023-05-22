using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class NodgePool : MonoBehaviour
{
    public static NodgePool Instance { get { return _instance; } }


    [SerializeField]
    GameObject _nameCanvasPf;

    [SerializeField]
    GameObject _nodePf;

    [SerializeField]
    Material _lineMat;

    ObjectPool<LabelNodgeUI> _labelNodgeUIPool;
    ObjectPool<Transform> _nodePool;
    ObjectPool<LineRenderer> _edgePool;
    static NodgePool _instance;

    Transform _parentGraphTf;
    Transform _poolGraphTf;


    private void Start()
    {

        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(this);
            return;
        }

        _parentGraphTf = new GameObject("ParentNodes").transform;
        _poolGraphTf = new GameObject("Pool Graph").transform;
        CreateLabelNodgePool();
        CreateEdgePool();
        CreateNodePool();
    }

    private void CreateLabelNodgePool()
    {
        _labelNodgeUIPool = new ObjectPool<LabelNodgeUI>(() =>
        {
            var canvas = Object.Instantiate(_nameCanvasPf);
            var txtLabel = canvas.GetComponentInChildren<TMP_Text>();
            var tf = canvas.transform;
            var labelNodgeUI = new LabelNodgeUI(tf, txtLabel);
            tf.SetParent(_parentGraphTf);
            return labelNodgeUI;
        }, labelNode =>
        {
            labelNode.SetActive(true);
            labelNode.Transform.SetParent(_parentGraphTf);
        }, labelNode =>
        {
            labelNode.SetActive(false);
            labelNode.Transform.SetParent(_poolGraphTf);
        }, labelNode =>
        {
            labelNode.Destroy();
        }, false, 100, 10000);

        LabelNodgeUI.CamTf = Camera.main.transform;
    }

    private void CreateNodePool()
    {
        _nodePool = new ObjectPool<Transform>(() =>
        {
            var nodeSphereTf = Instantiate(_nodePf).transform;
            nodeSphereTf.SetParent(_parentGraphTf);
            nodeSphereTf.localScale = new Vector3(.2f, .2f, .2f);
            return nodeSphereTf;

        }, nodeSphereTf =>
        {
            nodeSphereTf.gameObject.SetActive(true);
            nodeSphereTf.SetParent(_parentGraphTf);
        }, nodeSphereTf =>
        {
            nodeSphereTf.gameObject.SetActive(false);
            nodeSphereTf.SetParent(_poolGraphTf);
        }, nodeSphereTf =>
        {
            Destroy(nodeSphereTf.gameObject);
        }, false, 100, 10000);
    }

    private void CreateEdgePool()
    {
        _edgePool = new ObjectPool<LineRenderer>(() =>
        {
            var lineRendererGo = new GameObject();
            lineRendererGo.transform.SetParent(_parentGraphTf);

            var lineRenderer = lineRendererGo.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.material = _lineMat;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return lineRenderer;

        }, lineRenderer =>
        {
            lineRenderer.enabled = true;
            lineRenderer.transform.SetParent(_parentGraphTf);
        }, lineRenderer =>
        {
            lineRenderer.enabled = false;
            lineRenderer.transform.SetParent(_poolGraphTf);
        }, lineRenderer =>
        {
            Destroy(lineRenderer.gameObject);
        }, false, 100, 10000);
    }

    public LabelNodgeUI GetLabelNodge()
    {
        return _labelNodgeUIPool.Get();
    }

    public Transform GetNode()
    {
        return _nodePool.Get();
    }

    public LineRenderer GetEdge()
    {
        return _edgePool.Get();
    }

    public void Release(LabelNodgeUI lNodge)
    {
        _labelNodgeUIPool.Release(lNodge);
    }
    public void Release(Transform nodeTf)
    {
        _nodePool.Release(nodeTf);
    }
    public void Release(LineRenderer line)
    {
        _edgePool.Release(line);
    }

}
