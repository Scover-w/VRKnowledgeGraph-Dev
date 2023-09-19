using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleChangeBuilder
{

    static Dictionary<GraphConfigKey, StyleChange> _styleChanges = CreateDic();

    public static StyleChange Get(GraphConfigKey key)
    {
     
        if(_styleChanges.TryGetValue(key, out StyleChange styleChange))
        {
            return styleChange;
        }

        return StyleChange.None;
    }



    private static Dictionary<GraphConfigKey, StyleChange> CreateDic()
    {
        Dictionary<GraphConfigKey, StyleChange> styleChanges = new();

        #region NodeSize


        StyleChange styleChange = StyleChange.MainGraph
                                .Add(StyleChange.ImmersionMode)
                                .Add(StyleChange.Node)
                                .Add(StyleChange.Label)
                                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.NodeSizeImmersion, styleChange);



        styleChange = StyleChange.MainGraph
                    .Add(StyleChange.DeskMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.NodeSizeDesk, styleChange);



        styleChange = StyleChange.SubGraph
                    .Add(StyleChange.DeskMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.NodeSizeLens, styleChange);


        styleChange = StyleChange.SubGraph
                    .Add(StyleChange.ImmersionMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.NodeSizeGPS, styleChange);

        styleChange = StyleChange.MainGraph
                    .Add(StyleChange.ImmersionMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.NodeMinMaxSizeImmersion, styleChange);


        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);
        styleChanges.Add(GraphConfigKey.NodeMinMaxSizeDesk, styleChange);


        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.NodeMinMaxSizeLens, styleChange);

        #endregion


        #region Color
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigKey.NodeColor, styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigKey.NodeColorMapping, styleChange);
        styleChanges.Add(GraphConfigKey.NodeColorMappingColorA, styleChange);
        styleChanges.Add(GraphConfigKey.NodeColorMappingColorB, styleChange);
        styleChanges.Add(GraphConfigKey.NodeColorMappingColorC, styleChange);
        styleChanges.Add(GraphConfigKey.NodeColorMappingBoundaryColorA, styleChange);
        styleChanges.Add(GraphConfigKey.NodeColorMappingBoundaryColorB, styleChange);
        styleChanges.Add(GraphConfigKey.NodeColorMappingBoundaryColorC, styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigKey.AlphaNodePropagated, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigKey.AlphaNodeUnPropagated, styleChange);

        #endregion


        #region Ontology
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigKey.NbOntologyColor, styleChange);



        // styleChanges.Add(GraphConfigurationKey.MaxDeltaOntologyAlgo, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigKey.SaturationOntologyColor, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigKey.LuminosityOntologyColor, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigKey.NodeColorNoValueMetric, styleChange);
        #endregion


        #region Metrics
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigKey.SelectedMetricTypeColor, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.SelectedMetricTypeSize, styleChange);
        #endregion


        #region Graph_Size
        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.ImmersionGraphSize, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.GPSGraphSize, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.DeskGraphSize, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.LensGraphSize, styleChange);
        #endregion


        #region Edge
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigKey.EdgeColor, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigKey.PropagatedEdgeColor, styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigKey.AlphaEdgePropagated, styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigKey.AlphaEdgeUnPropagated, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.EdgeThicknessImmersion, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.EdgeThicknessDesk, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.EdgeThicknessLens, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.EdgeThicknessGPS, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color)
                .Add(StyleChange.Collider);

        styleChanges.Add(GraphConfigKey.CanSelectEdges, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigKey.DisplayEdges, styleChange);
        #endregion


        #region LabelNode

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.LabelNodeSizeImmersion, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.LabelNodeSizeDesk, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigKey.LabelNodeSizeLens, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigKey.DisplayLabelImmersion, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigKey.DisplayLabelDesk, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigKey.DisplayLabelLens, styleChange);

        #endregion


        #region Simulation
        // TODO : todo if usefull ?
        #endregion

        #region Miscelaneous
        styleChange = StyleChange.Propagation;

        styleChanges.Add(GraphConfigKey.LabelNodgePropagation, styleChange);


        // styleChanges.Add(GraphConfigurationKey.ResetPositionNodeOnUpdate, styleChange);

        // styleChanges.Add(GraphConfigurationKey.SeedRandomPosition, styleChange);

        // styleChanges.Add(GraphConfigurationKey.GraphModeTransitionTime, styleChange);


        styleChange = StyleChange.Selection
                      .Add(StyleChange.DeskMode)
                      .Add(StyleChange.SubGraph)
                      .Add(StyleChange.Selection);

        styleChanges.Add(GraphConfigKey.DisplayInterSelectedNeighborEdges, styleChange);


        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigKey.DisplayGPS, styleChange);

        #endregion



        return styleChanges;
    }
}
