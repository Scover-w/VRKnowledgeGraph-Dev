using AngleSharp.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


/// <summary>
/// Allow to handle the visual, transform part of an Edge (either the main or sub, i.e, the edge of the desk/immersion or lens/gps)
/// </summary>
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


    #region Style
    public void StyleEdgeBeforeFirstSimu(StyleChange styleChange, GraphMode graphMode)
    {
        StyleEdge(styleChange, graphMode, true);
    }

    public void StyleEdge(StyleChange styleChange, GraphMode graphMode, bool inSimulation = false)
    {
        if (GraphType == GraphType.Main && !styleChange.HasChanged(StyleChange.MainGraph))
            return;

        if (GraphType == GraphType.Sub && !styleChange.HasChanged(StyleChange.SubGraph))
            return;



        if (styleChange.HasChanged(StyleChange.Color) || styleChange.HasChanged(StyleChange.Visibility))
            StyleColor();

        if (styleChange.HasChanged(StyleChange.Size))
            StyleSize(styleChange, graphMode);

        if (styleChange.HasChanged(StyleChange.Position))
        {
            if (!inSimulation)
                StylePosition(styleChange, graphMode);
        }
    }

    private void StyleColor()
    {
        bool isEdgePropagated = Edge.IsPropagated;
        var color = isEdgePropagated ? GraphConfiguration.PropagatedEdgeColor : GraphConfiguration.EdgeColor;

        if (GraphConfiguration.DisplayEdges)
            color.a = isEdgePropagated ? GraphConfiguration.AlphaEdgePropagated : GraphConfiguration.AlphaEdgeUnPropagated;
        else
            color.a =  0f;

        _lineRenderer.colorGradient = CreateGradient(color);
    }

    private Gradient CreateGradient(Color color)
    {
        var gradient = new Gradient();

        // Set up the color keys
        var colorKey = new GradientColorKey[2];
        colorKey[0].color = color;
        colorKey[0].time = 0.2f;


        colorKey[1].color = color;
        colorKey[1].time = .8f;

        // Set up the alpha keys
        var alphaKey = new GradientAlphaKey[2];

        alphaKey[0].alpha = color.a;
        alphaKey[0].time = .2f;
        alphaKey[1].alpha = (Edge.EdgeDirection == EdgeDirection.Both) ?  color.a : 0f;
        alphaKey[1].time = .8f;

        // Apply the color and alpha keys to the gradient
        gradient.SetKeys(colorKey, alphaKey);

        return gradient;
    }

    private void StyleSize(StyleChange styleChange, GraphMode graphMode)
    {
        if(graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChange.DeskMode))
        {
            if (GraphType == GraphType.Main)
            {
                SetThickness(GraphConfiguration.EffectiveEdgeThicknessDesk);
            }
            else if(GraphType == GraphType.Sub)
            {
                SetThickness(GraphConfiguration.EffectiveEdgeThicknessLens);
            }
        }


        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChange.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
            {
                SetThickness(GraphConfiguration.EffectiveEdgeThicknessImmersion);
            }
            else if (GraphType == GraphType.Sub)
            {
                SetThickness(GraphConfiguration.EffectiveEdgeThicknessGPS);
            }
        }
    }

    private void StylePosition(StyleChange styleChange, GraphMode graphMode)
    {  
        if(graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChange.DeskMode))
        {
            if (GraphType == GraphType.Main)
            {
                SetPosition(GraphConfiguration.EffectiveDeskGraphSize);
            }
            else if(GraphType == GraphType.Sub)
            {
                SetPosition(GraphConfiguration.EffectiveLensGraphSize);
            }

        }

        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChange.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
            {
                SetPosition(GraphConfiguration.EffectiveImmersionGraphSize);
            }
            else if (GraphType == GraphType.Sub)
            {
                SetPosition(GraphConfiguration.EffectiveGPSGraphSize);
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

        Vector3 direction = positionB - positionA;

        _lineRenderer.SetPosition(0, positionA);
        _lineRenderer.SetPosition(1, positionA + direction * .2f);
        _lineRenderer.SetPosition(2, positionA + direction * .8f);
        _lineRenderer.SetPosition(3, positionB);


        //_colliderTf.localPosition = Vector3.Lerp(positionA, positionB, 0.5f);
        //_collider.height = (positionB - positionA).magnitude;

        //Vector3 worldPositionB = _tf.parent.TransformPoint(positionB);
        //_colliderTf.LookAt(worldPositionB);
    }

    public void SetSourcePositionFromMovingNode(Vector3 worldPosition)
    {
        var positionA = _tf.InverseTransformPoint(worldPosition);
        var positionB = _lineRenderer.GetPosition(3);//_tf.InverseTransformPoint(_lineRenderer.GetPosition(3));

        Vector3 direction = positionB - positionA;

        _lineRenderer.SetPosition(0, positionA);
        _lineRenderer.SetPosition(1, positionA + direction * .2f);
        _lineRenderer.SetPosition(2, positionA + direction * .8f);
        _lineRenderer.SetPosition(3, positionB);
    }

    public void SetTargetPositionFromMovingNode(Vector3 worldPosition)
    {
        var positionA = _lineRenderer.GetPosition(0);//_tf.InverseTransformPoint(_lineRenderer.GetPosition(0));
        var positionB = _tf.InverseTransformPoint(worldPosition);

        Vector3 direction = positionB - positionA;

        _lineRenderer.SetPosition(0, positionA);
        _lineRenderer.SetPosition(1, positionA + direction * .2f);
        _lineRenderer.SetPosition(2, positionA + direction * .8f);
        _lineRenderer.SetPosition(3, positionB);
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
        float scale = isNextDesk ? Mathf.Lerp(GraphConfiguration.EffectiveImmersionGraphSize, GraphConfiguration.EffectiveDeskGraphSize, t) :
                                    Mathf.Lerp(GraphConfiguration.EffectiveDeskGraphSize, GraphConfiguration.EffectiveImmersionGraphSize, t);
        SetPosition(scale);
    }

    private void StyleTransitionThickness(float t, bool isNextDesk)
    {
        float thickness = isNextDesk ? Mathf.Lerp(GraphConfiguration.EffectiveEdgeThicknessImmersion, GraphConfiguration.EffectiveEdgeThicknessDesk, t) :
                                    Mathf.Lerp(GraphConfiguration.EffectiveEdgeThicknessDesk, GraphConfiguration.EffectiveEdgeThicknessImmersion, t);

        SetThickness(thickness);
    }


    #endregion

    public void SetPropagation()
    {
        StyleColor();
    }
}
