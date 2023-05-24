using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class NodeStyler : MonoBehaviour
{
    public Transform Tf { get { return _tf; } }

    public Node Node { get; set; }

    public static GraphConfiguration GraphConfiguration;

    [SerializeField]
    Transform _tf;

    [SerializeField]
    MeshRenderer _renderer;

    [SerializeField]
    Material _defaultMat;

    [SerializeField]
    Material _hoveredMat;

    [SerializeField]
    Material _selectedMat;

    bool _isHovered = false;
    bool _isSelected = false;


    private void OnEnable()
    {
        _isHovered = false;
        _isSelected = false;

        _renderer.sharedMaterial = _defaultMat;
    }

    public void OnEnterHover(HoverEnterEventArgs args)
    {
        _isHovered = true;
        UpdateMaterial();
    }

    public void OnExitHover(HoverExitEventArgs args)
    {
        _isHovered = false;
        UpdateMaterial();
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        _isHovered = true;
        UpdateMaterial();
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        _isHovered = false;
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if(_isSelected)
            _renderer.sharedMaterial = _selectedMat;
        else if(_isHovered)
            _renderer.sharedMaterial = _hoveredMat;
        else
            _renderer.sharedMaterial = _defaultMat;
    }

    public void StyleNodeForFirstTime()
    {
        _renderer.material.color = GraphConfiguration.NodeColor;
        float scale = GraphConfiguration.NodeSizeBigGraph;
        _tf.localScale = new Vector3(scale, scale, scale);
    }


    public void StyleNode()
    {
        StyleColor();
        StyleSize();
    }

    public void StyleColor()
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeColor;
        if (selectedMetricType == GraphMetricType.None)
        {
            _renderer.material.color = GraphConfiguration.NodeColor;
            return;
        }

        float value = 0;

        switch (selectedMetricType)
        {
            case GraphMetricType.AverageShortestPath:
                value = Node.AverageShortestPathLength;
                break;
            case GraphMetricType.BetweennessCentrality:
                value = Node.BetweennessCentrality;
                break;
            case GraphMetricType.ClosenessCentrality:
                value = Node.ClosenessCentrality;
                break;
            case GraphMetricType.ClusteringCoefficient:
                value = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                value = Node.Degree;
                break;
        }

        Material copyMaterial = new Material(_renderer.material);
        copyMaterial.color = Color.Lerp(GraphConfiguration.NodeMappingAColor, GraphConfiguration.NodeMappingBColor, value);
        _renderer.material = copyMaterial;

        //_renderer.material.color = Color.Lerp(GraphConfiguration.EdgeMappingAColor, GraphConfiguration.EdgeMappingBColor, value);
    }

    public void StyleSize()
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeSize;

        if (selectedMetricType == GraphMetricType.None)
        {
            float scale = GraphConfiguration.NodeSizeBigGraph;
            _tf.localScale = new Vector3(scale, scale, scale);
            return;
        }

        float scaleB = 0;

        switch (selectedMetricType)
        {
            case GraphMetricType.AverageShortestPath:
                scaleB = Node.AverageShortestPathLength;
                break;
            case GraphMetricType.BetweennessCentrality:
                scaleB = Node.BetweennessCentrality;
                break;
            case GraphMetricType.ClosenessCentrality:
                scaleB = Node.ClosenessCentrality;
                break;
            case GraphMetricType.ClusteringCoefficient:
                scaleB = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                scaleB = Node.Degree;
                break;
        }

        scaleB = Mathf.Lerp(GraphConfiguration.NodeMinSizeBigGraph, GraphConfiguration.NodeMaxSizeBigGraph, scaleB);

        _tf.localScale = new Vector3(scaleB, scaleB, scaleB);
    }
}
