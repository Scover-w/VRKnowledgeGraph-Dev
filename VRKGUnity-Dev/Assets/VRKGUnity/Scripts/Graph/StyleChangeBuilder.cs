using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleChangeBuilder
{

    static Dictionary<GraphConfigurationKey, StyleChange> _styleChanges = CreateDic();

    public static StyleChange Get(GraphConfigurationKey key)
    {
     
        if(_styleChanges.TryGetValue(key, out StyleChange styleChange))
        {
            return styleChange;
        }

        return StyleChange.None;
    }



    private static Dictionary<GraphConfigurationKey, StyleChange> CreateDic()
    {
        Dictionary<GraphConfigurationKey, StyleChange> styleChanges = new();

        #region NodeSize


        StyleChange styleChange = StyleChange.MainGraph
                                .Add(StyleChange.ImmersionMode)
                                .Add(StyleChange.Node)
                                .Add(StyleChange.Label)
                                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.NodeSizeImmersion, styleChange);



        styleChange = StyleChange.MainGraph
                    .Add(StyleChange.DeskMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.NodeSizeDesk, styleChange);



        styleChange = StyleChange.SubGraph
                    .Add(StyleChange.DeskMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.NodeSizeLens, styleChange);


        styleChange = StyleChange.SubGraph
                    .Add(StyleChange.ImmersionMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.NodeSizeWatch, styleChange);

        styleChange = StyleChange.MainGraph
                    .Add(StyleChange.ImmersionMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.NodeMinSizeImmersion, styleChange);

        styleChange = StyleChange.MainGraph
                        .Add(StyleChange.ImmersionMode)
                        .Add(StyleChange.Node)
                        .Add(StyleChange.Label)
                        .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.NodeMaxSizeImmersion, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);
        styleChanges.Add(GraphConfigurationKey.NodeMinSizeDesk, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.NodeMaxSizeDesk, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.NodeMinSizeLens, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);
        styleChanges.Add(GraphConfigurationKey.NodeMaxSizeLens, styleChange);

        #endregion


        #region Color
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigurationKey.NodeColor, styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigurationKey.NodeColorMapping, styleChange);
        styleChanges.Add(GraphConfigurationKey.NodeColorMappingColorA, styleChange);
        styleChanges.Add(GraphConfigurationKey.NodeColorMappingColorB, styleChange);
        styleChanges.Add(GraphConfigurationKey.NodeColorMappingColorC, styleChange);
        styleChanges.Add(GraphConfigurationKey.NodeColorMappingBoundaryColorA, styleChange);
        styleChanges.Add(GraphConfigurationKey.NodeColorMappingBoundaryColorB, styleChange);
        styleChanges.Add(GraphConfigurationKey.NodeColorMappingBoundaryColorC, styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigurationKey.AlphaNodeColorPropagated, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigurationKey.AlphaNodeColorUnPropagated, styleChange);

        #endregion


        #region Ontology
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(GraphConfigurationKey.NbOntologyColor, styleChange);



        // styleChanges.Add(GraphConfigurationKey.MaxDeltaOntologyAlgo, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigurationKey.SaturationOntologyColor, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigurationKey.ValueOntologyColor, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigurationKey.NodeColorNoValueMetric, styleChange);
        #endregion


        #region Metrics
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigurationKey.SelectedMetricTypeColor, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.SelectedMetricTypeSize, styleChange);
        #endregion


        #region Graph_Size
        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position);

        styleChanges.Add(GraphConfigurationKey.ImmersionGraphSize, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position);

        styleChanges.Add(GraphConfigurationKey.WatchGraphSize, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position);

        styleChanges.Add(GraphConfigurationKey.DeskGraphSize, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position);

        styleChanges.Add(GraphConfigurationKey.LensGraphSize, styleChange);
        #endregion


        #region Edge
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigurationKey.EdgeColor, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigurationKey.PropagatedEdgeColor, styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigurationKey.AlphaEdgeColorPropagated, styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(GraphConfigurationKey.AlphaEdgeColorUnPropagated, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.EdgeThicknessImmersion, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.EdgeThicknessDesk, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.EdgeThicknessLens, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.EdgeThicknessWatch, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color)
                .Add(StyleChange.Collider);

        styleChanges.Add(GraphConfigurationKey.CanSelectEdges, styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigurationKey.DisplayEdges, styleChange);
        #endregion


        #region LabelNode

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.LabelNodeSizeImmersion, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.LabelNodeSizeDesk, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(GraphConfigurationKey.LabelNodeSizeLens, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigurationKey.ShowLabelImmersion, styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigurationKey.ShowLabelDesk, styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigurationKey.ShowLabelLens, styleChange);

        #endregion


        #region Simulation
        // TODO : todo if usefull ?
        #endregion

        #region Miscelaneous
        styleChange = StyleChange.Propagation;

        styleChanges.Add(GraphConfigurationKey.LabelNodgePropagation, styleChange);


        // styleChanges.Add(GraphConfigurationKey.ResetPositionNodeOnUpdate, styleChange);

        // styleChanges.Add(GraphConfigurationKey.SeedRandomPosition, styleChange);

        // styleChanges.Add(GraphConfigurationKey.GraphModeTransitionTime, styleChange);


        styleChange = StyleChange.Selection
                      .Add(StyleChange.DeskMode)
                      .Add(StyleChange.SubGraph)
                      .Add(StyleChange.Selection);

        styleChanges.Add(GraphConfigurationKey.DisplayInterSelectedNeighborEdges, styleChange);


        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Visibility);

        styleChanges.Add(GraphConfigurationKey.ShowWatch, styleChange);

        #endregion



        return styleChanges;
    }
}
