using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class NodgePool : MonoBehaviour
{
    // TODO : Need to convert it to SO
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    GameObject _nameCanvasPf;

    [SerializeField]
    GameObject _nodePf;

    [SerializeField]
    GameObject _edgePf;

    [SerializeField]
    Material _lineMat;

    ObjectPool<LabelNodgeUI> _labelNodgeUIPool;
    ObjectPool<NodeStyler> _nodePool;
    ObjectPool<EdgeStyler> _edgePool;

    static NodgePool _instance;

    Transform _poolGraphTf;
    Transform _labelParentTf;


    private void Start()
    {

        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(this);
            return;
        }

        _poolGraphTf = new GameObject("Pool Graph").transform;
        _labelParentTf = new GameObject("LabelParents").transform;

        CreateLabelNodgePool();
        CreateEdgePool();
        CreateNodePool();
    }

    private void CreateLabelNodgePool()
    {
        //TODO : performance, maybe better non-monobehavior
        _labelNodgeUIPool = new ObjectPool<LabelNodgeUI>(() =>
        {
            var labelCanvas = Instantiate(_nameCanvasPf);
            labelCanvas.transform.SetParent(_labelParentTf);
            var labelNodgeUI = labelCanvas.GetComponent<LabelNodgeUI>();
            return labelNodgeUI;
        }, labelCanvas =>
        {
            labelCanvas.SetActive(true);
            labelCanvas.transform.SetParent(_labelParentTf);
        }, labelCanvas =>
        {
            labelCanvas.SetActive(false);
            labelCanvas.Transform.SetParent(_poolGraphTf);
        }, labelCanvas =>
        {
            labelCanvas.Destroy();
        }, false, 100, 10000);
    }

    private void CreateNodePool()
    {
        _nodePool = new ObjectPool<NodeStyler>(() =>
        {
            var nodeSphereTf = Instantiate(_nodePf).transform;
            var nodeStyler = nodeSphereTf.GetComponent<NodeStyler>();
            nodeSphereTf.localScale = new Vector3(.2f, .2f, .2f);
            return nodeStyler;

        }, nodeStyler =>
        {
            nodeStyler.gameObject.SetActive(true);
        }, nodeStyler =>
        {
            nodeStyler.gameObject.SetActive(false);
            nodeStyler.Tf.SetParent(_poolGraphTf);
        }, nodeStyler =>
        {
            Destroy(nodeStyler.gameObject);
        }, false, 100, 10000);
    }

    private void CreateEdgePool()
    {
        _edgePool = new ObjectPool<EdgeStyler>(() =>
        {
            var edgeTf = Instantiate(_edgePf).transform;
            var edgeStyler = edgeTf.GetComponent<EdgeStyler>();
            return edgeStyler;

        }, lineRenderer =>
        {
            lineRenderer.gameObject.SetActive(true);
        }, lineRenderer =>
        {
            lineRenderer.gameObject.SetActive(false);
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

    public NodeStyler GetNodeStyler()
    {
        return _nodePool.Get();
    }

    public EdgeStyler GetEdgeStyler()
    {
        return _edgePool.Get();
    }

    public void Release(LabelNodgeUI lNodge)
    {
        _labelNodgeUIPool.Release(lNodge);
    }
    public void Release(NodeStyler nodeStyler)
    {
        _nodePool.Release(nodeStyler);
    }
    public void Release(EdgeStyler edgeStyler)
    {
        _edgePool.Release(edgeStyler);
    }

}
