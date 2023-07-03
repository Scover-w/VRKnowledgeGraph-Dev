using AngleSharp.Text;
using Unity.Collections.LowLevel.Unsafe;
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


    bool _isInPropagation = false;

    #region Style
    public void StyleEdgeBeforeFirstSimu(StyleChange styleChange, GraphMode graphMode)
    {
        StyleEdge(styleChange, graphMode, true);
    }

    public void StyleEdge(StyleChange styleChange, GraphMode graphMode, bool inSimulation = false)
    {
        if (GraphType == GraphType.Main && !styleChange.HasChanged(StyleChangeType.MainGraph))
            return;

        if (GraphType == GraphType.Sub && !styleChange.HasChanged(StyleChangeType.SubGraph))
            return;



        if (styleChange.HasChanged(StyleChangeType.Color) || styleChange.HasChanged(StyleChangeType.Visibility))
            StyleColor();

        if (styleChange.HasChanged(StyleChangeType.Size))
            StyleSize(styleChange, graphMode);

        if (styleChange.HasChanged(StyleChangeType.Position))
        {
            if (!inSimulation)
                StylePosition(styleChange, graphMode);
        }
    }

    private void StyleColor()
    {
        var color = _isInPropagation? GraphConfiguration.PropagatedEdgeColor : GraphConfiguration.EdgeColor;

        color.a = GraphConfiguration.DisplayEdges ? 1f : 0f;

        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;

        //_lineRenderer.sharedMaterial.color = color; // TODO : Temporary solution
    }

    private void StyleSize(StyleChange styleChange, GraphMode graphMode)
    {
        if(graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode))
        {
            if (GraphType == GraphType.Main)
            {
                SetThickness(GraphConfiguration.EdgeThicknessDesk);
            }
            else if(GraphType == GraphType.Sub)
            {
                SetThickness(GraphConfiguration.EdgeThicknessLens);
            }
        }


        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChangeType.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
            {
                SetThickness(GraphConfiguration.EdgeThicknessImmersion);
            }
            else if (GraphType == GraphType.Sub)
            {
                SetThickness(GraphConfiguration.EdgeThicknessWatch);
            }
        }
    }

    private void StylePosition(StyleChange styleChange, GraphMode graphMode)
    {  
        if(graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode))
        {
            if (GraphType == GraphType.Main)
            {
                SetPosition(GraphConfiguration.DeskGraphSize);
            }
            else if(GraphType == GraphType.Sub)
            {
                SetPosition(GraphConfiguration.LensGraphSize);
            }

        }

        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChangeType.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
            {
                SetPosition(GraphConfiguration.ImmersionGraphSize);
            }
            else if (GraphType == GraphType.Sub)
            {
                SetPosition(GraphConfiguration.WatchGraphSize);
            }
        }
    }


    public void SetColliderAfterEndSimu(GraphMode graphMode)
    {
        //StylePosition(styleChange, graphMode);
    }

    private void SetThickness(float thickness)
    {
        _lineRenderer.startWidth = thickness;
        _lineRenderer.endWidth = thickness;

        //_collider.radius = thickness;
    }

    private void SetPosition(float scalingFactor)
    {
        var positionA = Edge.Source.AbsolutePosition * scalingFactor;
        var positionB = Edge.Target.AbsolutePosition * scalingFactor;

        _lineRenderer.SetPosition(0, positionA);
        _lineRenderer.SetPosition(1, positionB);


        //_colliderTf.localPosition = Vector3.Lerp(positionA, positionB, 0.5f);
        //_collider.height = (positionB - positionA).magnitude;

        //Vector3 worldPositionB = _tf.parent.TransformPoint(positionB);
        //_colliderTf.LookAt(worldPositionB);
    }

    public void StyleTransitionEdge(float t, bool isNextDesk)
    {
        if (GraphType == GraphType.Sub)
            return;

        StyleTransitionPosition(t, isNextDesk);
        StyleTransitionThickness(t, isNextDesk);
    }


    private void StyleTransitionPosition(float t, bool isNextDesk)
    {
        float scale = isNextDesk ? Mathf.Lerp(GraphConfiguration.ImmersionGraphSize, GraphConfiguration.DeskGraphSize, t) :
                                    Mathf.Lerp(GraphConfiguration.DeskGraphSize, GraphConfiguration.ImmersionGraphSize, t);
        SetPosition(scale);
    }

    private void StyleTransitionThickness(float t, bool isNextDesk)
    {
        float thickness = isNextDesk ? Mathf.Lerp(GraphConfiguration.EdgeThicknessImmersion, GraphConfiguration.EdgeThicknessDesk, t) :
                                    Mathf.Lerp(GraphConfiguration.EdgeThicknessDesk, GraphConfiguration.EdgeThicknessImmersion, t);

        SetThickness(thickness);
    }


    #endregion

    public void SetPropagation(bool isInPropagation)
    {
        _isInPropagation = isInPropagation;
        StyleColor();
    }
}
