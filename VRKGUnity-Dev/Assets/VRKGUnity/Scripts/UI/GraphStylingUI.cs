using UnityEngine;

public class GraphStylingUI : MonoBehaviour
{
    [SerializeField]
    GraphConfigurationContainerSO _graphConfigContainerSO;

    [SerializeField]
    GraphStyling _graphStyling;

    [SerializeField]
    LabelNodgeManagerUI _labelNodgeManagerUI;

    [Space(10)]
    [Header("/!\\ Values will be reset in play mode. \nUseless to change them in Edit Mode.\nChange the values in the SO")]
    [Space(5)]
    [Header("Graph Size")]
    public float ImmersionGraphSize = 1f;
    public float DeskGraphSize = 1f;

    public float GPSGraphSize = 1f;
    public float LensGraphSize = 1f;

    [Header("Node Size")]
    public float NodeSizeImmersion = 1f;
    public float NodeSizeDesk = 1f;

    [Space(5)]
    public float NodeSizeGPS = 1f;
    public float NodeSizeLens = 1f;

    [Space(5)]
    public float NodeMinSizeImmersion = .8f;
    public float NodeMaxSizeImmersion = .8f;

    [Space(5)]
    public float NodeMinSizeDesk = .8f;
    public float NodeMaxSizeDesk = .8f;

    [Space(5)]
    public float NodeMinSizeLens = .8f;
    public float NodeMaxSizeLens = .8f;


    [Header("Label Size")]
    public float LabelNodeSizeImmersion = 1f;
    public float LabelNodeSizeDesk = 1f;

    public float LabelNodeSizeLens = 1f;
    [Space(5)]
    public bool ShowLabelImmersion = true;
    public bool ShowLabelDesk = true;


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


    [Header("Ontology")]
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

    public float EdgeThicknessImmersion = 1f;
    public float EdgeThicknessDesk = 1f;
    public float EdgeThicknessLens = 1f;
    public float EdgeThicknessGPS = 1f;


    public bool CanSelectEdges;

    [Header("SelectedMetrics")]
    public GraphMetricType SelectedMetricTypeSize = GraphMetricType.None;
    public GraphMetricType SelectedMetricTypeColor = GraphMetricType.None;

    [Header("Miscelaneous")]
    public int LabelNodgePropagation = 1;

    public bool ResetPositionNodeOnUpdate = true;

    public int SeedRandomPosition = 0;

    public float GraphModeTransitionTime = 1f;


    GraphConfiguration _graphConfig;

    async void Start()
    {
        _graphConfig = await _graphConfigContainerSO.GetGraphConfiguration();


        NodeSizeImmersion = _graphConfig.NodeSizeImmersion;
        NodeSizeDesk = _graphConfig.NodeSizeDesk;

        NodeSizeGPS = _graphConfig.NodeSizeGPS;
        NodeSizeLens = _graphConfig.NodeSizeLens;

        NodeMinSizeImmersion = _graphConfig.NodeMinSizeImmersion;
        NodeMaxSizeImmersion = _graphConfig.NodeMaxSizeImmersion;

        NodeMinSizeDesk = _graphConfig.NodeMinSizeDesk;
        NodeMaxSizeDesk = _graphConfig.NodeMaxSizeDesk;

        NodeMinSizeLens = _graphConfig.NodeMinSizeLens;
        NodeMaxSizeLens = _graphConfig.NodeMaxSizeLens;



        LabelNodeSizeImmersion = _graphConfig.LabelNodeSizeImmersion;
        LabelNodeSizeDesk = _graphConfig.LabelNodeSizeDesk;

        LabelNodeSizeLens = _graphConfig.LabelNodeSizeLens;

        ShowLabelImmersion = _graphConfig.ShowLabelImmersion;
        ShowLabelDesk = _graphConfig.ShowLabelDesk;


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


        ImmersionGraphSize = _graphConfig.ImmersionGraphSize;
        DeskGraphSize = _graphConfig.DeskGraphSize;

        GPSGraphSize = _graphConfig.GPSGraphSize;
        LensGraphSize = _graphConfig.LensGraphSize;


        EdgeColor = _graphConfig.EdgeColor;

        EdgeThicknessImmersion = _graphConfig.EdgeThicknessImmersion;
        EdgeThicknessDesk = _graphConfig.EdgeThicknessDesk;
        EdgeThicknessLens = _graphConfig.EdgeThicknessLens;
        EdgeThicknessGPS = _graphConfig.EdgeThicknessGPS;

        CanSelectEdges = _graphConfig.CanSelectEdges;


        LabelNodgePropagation = _graphConfig.LabelNodgePropagation;

        ResetPositionNodeOnUpdate = _graphConfig.ResetPositionNodeOnUpdate;

        SeedRandomPosition = _graphConfig.SeedRandomPosition;
 
        GraphModeTransitionTime = _graphConfig.GraphModeTransitionTime;
}


    public void UpdateGraph(StyleChange styleChange)
    {
        if (_graphConfig == null)
            return;

        if (styleChange.HasChanged(StyleChangeType.Label))
            _labelNodgeManagerUI.StyleLabels(styleChange);
        else
            _graphStyling.StyleGraph(styleChange);

        _graphConfig.Save();

    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        if (_graphConfig == null)
            return;

        


        StyleChange styleChange = new();

        #region NodeSize

        if (NodeMinSizeImmersion > NodeMaxSizeImmersion)
            NodeMinSizeImmersion = NodeMaxSizeImmersion;

        if (NodeMinSizeDesk > NodeMaxSizeDesk)
            NodeMinSizeDesk = NodeMaxSizeDesk;

        if (NodeMinSizeLens > NodeMaxSizeLens)
            NodeMinSizeLens = NodeMaxSizeLens;


        if (_graphConfig.NodeSizeImmersion != NodeSizeImmersion)
        {
            _graphConfig.NodeSizeImmersion = NodeSizeImmersion;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.NodeSizeDesk != NodeSizeDesk)
        {
            _graphConfig.NodeSizeDesk = NodeSizeDesk;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }


        if (_graphConfig.NodeSizeLens != NodeSizeLens)
        {
            _graphConfig.NodeSizeLens = NodeSizeLens;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }


        if (_graphConfig.NodeSizeGPS != NodeSizeGPS)
        {
            _graphConfig.NodeSizeGPS = NodeSizeGPS;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }



        if (_graphConfig.NodeMinSizeImmersion != NodeMinSizeImmersion)
        {
            _graphConfig.NodeMinSizeImmersion = NodeMinSizeImmersion;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.NodeMaxSizeImmersion != NodeMaxSizeImmersion)
        {
            _graphConfig.NodeMaxSizeImmersion = NodeMaxSizeImmersion;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }


        if (_graphConfig.NodeMinSizeDesk != NodeMinSizeDesk)
        {
            _graphConfig.NodeMinSizeDesk = NodeMinSizeDesk;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.NodeMaxSizeDesk != NodeMaxSizeDesk)
        {
            _graphConfig.NodeMaxSizeDesk = NodeMaxSizeDesk;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }


        if (_graphConfig.NodeMinSizeLens != NodeMinSizeLens)
        {
            _graphConfig.NodeMinSizeLens = NodeMinSizeLens;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.NodeMaxSizeLens != NodeMaxSizeLens)
        {
            _graphConfig.NodeMaxSizeLens = NodeMaxSizeLens;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }

        #endregion


        #region Color
        if (_graphConfig.NodeColor != NodeColor)
        {
            _graphConfig.NodeColor = NodeColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        var colorLerpMapper = _graphConfig.NodeColorMapping;

        if (colorLerpMapper.ColorA != NodeMappingAColor)
        {
            colorLerpMapper.ColorA = NodeMappingAColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (colorLerpMapper.ColorB != NodeMappingBColor)
        {
            colorLerpMapper.ColorB = NodeMappingBColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (colorLerpMapper.ColorC != NodeMappingCColor)
        {
            colorLerpMapper.ColorC = NodeMappingCColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }


        if (colorLerpMapper.BoundaryColorA != BoundaryColorA)
        {
            colorLerpMapper.BoundaryColorA = BoundaryColorA;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (colorLerpMapper.BoundaryColorB != BoundaryColorB)
        {
            colorLerpMapper.BoundaryColorB = BoundaryColorB;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (colorLerpMapper.BoundaryColorC != BoundaryColorC)
        {
            colorLerpMapper.BoundaryColorC = BoundaryColorC;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }
        #endregion


        #region Ontology
        if (_graphConfig.NbOntologyColor != NbOntologyColor)
        {
            _graphConfig.NbOntologyColor = NbOntologyColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
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
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.ValueOntologyColor != ValueOntologyColor)
        {
            _graphConfig.ValueOntologyColor = ValueOntologyColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.NodeColorNoOntology != NodeColorNoOntology)
        {
            _graphConfig.NodeColorNoOntology = NodeColorNoOntology;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }
        #endregion


        #region Metrics
        if (_graphConfig.SelectedMetricTypeColor != SelectedMetricTypeColor)
        {
            _graphConfig.SelectedMetricTypeColor = SelectedMetricTypeColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);
        }

        if (_graphConfig.SelectedMetricTypeSize != SelectedMetricTypeSize)
        {
            _graphConfig.SelectedMetricTypeSize = SelectedMetricTypeSize;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);
        }
        #endregion


        #region Graph_Size
        if (_graphConfig.ImmersionGraphSize != ImmersionGraphSize)
        {
            _graphConfig.ImmersionGraphSize = ImmersionGraphSize;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Position);
        }

        if (_graphConfig.GPSGraphSize != GPSGraphSize)
        {
            _graphConfig.GPSGraphSize = GPSGraphSize;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Position);
        }

        if (_graphConfig.DeskGraphSize != DeskGraphSize)
        {
            _graphConfig.DeskGraphSize = DeskGraphSize;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Position);
        }

        if (_graphConfig.LensGraphSize != LensGraphSize)
        {
            _graphConfig.LensGraphSize = LensGraphSize;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Position);
        }
        #endregion


        #region Edge
        if (_graphConfig.EdgeColor != EdgeColor)
        {
            _graphConfig.EdgeColor = EdgeColor;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Color);
        }


        if (_graphConfig.EdgeThicknessImmersion != EdgeThicknessImmersion)
        {
            _graphConfig.EdgeThicknessImmersion = EdgeThicknessImmersion;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.EdgeThicknessDesk != EdgeThicknessDesk)
        {
            _graphConfig.EdgeThicknessDesk = EdgeThicknessDesk;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.EdgeThicknessLens != EdgeThicknessLens)
        {
            _graphConfig.EdgeThicknessLens = EdgeThicknessLens;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.EdgeThicknessGPS != EdgeThicknessGPS)
        {
            _graphConfig.EdgeThicknessGPS = EdgeThicknessGPS;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.CanSelectEdges != CanSelectEdges)
        {
            _graphConfig.CanSelectEdges = CanSelectEdges;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Edge)
                .Add(StyleChangeType.Color)
                .Add(StyleChangeType.Collider);
        }
        #endregion


        #region LabelNode

        if (_graphConfig.LabelNodeSizeImmersion != LabelNodeSizeImmersion)
        {
            _graphConfig.LabelNodeSizeImmersion = LabelNodeSizeImmersion;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Label)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.LabelNodeSizeDesk != LabelNodeSizeDesk)
        {
            _graphConfig.LabelNodeSizeDesk = LabelNodeSizeDesk;
            styleChange = styleChange.Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Label)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.LabelNodeSizeLens != LabelNodeSizeLens)
        {
            _graphConfig.LabelNodeSizeLens = LabelNodeSizeLens;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Label)
                .Add(StyleChangeType.Size);
        }

        if (_graphConfig.ShowLabelImmersion != ShowLabelImmersion)
        {
            _graphConfig.ShowLabelImmersion = ShowLabelImmersion;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Label)
                .Add(StyleChangeType.Visibility);
        }

        if (_graphConfig.ShowLabelDesk != ShowLabelDesk)
        {
            _graphConfig.ShowLabelDesk = ShowLabelDesk;
            styleChange = styleChange.Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.Label)
                .Add(StyleChangeType.Visibility);
        }

        #endregion


        #region Miscelaneous
        if (_graphConfig.LabelNodgePropagation != LabelNodgePropagation)
        {
            _graphConfig.LabelNodgePropagation = LabelNodgePropagation;
            styleChange = styleChange.Add(StyleChangeType.Propagation);
        }

        if (_graphConfig.ResetPositionNodeOnUpdate != ResetPositionNodeOnUpdate)
        {
            _graphConfig.ResetPositionNodeOnUpdate = ResetPositionNodeOnUpdate;
        }

        if (_graphConfig.SeedRandomPosition != SeedRandomPosition)
        {
            _graphConfig.SeedRandomPosition = SeedRandomPosition;
        }

        if (_graphConfig.GraphModeTransitionTime != GraphModeTransitionTime)
        {
            _graphConfig.GraphModeTransitionTime = GraphModeTransitionTime;
        }

        #endregion



        UpdateGraph(styleChange);
    }
}
