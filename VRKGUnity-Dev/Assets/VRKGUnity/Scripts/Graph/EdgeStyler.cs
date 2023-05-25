using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeStyler : MonoBehaviour
{
    public Transform Tf { get { return _tf; } }

    public Edge Edge { get; set; }

    public LineRenderer LineRenderer 
    {
        get 
        { 
            return _lineRenderer; 
        }
    } 

    public static GraphConfiguration GraphConfiguration;

    [SerializeField]
    LineRenderer _lineRenderer;

    [SerializeField]
    Transform _tf;

    [SerializeField]
    CapsuleCollider _collider;

    [SerializeField]
    Transform _colliderTf;


    public void StyleEdge(bool inSimulation)
    {
        StyleColor();
        StyleSize();

        if (!inSimulation)
            StylePosition();
    }

    private void StyleColor()
    {
        var color = GraphConfiguration.EdgeColor;
        _lineRenderer.sharedMaterial.color = color; // TODO : Temporary solution
    }

    private void StyleSize()
    {
        var thickness = GraphConfiguration.EdgeThicknessBigGraph;

        _lineRenderer.startWidth = thickness;
        _lineRenderer.endWidth = thickness;

        _collider.radius = thickness;
    }

    private void StylePosition()
    {
        float scalingFactor = GraphConfiguration.BigGraphSize;

        var positionA = Edge.Source.Position * scalingFactor;
        var positionB = Edge.Target.Position * scalingFactor;

        _lineRenderer.SetPosition(0, positionA);
        _lineRenderer.SetPosition(1, positionB);


        _colliderTf.position = Vector3.Lerp(positionA, positionB, 0.5f);
        _collider.height = (positionB - positionA).magnitude;

        _colliderTf.LookAt(positionB);
    }


    public void SetColliderAfterEndSimu()
    {
        StylePosition();
    }
}
