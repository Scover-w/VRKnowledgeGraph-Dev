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

    public float GPSGraphSize = 1f;
    public float LensGraphSize = 1f;

    [Header("Node Size")]
    public float NodeSizeImmersion = 1f;
    public float NodeSizeDesk = 1f;

    [Space(5)]
    public float NodeSizeGPS = 1f;
    public float NodeSizeLens = 1f;

    [Space(5)]
    public float NodeMinMaxSizeImmersion = .8f;
    public float NodeMinMaxSizeDesk = .8f;
    public float NodeMinMaxSizeLens = .8f;


    [Header("Label Size")]
    public float LabelNodeSizeImmersion = 1f;
    public float LabelNodeSizeDesk = 1f;

    public float LabelNodeSizeLens = 1f;
    [Space(5)]
    public bool DisplayLabelImmersion = true;
    public bool DisplayLabelDesk = true;
    public bool DisplayLabelLens = true;


    [Header("Node Color")]
    public Color NodeColor;

    public Color NodeMappingAColor;
    public Color NodeMappingBColor;
    public Color NodeMappingCColor;

    [Range(0f, 1f)]
    public float AlphaNodePropagated = 1f;
    [Range(0f, 1f)]
    public float AlphaNodeUnPropagated = 1f;

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
    public float LuminosityOntologyColor;

    public Color NodeColorNoValueMetric;

    [Header("Edge")]
    public Color EdgeColor;
    public Color PropagatedEdgeColor;

    [Range(0f, 1f)]
    public float AlphaEdgePropagated = 1f;
    [Range(0f, 1f)]
    public float AlphaEdgeUnPropagated = 1f;

    public float EdgeThicknessImmersion = 1f;
    public float EdgeThicknessDesk = 1f;
    public float EdgeThicknessLens = 1f;
    public float EdgeThicknessGPS = 1f;


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

    public bool DisplayGPS = true;


    GraphConfiguration _graphConfig;

    void Start()
    {
        _graphConfig = GraphConfiguration.Instance;


        NodeSizeImmersion = _graphConfig.NodeSizeImmersion;
        NodeSizeDesk = _graphConfig.NodeSizeDesk;

        NodeSizeGPS = _graphConfig.NodeSizeGPS;
        NodeSizeLens = _graphConfig.NodeSizeLens;

        NodeMinMaxSizeImmersion = _graphConfig.NodeMinMaxSizeImmersion;
        NodeMinMaxSizeDesk = _graphConfig.NodeMinMaxSizeDesk;
        NodeMinMaxSizeLens = _graphConfig.NodeMinMaxSizeLens;

        LabelNodeSizeImmersion = _graphConfig.LabelNodeSizeImmersion;
        LabelNodeSizeDesk = _graphConfig.LabelNodeSizeDesk;

        LabelNodeSizeLens = _graphConfig.LabelNodeSizeLens;

        DisplayLabelImmersion = _graphConfig.DisplayLabelImmersion;
        DisplayLabelDesk = _graphConfig.DisplayLabelDesk;

        DisplayLabelLens = _graphConfig.DisplayLabelLens;


        NodeColor = _graphConfig.NodeColor;

        var colorLerpMapper = _graphConfig.NodeColorMapping;
        NodeMappingAColor = colorLerpMapper.ColorA;
        NodeMappingBColor = colorLerpMapper.ColorB;
        NodeMappingCColor = colorLerpMapper.ColorC;

        AlphaNodePropagated = _graphConfig.AlphaNodePropagated;
        AlphaNodeUnPropagated = _graphConfig.AlphaNodeUnPropagated;

        BoundaryColorA = colorLerpMapper.BoundaryColorA;
        BoundaryColorB = colorLerpMapper.BoundaryColorB;
        BoundaryColorC = colorLerpMapper.BoundaryColorC;


        NbOntologyColor = _graphConfig.NbOntologyColor;
        MaxDeltaOntologyAlgo = _graphConfig.MaxDeltaOntologyAlgo;
        SaturationOntologyColor = _graphConfig.SaturationOntologyColor;
        LuminosityOntologyColor = _graphConfig.LuminosityOntologyColor;

        NodeColorNoValueMetric = _graphConfig.NodeColorNoValueMetric;


        SelectedMetricTypeColor = _graphConfig.SelectedMetricTypeColor;
        SelectedMetricTypeSize = _graphConfig.SelectedMetricTypeSize;


        ImmersionGraphSize = _graphConfig.ImmersionGraphSize;
        DeskGraphSize = _graphConfig.DeskGraphSize;

        GPSGraphSize = _graphConfig.GPSGraphSize;
        LensGraphSize = _graphConfig.LensGraphSize;


        EdgeColor = _graphConfig.EdgeColor;
        PropagatedEdgeColor = _graphConfig.PropagatedEdgeColor;

        AlphaEdgePropagated = _graphConfig.AlphaEdgePropagated;
        AlphaEdgeUnPropagated = _graphConfig.AlphaEdgeUnPropagated;

        EdgeThicknessImmersion = _graphConfig.EdgeThicknessImmersion;
        EdgeThicknessDesk = _graphConfig.EdgeThicknessDesk;
        EdgeThicknessLens = _graphConfig.EdgeThicknessLens;
        EdgeThicknessGPS = _graphConfig.EdgeThicknessGPS;

        CanSelectEdges = _graphConfig.CanSelectEdges;
        DisplayEdges = _graphConfig.DisplayEdges;


        LabelNodgePropagation = _graphConfig.LabelNodgePropagation;

        ResetPositionNodeOnUpdate = _graphConfig.ResetPositionNodeOnUpdate;

        SeedRandomPosition = _graphConfig.SeedRandomPosition;
 
        GraphModeTransitionTime = _graphConfig.GraphModeTransitionTime;

        DisplayInterSelectedNeighborEdges = _graphConfig.DisplayInterSelectedNeighborEdges;

        DisplayGPS = _graphConfig.DisplayGPS;
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

        if (_graphConfig.NodeSizeImmersion != NodeSizeImmersion)
        {
            _graphConfig.NodeSizeImmersion = NodeSizeImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeSizeImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.NodeSizeDesk != NodeSizeDesk)
        {
            _graphConfig.NodeSizeDesk = NodeSizeDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeSizeDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.NodeSizeLens != NodeSizeLens)
        {
            _graphConfig.NodeSizeLens = NodeSizeLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeSizeLens);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.NodeSizeGPS != NodeSizeGPS)
        {
            _graphConfig.NodeSizeGPS = NodeSizeGPS;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeSizeGPS);
            styleChange = styleChange.Add(addedStyleChange);
        }



        if (_graphConfig.NodeMinMaxSizeImmersion != NodeMinMaxSizeImmersion)
        {
            _graphConfig.NodeMinMaxSizeImmersion = NodeMinMaxSizeImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeMinMaxSizeImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.NodeMinMaxSizeDesk != NodeMinMaxSizeDesk)
        {
            _graphConfig.NodeMinMaxSizeDesk = NodeMinMaxSizeDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeMinMaxSizeDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.NodeMinMaxSizeLens != NodeMinMaxSizeLens)
        {
            _graphConfig.NodeMinMaxSizeLens = NodeMinMaxSizeLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeMinMaxSizeLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        #endregion


        #region Color
        if (_graphConfig.NodeColor != NodeColor)
        {
            _graphConfig.NodeColor = NodeColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        var colorLerpMapper = _graphConfig.NodeColorMapping;

        if (colorLerpMapper.ColorA != NodeMappingAColor)
        {
            colorLerpMapper.ColorA = NodeMappingAColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (colorLerpMapper.ColorB != NodeMappingBColor)
        {
            colorLerpMapper.ColorB = NodeMappingBColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (colorLerpMapper.ColorC != NodeMappingCColor)
        {
            colorLerpMapper.ColorC = NodeMappingCColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.AlphaNodePropagated != AlphaNodePropagated)
        {
            _graphConfig.AlphaNodePropagated = AlphaNodePropagated;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.AlphaNodePropagated);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.AlphaNodeUnPropagated != AlphaNodeUnPropagated)
        {
            _graphConfig.AlphaNodeUnPropagated = AlphaNodeUnPropagated;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.AlphaNodeUnPropagated);
            styleChange = styleChange.Add(addedStyleChange);
        }



        if (colorLerpMapper.BoundaryColorA != BoundaryColorA)
        {
            colorLerpMapper.BoundaryColorA = BoundaryColorA;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (colorLerpMapper.BoundaryColorB != BoundaryColorB)
        {
            colorLerpMapper.BoundaryColorB = BoundaryColorB;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (colorLerpMapper.BoundaryColorC != BoundaryColorC)
        {
            colorLerpMapper.BoundaryColorC = BoundaryColorC;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeColorMapping);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region Ontology
        if (_graphConfig.NbOntologyColor != NbOntologyColor)
        {
            _graphConfig.NbOntologyColor = NbOntologyColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NbOntologyColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.MaxDeltaOntologyAlgo != MaxDeltaOntologyAlgo)
        {
            _graphConfig.MaxDeltaOntologyAlgo = MaxDeltaOntologyAlgo;
        }

        if (_graphConfig.SaturationOntologyColor != SaturationOntologyColor)
        {
            _graphConfig.SaturationOntologyColor = SaturationOntologyColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.SaturationOntologyColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.LuminosityOntologyColor != LuminosityOntologyColor)
        {
            _graphConfig.LuminosityOntologyColor = LuminosityOntologyColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.LuminosityOntologyColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.NodeColorNoValueMetric != NodeColorNoValueMetric)
        {
            _graphConfig.NodeColorNoValueMetric = NodeColorNoValueMetric;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.NodeColorNoValueMetric);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region Metrics
        if (_graphConfig.SelectedMetricTypeColor != SelectedMetricTypeColor)
        {
            _graphConfig.SelectedMetricTypeColor = SelectedMetricTypeColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.SelectedMetricTypeColor);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.SelectedMetricTypeSize != SelectedMetricTypeSize)
        {
            _graphConfig.SelectedMetricTypeSize = SelectedMetricTypeSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.SelectedMetricTypeSize);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region Graph_Size
        if (_graphConfig.ImmersionGraphSize != ImmersionGraphSize)
        {
            _graphConfig.ImmersionGraphSize = ImmersionGraphSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.ImmersionGraphSize);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.GPSGraphSize != GPSGraphSize)
        {
            _graphConfig.GPSGraphSize = GPSGraphSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.GPSGraphSize);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.DeskGraphSize != DeskGraphSize)
        {
            _graphConfig.DeskGraphSize = DeskGraphSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.DeskGraphSize);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.LensGraphSize != LensGraphSize)
        {
            _graphConfig.LensGraphSize = LensGraphSize;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.LensGraphSize);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region Edge
        if (_graphConfig.EdgeColor != EdgeColor)
        {
            _graphConfig.EdgeColor = EdgeColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.EdgeColor);
            styleChange = styleChange.Add(addedStyleChange);
        }
        
        if (_graphConfig.PropagatedEdgeColor != PropagatedEdgeColor)
        {
            _graphConfig.PropagatedEdgeColor = PropagatedEdgeColor;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.PropagatedEdgeColor);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.AlphaEdgePropagated != AlphaEdgePropagated)
        {
            _graphConfig.AlphaEdgePropagated = AlphaEdgePropagated;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.AlphaEdgePropagated);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.AlphaEdgeUnPropagated != AlphaEdgeUnPropagated)
        {
            _graphConfig.AlphaEdgeUnPropagated = AlphaEdgeUnPropagated;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.AlphaEdgeUnPropagated);
            styleChange = styleChange.Add(addedStyleChange);
        }


        if (_graphConfig.EdgeThicknessImmersion != EdgeThicknessImmersion)
        {
            _graphConfig.EdgeThicknessImmersion = EdgeThicknessImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.EdgeThicknessImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.EdgeThicknessDesk != EdgeThicknessDesk)
        {
            _graphConfig.EdgeThicknessDesk = EdgeThicknessDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.EdgeThicknessDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.EdgeThicknessLens != EdgeThicknessLens)
        {
            _graphConfig.EdgeThicknessLens = EdgeThicknessLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.EdgeThicknessLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.EdgeThicknessGPS != EdgeThicknessGPS)
        {
            _graphConfig.EdgeThicknessGPS = EdgeThicknessGPS;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.EdgeThicknessGPS);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.CanSelectEdges != CanSelectEdges)
        {
            _graphConfig.CanSelectEdges = CanSelectEdges;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.CanSelectEdges);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.DisplayEdges != DisplayEdges)
        {
            _graphConfig.DisplayEdges = DisplayEdges;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.DisplayEdges);
            styleChange = styleChange.Add(addedStyleChange);
        }
        #endregion


        #region LabelNode

        if (_graphConfig.LabelNodeSizeImmersion != LabelNodeSizeImmersion)
        {
            _graphConfig.LabelNodeSizeImmersion = LabelNodeSizeImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.LabelNodeSizeImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.LabelNodeSizeDesk != LabelNodeSizeDesk)
        {
            _graphConfig.LabelNodeSizeDesk = LabelNodeSizeDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.LabelNodeSizeDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.LabelNodeSizeLens != LabelNodeSizeLens)
        {
            _graphConfig.LabelNodeSizeLens = LabelNodeSizeLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.LabelNodeSizeLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.DisplayLabelImmersion != DisplayLabelImmersion)
        {
            _graphConfig.DisplayLabelImmersion = DisplayLabelImmersion;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.DisplayLabelImmersion);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.DisplayLabelDesk != DisplayLabelDesk)
        {
            _graphConfig.DisplayLabelDesk = DisplayLabelDesk;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.DisplayLabelDesk);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.DisplayLabelLens != DisplayLabelLens)
        {
            _graphConfig.DisplayLabelLens = DisplayLabelLens;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.DisplayLabelLens);
            styleChange = styleChange.Add(addedStyleChange);
        }

        #endregion


        #region Miscelaneous
        if (_graphConfig.LabelNodgePropagation != LabelNodgePropagation)
        {
            _graphConfig.LabelNodgePropagation = LabelNodgePropagation;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.LabelNodgePropagation);
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


            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.DisplayInterSelectedNeighborEdges);
            styleChange = styleChange.Add(addedStyleChange);
        }

        if (_graphConfig.DisplayGPS != DisplayGPS)
        {
            _graphConfig.DisplayGPS = DisplayGPS;

            addedStyleChange = StyleChangeBuilder.Get(GraphConfigKey.DisplayGPS);
            styleChange = styleChange.Add(addedStyleChange);
        }

        #endregion


        _stylingManager.UpdateStyling(styleChange);
    }
}
