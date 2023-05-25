using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphStylingUI : MonoBehaviour
{
    [SerializeField]
    GraphConfigurationContainerSO _graphConfigContainerSO;

    [SerializeField]
    GraphStyling _graphStyling;

    [Space(10)]
    [Header("/!\\ Values will be reset in play mode. \nUseless to change them in Edit Mode.\nChange the values in the SO")]
    [Space(5)]
    [Header("Node Size")]
    [Range(0f, 1f)]
    public float NodeSize = .2f;


    [Range(0f, 1f)]
    public float NodeMaxSize = .2f;
    [Range(0f, 1f)]
    public float NodeMinSize = .2f;


    [Header("Node Color")]
    public Color NodeColor;

    public Color NodeMappingAColor;
    public Color NodeMappingBColor;

    [Header("SelectedMetrics")]
    public GraphMetricType SelectedMetricTypeSize = GraphMetricType.None;
    public GraphMetricType SelectedMetricTypeColor = GraphMetricType.None;

    GraphConfiguration _graphConfig;

    async void Start()
    {
        _graphConfig = await _graphConfigContainerSO.GetGraphConfiguration();

        NodeSize = _graphConfig.NodeSizeBigGraph;
        NodeMaxSize = _graphConfig.NodeMaxSizeBigGraph;
        NodeMinSize = _graphConfig.NodeMinSizeBigGraph;

        NodeColor = _graphConfig.NodeColor;
        NodeMappingAColor = _graphConfig.NodeMappingAColor;
        NodeMappingBColor = _graphConfig.NodeMappingBColor;

        SelectedMetricTypeColor = _graphConfig.SelectedMetricTypeColor;
        SelectedMetricTypeSize = _graphConfig.SelectedMetricTypeSize;
    }


    public void UpdateGraph()
    {
        if (_graphConfig == null)
            return;

        _graphStyling.StyleGraph();
        _graphConfig.Save();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        if (_graphConfig == null)
            return;

        if (NodeMaxSize < NodeMinSize)
            NodeMaxSize = NodeMinSize;

        if(NodeMinSize > NodeMaxSize)
            NodeMinSize = NodeMaxSize;

        _graphConfig.NodeSizeBigGraph = NodeSize;
        _graphConfig.NodeMaxSizeBigGraph = NodeMaxSize;
        _graphConfig.NodeMinSizeBigGraph = NodeMinSize;

        _graphConfig.NodeColor = NodeColor;
        _graphConfig.NodeMappingAColor = NodeMappingAColor;
        _graphConfig.NodeMappingBColor = NodeMappingBColor;

        _graphConfig.SelectedMetricTypeColor = SelectedMetricTypeColor;
        _graphConfig.SelectedMetricTypeSize = SelectedMetricTypeSize;

        UpdateGraph();
    }
}
