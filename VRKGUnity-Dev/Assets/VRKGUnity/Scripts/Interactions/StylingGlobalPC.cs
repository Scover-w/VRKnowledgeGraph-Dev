using UnityEngine;

public class StylingGlobalPC : MonoBehaviour
{
    [SerializeField]
    StylingManager _stylingManager;

    [Space(10)]
    [Header("/!\\ Values will be reset in play mode. \nUseless to change them in Edit Mode.\nChange the values in the SO")]
    [Space(5)]
    [Header("Graph Size")]
    public float ImmersionGraphSize = 1f;
    public float DeskGraphSize = 1f;

    public float WatchGraphSize = 1f;
    public float LensGraphSize = 1f;

    [Header("Node Size")]
    public float NodeSizeImmersion = 1f;
    public float NodeSizeDesk = 1f;

    [Space(5)]
    public float NodeSizeWatch = 1f;
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
    public bool ShowLabelLens = true;


    [Header("Node Color")]
    public Color NodeColor;

    public Color NodeMappingAColor;
    public Color NodeMappingBColor;
    public Color NodeMappingCColor;

    [Range(0f, 1f)]
    public float AlphaNodeColorPropagated = 1f;
    [Range(0f, 1f)]
    public float AlphaNodeColorUnPropagated = 1f;

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

    public Color NodeColorNoValueMetric;

    [Header("Edge")]
    public Color EdgeColor;
    public Color PropagatedEdgeColor;

    [Range(0f, 1f)]
    public float AlphaEdgeColorPropagated = 1f;
    [Range(0f, 1f)]
    public float AlphaEdgeColorUnPropagated = 1f;

    public float EdgeThicknessImmersion = 1f;
    public float EdgeThicknessDesk = 1f;
    public float EdgeThicknessLens = 1f;
    public float EdgeThicknessWatch = 1f;


    public bool CanSelectEdges;
    public bool DisplayEdges;

    [Header("SelectedMetrics")]
    public GraphMetricType SelectedMetricTypeSize = GraphMetricType.None;
    public GraphMetricType SelectedMetricTypeColor = GraphMetricType.None;

    [Header("Miscelaneous")]
    public int LabelNodgePropagation = 1;

    public bool ResetPositionNodeOnUpdate = true;

    public int SeedRandomPosition = 0;

    public float GraphModeTransitionTime = 1f;

    public bool DisplayInterSelectedNeighborEdges = false;

    public bool ShowWatch = true;


    GraphConfiguration _graphConfig;

    void Start()
    {
        _graphConfig = GraphConfiguration.Instance;


        NodeSizeImmersion = _graphConfig.NodeSizeImmersion;
        NodeSizeDesk = _graphConfig.NodeSizeDesk;

        NodeSizeWatch = _graphConfig.NodeSizeWatch;
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

        ShowLabelLens = _graphConfig.ShowLabelLens;


        NodeColor = _graphConfig.NodeColor;

        var colorLerpMapper = _graphConfig.NodeColorMapping;
        NodeMappingAColor = colorLerpMapper.ColorA;
        NodeMappingBColor = colorLerpMapper.ColorB;
        NodeMappingCColor = colorLerpMapper.ColorC;

        AlphaNodeColorPropagated = _graphConfig.AlphaNodeColorPropagated;
        AlphaNodeColorUnPropagated = _graphConfig.AlphaNodeColorUnPropagated;

        BoundaryColorA = colorLerpMapper.BoundaryColorA;
        BoundaryColorB = colorLerpMapper.BoundaryColorB;
        BoundaryColorC = colorLerpMapper.BoundaryColorC;


        NbOntologyColor = _graphConfig.NbOntologyColor;
        MaxDeltaOntologyAlgo = _graphConfig.MaxDeltaOntologyAlgo;
        SaturationOntologyColor = _graphConfig.SaturationOntologyColor;
        ValueOntologyColor = _graphConfig.ValueOntologyColor;

        NodeColorNoValueMetric = _graphConfig.NodeColorNoValueMetric;


        SelectedMetricTypeColor = _graphConfig.SelectedMetricTypeColor;
        SelectedMetricTypeSize = _graphConfig.SelectedMetricTypeSize;


        ImmersionGraphSize = _graphConfig.ImmersionGraphSize;
        DeskGraphSize = _graphConfig.DeskGraphSize;

        WatchGraphSize = _graphConfig.WatchGraphSize;
        LensGraphSize = _graphConfig.LensGraphSize;


        EdgeColor = _graphConfig.EdgeColor;
        PropagatedEdgeColor = _graphConfig.PropagatedEdgeColor;

        AlphaEdgeColorPropagated = _graphConfig.AlphaEdgeColorPropagated;
        AlphaEdgeColorUnPropagated = _graphConfig.AlphaEdgeColorUnPropagated;

        EdgeThicknessImmersion = _graphConfig.EdgeThicknessImmersion;
        EdgeThicknessDesk = _graphConfig.EdgeThicknessDesk;
        EdgeThicknessLens = _graphConfig.EdgeThicknessLens;
        EdgeThicknessWatch = _graphConfig.EdgeThicknessWatch;

        CanSelectEdges = _graphConfig.CanSelectEdges;
        DisplayEdges = _graphConfig.DisplayEdges;


        LabelNodgePropagation = _graphConfig.LabelNodgePropagation;

        ResetPositionNodeOnUpdate = _graphConfig.ResetPositionNodeOnUpdate;

        SeedRandomPosition = _graphConfig.SeedRandomPosition;
 
        GraphModeTransitionTime = _graphConfig.GraphModeTransitionTime;

        DisplayInterSelectedNeighborEdges = _graphConfig.DisplayInterSelectedNeighborEdges;

        ShowWatch = _graphConfig.ShowWatch;
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        if (_graphConfig == null)
            return;


        StyleChange styleChange = StyleChange.None;
        StyleChange addedStyleChange = StyleChange.None;

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

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeSizeImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.NodeSizeDesk != NodeSizeDesk)
        {
            _graphConfig.NodeSizeDesk = NodeSizeDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeSizeDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.NodeSizeLens != NodeSizeLens)
        {
            _graphConfig.NodeSizeLens = NodeSizeLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeSizeLens);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.NodeSizeWatch != NodeSizeWatch)
        {
            _graphConfig.NodeSizeWatch = NodeSizeWatch;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeSizeWatch);
            styleChange = styleChange.Add(addedStyleChange);
        }



        if (_graphConfig.NodeMinSizeImmersion != NodeMinSizeImmersion)
        {
            _graphConfig.NodeMinSizeImmersion = NodeMinSizeImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeMinSizeImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.NodeMaxSizeImmersion != NodeMaxSizeImmersion)
        {
            _graphConfig.NodeMaxSizeImmersion = NodeMaxSizeImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeMaxSizeImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.NodeMinSizeDesk != NodeMinSizeDesk)
        {
            _graphConfig.NodeMinSizeDesk = NodeMinSizeDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeMinSizeDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.NodeMaxSizeDesk != NodeMaxSizeDesk)
        {
            _graphConfig.NodeMaxSizeDesk = NodeMaxSizeDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeMaxSizeDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.NodeMinSizeLens != NodeMinSizeLens)
        {
            _graphConfig.NodeMinSizeLens = NodeMinSizeLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeMinSizeLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.NodeMaxSizeLens != NodeMaxSizeLens)
        {
            _graphConfig.NodeMaxSizeLens = NodeMaxSizeLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeMaxSizeLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        #endregion


        #region Color
        if (_graphConfig.NodeColor != NodeColor)
        {
            _graphConfig.NodeColor = NodeColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        var colorLerpMapper = _graphConfig.NodeColorMapping;

        if (colorLerpMapper.ColorA != NodeMappingAColor)
        {
            colorLerpMapper.ColorA = NodeMappingAColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (colorLerpMapper.ColorB != NodeMappingBColor)
        {
            colorLerpMapper.ColorB = NodeMappingBColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (colorLerpMapper.ColorC != NodeMappingCColor)
        {
            colorLerpMapper.ColorC = NodeMappingCColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.AlphaNodeColorPropagated != AlphaNodeColorPropagated)
        {
            _graphConfig.AlphaNodeColorPropagated = AlphaNodeColorPropagated;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.AlphaNodeColorPropagated);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.AlphaNodeColorUnPropagated != AlphaNodeColorUnPropagated)
        {
            _graphConfig.AlphaNodeColorUnPropagated = AlphaNodeColorUnPropagated;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.AlphaNodeColorUnPropagated);
            styleChange = styleChange.Add(addedStyleChange);
        }



        if (colorLerpMapper.BoundaryColorA != BoundaryColorA)
        {
            colorLerpMapper.BoundaryColorA = BoundaryColorA;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (colorLerpMapper.BoundaryColorB != BoundaryColorB)
        {
            colorLerpMapper.BoundaryColorB = BoundaryColorB;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (colorLerpMapper.BoundaryColorC != BoundaryColorC)
        {
            colorLerpMapper.BoundaryColorC = BoundaryColorC;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region Ontology
        if (_graphConfig.NbOntologyColor != NbOntologyColor)
        {
            _graphConfig.NbOntologyColor = NbOntologyColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NbOntologyColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.MaxDeltaOntologyAlgo != MaxDeltaOntologyAlgo)
        {
            _graphConfig.MaxDeltaOntologyAlgo = MaxDeltaOntologyAlgo;
        }

        if (_graphConfig.SaturationOntologyColor != SaturationOntologyColor)
        {
            _graphConfig.SaturationOntologyColor = SaturationOntologyColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.SaturationOntologyColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.ValueOntologyColor != ValueOntologyColor)
        {
            _graphConfig.ValueOntologyColor = ValueOntologyColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.ValueOntologyColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.NodeColorNoValueMetric != NodeColorNoValueMetric)
        {
            _graphConfig.NodeColorNoValueMetric = NodeColorNoValueMetric;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.NodeColorNoValueMetric);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region Metrics
        if (_graphConfig.SelectedMetricTypeColor != SelectedMetricTypeColor)
        {
            _graphConfig.SelectedMetricTypeColor = SelectedMetricTypeColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.SelectedMetricTypeColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.SelectedMetricTypeSize != SelectedMetricTypeSize)
        {
            _graphConfig.SelectedMetricTypeSize = SelectedMetricTypeSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.SelectedMetricTypeSize);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region Graph_Size
        if (_graphConfig.ImmersionGraphSize != ImmersionGraphSize)
        {
            _graphConfig.ImmersionGraphSize = ImmersionGraphSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.ImmersionGraphSize);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.WatchGraphSize != WatchGraphSize)
        {
            _graphConfig.WatchGraphSize = WatchGraphSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.WatchGraphSize);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.DeskGraphSize != DeskGraphSize)
        {
            _graphConfig.DeskGraphSize = DeskGraphSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.DeskGraphSize);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.LensGraphSize != LensGraphSize)
        {
            _graphConfig.LensGraphSize = LensGraphSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.LensGraphSize);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region Edge
        if (_graphConfig.EdgeColor != EdgeColor)
        {
            _graphConfig.EdgeColor = EdgeColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.EdgeColor);
            styleChange = styleChange.Add(addedStyleChange);
        }
        
        if (_graphConfig.PropagatedEdgeColor != PropagatedEdgeColor)
        {
            _graphConfig.PropagatedEdgeColor = PropagatedEdgeColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.PropagatedEdgeColor);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.AlphaEdgeColorPropagated != AlphaEdgeColorPropagated)
        {
            _graphConfig.AlphaEdgeColorPropagated = AlphaEdgeColorPropagated;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.AlphaEdgeColorPropagated);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.AlphaEdgeColorUnPropagated != AlphaEdgeColorUnPropagated)
        {
            _graphConfig.AlphaEdgeColorUnPropagated = AlphaEdgeColorUnPropagated;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.AlphaEdgeColorUnPropagated);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.EdgeThicknessImmersion != EdgeThicknessImmersion)
        {
            _graphConfig.EdgeThicknessImmersion = EdgeThicknessImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.EdgeThicknessImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.EdgeThicknessDesk != EdgeThicknessDesk)
        {
            _graphConfig.EdgeThicknessDesk = EdgeThicknessDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.EdgeThicknessDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.EdgeThicknessLens != EdgeThicknessLens)
        {
            _graphConfig.EdgeThicknessLens = EdgeThicknessLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.EdgeThicknessLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.EdgeThicknessWatch != EdgeThicknessWatch)
        {
            _graphConfig.EdgeThicknessWatch = EdgeThicknessWatch;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.EdgeThicknessWatch);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.CanSelectEdges != CanSelectEdges)
        {
            _graphConfig.CanSelectEdges = CanSelectEdges;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.CanSelectEdges);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.DisplayEdges != DisplayEdges)
        {
            _graphConfig.DisplayEdges = DisplayEdges;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.DisplayEdges);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region LabelNode

        if (_graphConfig.LabelNodeSizeImmersion != LabelNodeSizeImmersion)
        {
            _graphConfig.LabelNodeSizeImmersion = LabelNodeSizeImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.LabelNodeSizeImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.LabelNodeSizeDesk != LabelNodeSizeDesk)
        {
            _graphConfig.LabelNodeSizeDesk = LabelNodeSizeDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.LabelNodeSizeDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.LabelNodeSizeLens != LabelNodeSizeLens)
        {
            _graphConfig.LabelNodeSizeLens = LabelNodeSizeLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.LabelNodeSizeLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.ShowLabelImmersion != ShowLabelImmersion)
        {
            _graphConfig.ShowLabelImmersion = ShowLabelImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.ShowLabelImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.ShowLabelDesk != ShowLabelDesk)
        {
            _graphConfig.ShowLabelDesk = ShowLabelDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.ShowLabelDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.ShowLabelLens != ShowLabelLens)
        {
            _graphConfig.ShowLabelLens = ShowLabelLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.ShowLabelLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        #endregion


        #region Miscelaneous
        if (_graphConfig.LabelNodgePropagation != LabelNodgePropagation)
        {
            _graphConfig.LabelNodgePropagation = LabelNodgePropagation;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.LabelNodgePropagation);
            styleChange = styleChange.Add(addedStyleChange);
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


        if(_graphConfig.DisplayInterSelectedNeighborEdges != DisplayInterSelectedNeighborEdges)
        {
            _graphConfig.DisplayInterSelectedNeighborEdges = DisplayInterSelectedNeighborEdges;


            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.DisplayInterSelectedNeighborEdges);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.ShowWatch != ShowWatch)
        {
            _graphConfig.ShowWatch = ShowWatch;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigurationKey.ShowWatch);
            styleChange = styleChange.Add(addedStyleChange);
        }

        #endregion


        _stylingManager.UpdateStyling(styleChange);
    }
}
