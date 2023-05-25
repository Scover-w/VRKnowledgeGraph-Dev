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
    }

    private void StylePosition()
    {
        float scalingFactor = GraphConfiguration.BigGraphSize;

        _lineRenderer.SetPosition(0, Edge.Source.Position * scalingFactor);
        _lineRenderer.SetPosition(1, Edge.Target.Position * scalingFactor);
    }
}
