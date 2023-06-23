using UnityEngine;

public class EdgeStyler : MonoBehaviour
{
    public Transform Tf { get { return _tf; } }
    public Transform ColliderTf { get { return _colliderTf; } }

    public Edge Edge { get; set; }

    public LineRenderer LineRenderer 
    {
        get 
        { 
            return _lineRenderer; 
        }
    } 

    public GraphType GraphType { private get; set; }

    public static GraphConfiguration GraphConfiguration;

    [SerializeField]
    LineRenderer _lineRenderer;

    [SerializeField]
    Transform _tf;

    [SerializeField]
    CapsuleCollider _collider;

    [SerializeField]
    Transform _colliderTf;


    public void StyleEdgeForFirstTime()
    {
        var styleChange = new StyleChange().Add(StyleChangeType.All);

        StyleEdge(styleChange, true);
    }

    public void StyleEdge(StyleChange styleChange, bool inSimulation)
    {
        if (styleChange.HasChanged(StyleChangeType.Collider))
            _collider.enabled = GraphConfiguration.CanSelectEdges;

        if (styleChange.HasChanged(StyleChangeType.Color))
            StyleColor();

        if (styleChange.HasChanged(StyleChangeType.Size))
            StyleSize();

        if (styleChange.HasChanged(StyleChangeType.Position))
        {
            if (!inSimulation)
                StylePosition();
        }
    }

    private void StyleColor()
    {
        var color = GraphConfiguration.EdgeColor;

        color.a = GraphConfiguration.CanSelectEdges ? 1f : 0f;
        _lineRenderer.sharedMaterial.color = color; // TODO : Temporary solution
    }

    private void StyleSize()
    {
        var thickness = (GraphType == GraphType.Main)? GraphConfiguration.EdgeThicknessMegaGraph 
                                                        : GraphConfiguration.EdgeThicknessMiniGraph;

        _lineRenderer.startWidth = thickness;
        _lineRenderer.endWidth = thickness;

        _collider.radius = thickness;
    }

    private void StylePosition()
    {
        float scalingFactor = (GraphType == GraphType.Main) ? GraphConfiguration.ImmersionGraphSize
                                                                : GraphConfiguration.SubImmersionGraphSize;

        var positionA = Edge.Source.AbsolutePosition * scalingFactor;
        var positionB = Edge.Target.AbsolutePosition * scalingFactor;

        _lineRenderer.SetPosition(0, positionA);
        _lineRenderer.SetPosition(1, positionB);


        _colliderTf.localPosition = Vector3.Lerp(positionA, positionB, 0.5f);
        _collider.height = (positionB - positionA).magnitude;

        Vector3 worldPositionB = _tf.parent.TransformPoint(positionB);
        _colliderTf.LookAt(worldPositionB);
    }


    public void SetColliderAfterEndSimu()
    {
        StylePosition();
    }
}
