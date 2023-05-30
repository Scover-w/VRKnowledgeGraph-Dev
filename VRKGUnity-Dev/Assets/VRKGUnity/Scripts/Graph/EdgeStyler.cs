using System.Collections;
using System.Collections.Generic;
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
        StyleEdge(true);
    }

    public void StyleEdge(bool inSimulation)
    {
        _collider.enabled = GraphConfiguration.CanSelectEdges;

        StyleColor();
        StyleSize();

        if (!inSimulation)
            StylePosition();
    }

    private void StyleColor()
    {
        var color = GraphConfiguration.EdgeColor;

        color.a = GraphConfiguration.CanSelectEdges ? 1f : 0f;
        _lineRenderer.sharedMaterial.color = color; // TODO : Temporary solution
    }

    private void StyleSize()
    {
        var thickness = (GraphType == GraphType.Mega)? GraphConfiguration.EdgeThicknessMegaGraph 
                                                        : GraphConfiguration.EdgeThicknessMiniGraph;

        _lineRenderer.startWidth = thickness;
        _lineRenderer.endWidth = thickness;

        _collider.radius = thickness;
    }

    private void StylePosition()
    {
        float scalingFactor = (GraphType == GraphType.Mega) ? GraphConfiguration.MegaGraphSize
                                                                : GraphConfiguration.MiniGraphSize;

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
