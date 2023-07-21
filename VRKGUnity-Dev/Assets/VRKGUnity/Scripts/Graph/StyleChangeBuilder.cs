using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleChangeBuilder
{

    static Dictionary<string, StyleChange> _styleChanges = CreateDic();

    public static StyleChange Build(string test)
    {
     
        if(_styleChanges.TryGetValue(test, out StyleChange styleChange))
        {
            return styleChange;
        }

        return StyleChange.None;
    }



    private static Dictionary<string, StyleChange> CreateDic()
    {
        Dictionary<string, StyleChange> styleChanges = new();

        #region NodeSize


        StyleChange styleChange = StyleChange.MainGraph
                                .Add(StyleChange.ImmersionMode)
                                .Add(StyleChange.Node)
                                .Add(StyleChange.Label)
                                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeSizeImmersion), styleChange);



        styleChange = StyleChange.MainGraph
                    .Add(StyleChange.DeskMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeSizeDesk), styleChange);



        styleChange = StyleChange.SubGraph
                    .Add(StyleChange.DeskMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeSizeLens), styleChange);


        styleChange = StyleChange.SubGraph
                    .Add(StyleChange.ImmersionMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeSizeWatch), styleChange);

        styleChange = StyleChange.MainGraph
                    .Add(StyleChange.ImmersionMode)
                    .Add(StyleChange.Node)
                    .Add(StyleChange.Label)
                    .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeMinSizeImmersion), styleChange);

        styleChange = StyleChange.MainGraph
                        .Add(StyleChange.ImmersionMode)
                        .Add(StyleChange.Node)
                        .Add(StyleChange.Label)
                        .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeMaxSizeImmersion), styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);
        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeMinSizeDesk), styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeMaxSizeDesk), styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeMinSizeLens), styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);
        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeMaxSizeLens), styleChange);

        #endregion


        #region Color
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeColor), styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeColorMapping), styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(nameof(GraphConfiguration.Instance.AlphaNodeColorPropagated), styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(nameof(GraphConfiguration.Instance.AlphaNodeColorUnPropagated), styleChange);

        #endregion


        #region Ontology
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);
        styleChanges.Add(nameof(GraphConfiguration.Instance.NbOntologyColor), styleChange);



        // styleChanges.Add(nameof(GraphConfiguration.Instance.MaxDeltaOntologyAlgo), styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(nameof(GraphConfiguration.Instance.SaturationOntologyColor), styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(nameof(GraphConfiguration.Instance.ValueOntologyColor), styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(nameof(GraphConfiguration.Instance.NodeColorNoValueMetric), styleChange);
        #endregion


        #region Metrics
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Color);

        styleChanges.Add(nameof(GraphConfiguration.Instance.SelectedMetricTypeColor), styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Node)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.SelectedMetricTypeSize), styleChange);
        #endregion


        #region Graph_Size
        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position);

        styleChanges.Add(nameof(GraphConfiguration.Instance.ImmersionGraphSize), styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position);

        styleChanges.Add(nameof(GraphConfiguration.Instance.WatchGraphSize), styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position);

        styleChanges.Add(nameof(GraphConfiguration.Instance.DeskGraphSize), styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Nodge)
                .Add(StyleChange.Position);

        styleChanges.Add(nameof(GraphConfiguration.Instance.LensGraphSize), styleChange);
        #endregion


        #region Edge
        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(nameof(GraphConfiguration.Instance.EdgeColor), styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(nameof(GraphConfiguration.Instance.PropagatedEdgeColor), styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(nameof(GraphConfiguration.Instance.AlphaEdgeColorPropagated), styleChange);


        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color);

        styleChanges.Add(nameof(GraphConfiguration.Instance.AlphaEdgeColorUnPropagated), styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.EdgeThicknessImmersion), styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.EdgeThicknessDesk), styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.EdgeThicknessLens), styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.EdgeThicknessWatch), styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Color)
                .Add(StyleChange.Collider);

        styleChanges.Add(nameof(GraphConfiguration.Instance.CanSelectEdges), styleChange);

        styleChange = StyleChange.BothGraph
                .Add(StyleChange.BothMode)
                .Add(StyleChange.Edge)
                .Add(StyleChange.Visibility);

        styleChanges.Add(nameof(GraphConfiguration.Instance.DisplayEdges), styleChange);
        #endregion


        #region LabelNode

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.LabelNodeSizeImmersion), styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.LabelNodeSizeDesk), styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Size);

        styleChanges.Add(nameof(GraphConfiguration.Instance.LabelNodeSizeLens), styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(nameof(GraphConfiguration.Instance.ShowLabelImmersion), styleChange);

        styleChange = StyleChange.MainGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(nameof(GraphConfiguration.Instance.ShowLabelDesk), styleChange);

        styleChange = StyleChange.SubGraph
                .Add(StyleChange.DeskMode)
                .Add(StyleChange.Label)
                .Add(StyleChange.Visibility);

        styleChanges.Add(nameof(GraphConfiguration.Instance.ShowLabelLens), styleChange);

        #endregion


        #region Miscelaneous
        styleChange = StyleChange.Propagation;

        styleChanges.Add(nameof(GraphConfiguration.Instance.LabelNodgePropagation), styleChange);


        // styleChanges.Add(nameof(GraphConfiguration.Instance.ResetPositionNodeOnUpdate), styleChange);

        // styleChanges.Add(nameof(GraphConfiguration.Instance.SeedRandomPosition), styleChange);

        // styleChanges.Add(nameof(GraphConfiguration.Instance.GraphModeTransitionTime), styleChange);


        styleChange = StyleChange.Selection;

        styleChanges.Add(nameof(GraphConfiguration.Instance.DisplayInterSelectedNeighborEdges), styleChange);


        styleChange = StyleChange.SubGraph
                .Add(StyleChange.ImmersionMode)
                .Add(StyleChange.Visibility);

        styleChanges.Add(nameof(GraphConfiguration.Instance.ShowWatch), styleChange);

        #endregion



        return styleChanges;
    }
}
