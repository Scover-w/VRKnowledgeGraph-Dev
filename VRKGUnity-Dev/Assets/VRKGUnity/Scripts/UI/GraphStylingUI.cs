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
    [Header("Graph Size")]
    [Range(.1f,50f)]
    public float MegaGraphSize = 1f;
    [Range(.1f,5f)]
    public float MiniGraphSize = 1f;

    [Header("Node Size")]
    [Range(0f, 10f)]
    public float MegaNodeSize = .2f;
    [Range(0f, 1f)]
    public float MiniNodeSize = .2f;

    [Space(5)]
    [Range(0f, 1f)]
    public float MegaNodeMaxSize = .2f;
    [Range(0f, 1f)]
    public float MegaNodeMinSize = .2f;
    [Space(5)]
    [Range(0f, 1f)]
    public float MiniNodeMaxSize = .2f;
    [Range(0f, 1f)]
    public float MiniNodeMinSize = .2f;


    [Header("Node Color")]
    public Color NodeColor;

    public Color NodeMappingAColor;
    public Color NodeMappingBColor;
    public Color NodeMappingCColor;

    [Range(0f,1f)]
    public float BoundaryColorA;
    [Range(0f,1f)]
    public float BoundaryColorB;
    [Range(0f, 1f)]
    public float BoundaryColorC;

    [Range(1, 15)]
    public int NbOntologyColor;
    [Range(0, 15)]
    public int MaxDeltaOntologyAlgo;
    [Range(0f, 1f)]
    public float SaturationOntologyColor;
    [Range(0f, 1f)]
    public float ValueOntologyColor;

    public Color NodeColorNoOntology;

    [Header("Edge")]
    public Color EdgeColor;

    public float MegaEdgeThickness;
    public float MiniEdgeThickness;


    public bool CanSelectEdges;

    [Header("SelectedMetrics")]
    public GraphMetricType SelectedMetricTypeSize = GraphMetricType.None;
    public GraphMetricType SelectedMetricTypeColor = GraphMetricType.None;

    GraphConfiguration _graphConfig;

    async void Start()
    {
        _graphConfig = await _graphConfigContainerSO.GetGraphConfiguration();

        MegaNodeSize = _graphConfig.NodeSizeMegaGraph;
        MiniNodeSize = _graphConfig.NodeSizeMiniGraph;


        MegaNodeMaxSize = _graphConfig.NodeMaxSizeMegaGraph;
        MegaNodeMinSize = _graphConfig.NodeMinSizeMegaGraph;

        MiniNodeMaxSize = _graphConfig.NodeMaxSizeMiniGraph;
        MiniNodeMinSize = _graphConfig.NodeMinSizeMiniGraph;

        NodeColor = _graphConfig.NodeColor;

        var colorLerpMapper = _graphConfig.NodeColorMapping;
        NodeMappingAColor = colorLerpMapper.ColorA;
        NodeMappingBColor = colorLerpMapper.ColorB;
        NodeMappingCColor = colorLerpMapper.ColorC;

        BoundaryColorA = colorLerpMapper.BoundaryColorA;
        BoundaryColorB = colorLerpMapper.BoundaryColorB;
        BoundaryColorC = colorLerpMapper.BoundaryColorC;


        NbOntologyColor = _graphConfig.NbOntologyColor;
        MaxDeltaOntologyAlgo = _graphConfig.MaxDeltaOntologyAlgo;
        SaturationOntologyColor = _graphConfig.SaturationOntologyColor;
        ValueOntologyColor = _graphConfig.ValueOntologyColor;

        NodeColorNoOntology = _graphConfig.NodeColorNoOntology;


        SelectedMetricTypeColor = _graphConfig.SelectedMetricTypeColor;
        SelectedMetricTypeSize = _graphConfig.SelectedMetricTypeSize;

        MegaGraphSize = _graphConfig.MegaGraphSize;
        MiniGraphSize = _graphConfig.MiniGraphSize;

        EdgeColor = _graphConfig.EdgeColor;

        MegaEdgeThickness = _graphConfig.EdgeThicknessMegaGraph;
        MiniEdgeThickness = _graphConfig.EdgeThicknessMiniGraph;

        CanSelectEdges = _graphConfig.CanSelectEdges;
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

        if (MegaNodeMaxSize < MegaNodeMinSize)
            MegaNodeMaxSize = MegaNodeMinSize;

        if(MegaNodeMinSize > MegaNodeMaxSize)
            MegaNodeMinSize = MegaNodeMaxSize;

        _graphConfig.NodeSizeMegaGraph = MegaNodeSize;
        _graphConfig.NodeSizeMiniGraph = MiniNodeSize;


        _graphConfig.NodeMaxSizeMegaGraph = MegaNodeMaxSize;
        _graphConfig.NodeMinSizeMegaGraph = MegaNodeMinSize;

        _graphConfig.NodeColor = NodeColor;

        var colorLerpMapper = _graphConfig.NodeColorMapping;
        colorLerpMapper.ColorA = NodeMappingAColor;
        colorLerpMapper.ColorB = NodeMappingBColor;
        colorLerpMapper.ColorC = NodeMappingCColor;

        colorLerpMapper.BoundaryColorA = BoundaryColorA;
        colorLerpMapper.BoundaryColorB = BoundaryColorB;
        colorLerpMapper.BoundaryColorC = BoundaryColorC;


        _graphConfig.NbOntologyColor = NbOntologyColor;
        _graphConfig.MaxDeltaOntologyAlgo = MaxDeltaOntologyAlgo;
        _graphConfig.SaturationOntologyColor = SaturationOntologyColor;
        _graphConfig.ValueOntologyColor = ValueOntologyColor; 

        _graphConfig.NodeColorNoOntology = NodeColorNoOntology;

        _graphConfig.SelectedMetricTypeColor = SelectedMetricTypeColor;
        _graphConfig.SelectedMetricTypeSize = SelectedMetricTypeSize;

        _graphConfig.MegaGraphSize = MegaGraphSize;
        _graphConfig.MiniGraphSize = MiniGraphSize;

        _graphConfig.EdgeColor = EdgeColor;

        _graphConfig.EdgeThicknessMegaGraph = MegaEdgeThickness;
        _graphConfig.EdgeThicknessMiniGraph = MiniEdgeThickness;

        _graphConfig.CanSelectEdges = CanSelectEdges;

        UpdateGraph();
    }
}
