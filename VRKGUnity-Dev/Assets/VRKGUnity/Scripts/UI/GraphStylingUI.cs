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

        MegaGraphSize = _graphConfig.ImmersionGraphSize;
        MiniGraphSize = _graphConfig.SubImmersionGraphSize;

        EdgeColor = _graphConfig.EdgeColor;

        MegaEdgeThickness = _graphConfig.EdgeThicknessMegaGraph;
        MiniEdgeThickness = _graphConfig.EdgeThicknessMiniGraph;

        CanSelectEdges = _graphConfig.CanSelectEdges;
    }


    public void UpdateGraph(StyleChange styleChange)
    {
        if (_graphConfig == null)
            return;

        _graphStyling.StyleGraph(styleChange);
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


        StyleChange styleChange = new();

        if(_graphConfig.NodeSizeMegaGraph != MegaNodeSize)
        {
            _graphConfig.NodeSizeMegaGraph = MegaNodeSize;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if(_graphConfig.NodeSizeMiniGraph != MiniNodeSize)
        {
            _graphConfig.NodeSizeMiniGraph = MiniNodeSize;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }


        if (_graphConfig.NodeMaxSizeMegaGraph != MegaNodeMaxSize)
        {
            _graphConfig.NodeMaxSizeMegaGraph = MegaNodeMaxSize;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.NodeMinSizeMegaGraph != MegaNodeMinSize)
        {
            _graphConfig.NodeMinSizeMegaGraph = MegaNodeMinSize;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.NodeMaxSizeMiniGraph != MiniNodeMaxSize)
        {
            _graphConfig.NodeMaxSizeMiniGraph = MiniNodeMaxSize;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.NodeMinSizeMiniGraph != MiniNodeMinSize)
        {
            _graphConfig.NodeMinSizeMiniGraph = MiniNodeMinSize;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.NodeColor != NodeColor)
        {
            _graphConfig.NodeColor = NodeColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        var colorLerpMapper = _graphConfig.NodeColorMapping;

        if (colorLerpMapper.ColorA != NodeMappingAColor)
        {
            colorLerpMapper.ColorA = NodeMappingAColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (colorLerpMapper.ColorB != NodeMappingBColor)
        {
            colorLerpMapper.ColorB = NodeMappingBColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (colorLerpMapper.ColorC != NodeMappingCColor)
        {
            colorLerpMapper.ColorC = NodeMappingCColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }


        if (colorLerpMapper.BoundaryColorA != BoundaryColorA)
        {
            colorLerpMapper.BoundaryColorA = BoundaryColorA;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (colorLerpMapper.BoundaryColorB != BoundaryColorB)
        {
            colorLerpMapper.BoundaryColorB = BoundaryColorB;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (colorLerpMapper.BoundaryColorC != BoundaryColorC)
        {
            colorLerpMapper.BoundaryColorC = BoundaryColorC;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }


        if (_graphConfig.NbOntologyColor != NbOntologyColor)
        {
            _graphConfig.NbOntologyColor = NbOntologyColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.MaxDeltaOntologyAlgo != MaxDeltaOntologyAlgo)
        {
            _graphConfig.MaxDeltaOntologyAlgo = MaxDeltaOntologyAlgo;
        }

        if (_graphConfig.SaturationOntologyColor != SaturationOntologyColor)
        {
            _graphConfig.SaturationOntologyColor = SaturationOntologyColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.ValueOntologyColor != ValueOntologyColor)
        {
            _graphConfig.ValueOntologyColor = ValueOntologyColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.NodeColorNoOntology != NodeColorNoOntology)
        {
            _graphConfig.NodeColorNoOntology = NodeColorNoOntology;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.SelectedMetricTypeColor != SelectedMetricTypeColor)
        {
            _graphConfig.SelectedMetricTypeColor = SelectedMetricTypeColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.SelectedMetricTypeSize != SelectedMetricTypeSize)
        {
            _graphConfig.SelectedMetricTypeSize = SelectedMetricTypeSize;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.ImmersionGraphSize != MegaGraphSize)
        {
            _graphConfig.ImmersionGraphSize = MegaGraphSize;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Position);
        }

        if (_graphConfig.SubImmersionGraphSize != MiniGraphSize)
        {
            _graphConfig.SubImmersionGraphSize = MiniGraphSize;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Position);
        }

        if (_graphConfig.EdgeColor != EdgeColor)
        {
            _graphConfig.EdgeColor = EdgeColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.EdgeThicknessMegaGraph != MegaEdgeThickness)
        {
            _graphConfig.EdgeThicknessMegaGraph = MegaEdgeThickness;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.EdgeThicknessMiniGraph != MiniEdgeThickness)
        {
            _graphConfig.EdgeThicknessMiniGraph = MiniEdgeThickness;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.CanSelectEdges != CanSelectEdges)
        {
            _graphConfig.CanSelectEdges = CanSelectEdges;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Color)
                .Add(StyleChangeType.Collider);
        }

        UpdateGraph(styleChange);
    }
}
