using System;
using UnityEngine;

public static class GraphConfigurationTools
{

    public static bool IsGoodType(this GraphConfigKey key, Type askedType)
    {
        Type realType = key.GetRealType();

        if (askedType == realType)
            return true;

        if (askedType == typeof(float) && realType == typeof(int))
            return true;

        if (askedType == typeof(string) && realType == typeof(GraphMetricType))
            return true;

        return false;
    }


    public static GraphConfigValueType GetConfigValueType(this GraphConfigKey key)
    {
        return key switch
        {
            GraphConfigKey.SimuParameters => GraphConfigValueType.Float,
            GraphConfigKey.LensSimuParameters => GraphConfigValueType.Float,
            GraphConfigKey.ImmersionGraphSize => GraphConfigValueType.Float,
            GraphConfigKey.DeskGraphSize => GraphConfigValueType.Float,
            GraphConfigKey.WatchGraphSize => GraphConfigValueType.Float,
            GraphConfigKey.LensGraphSize => GraphConfigValueType.Float,
            GraphConfigKey.NodeSizeImmersion => GraphConfigValueType.Float,
            GraphConfigKey.NodeSizeDesk => GraphConfigValueType.Float,
            GraphConfigKey.NodeSizeWatch => GraphConfigValueType.Float,
            GraphConfigKey.NodeSizeLens => GraphConfigValueType.Float,
            GraphConfigKey.NodeMinSizeImmersion => GraphConfigValueType.Float,
            GraphConfigKey.NodeMaxSizeImmersion => GraphConfigValueType.Float,
            GraphConfigKey.NodeMinSizeDesk => GraphConfigValueType.Float,
            GraphConfigKey.NodeMaxSizeDesk => GraphConfigValueType.Float,
            GraphConfigKey.NodeMinSizeLens => GraphConfigValueType.Float,
            GraphConfigKey.NodeMaxSizeLens => GraphConfigValueType.Float,
            GraphConfigKey.LabelNodeSizeImmersion => GraphConfigValueType.Float,
            GraphConfigKey.LabelNodeSizeDesk => GraphConfigValueType.Float,
            GraphConfigKey.LabelNodeSizeLens => GraphConfigValueType.Float,
            GraphConfigKey.ShowLabelImmersion => GraphConfigValueType.Bool,
            GraphConfigKey.ShowLabelDesk => GraphConfigValueType.Bool,
            GraphConfigKey.ShowLabelLens => GraphConfigValueType.Bool,
            GraphConfigKey.EdgeThicknessImmersion => GraphConfigValueType.Float,
            GraphConfigKey.EdgeThicknessDesk => GraphConfigValueType.Float,
            GraphConfigKey.EdgeThicknessLens => GraphConfigValueType.Float,
            GraphConfigKey.EdgeThicknessWatch => GraphConfigValueType.Float,
            GraphConfigKey.CanSelectEdges => GraphConfigValueType.Bool,
            GraphConfigKey.DisplayEdges => GraphConfigValueType.Bool,
            GraphConfigKey.NodeColor => GraphConfigValueType.Color,
            GraphConfigKey.NodeColorNoValueMetric => GraphConfigValueType.Color,
            GraphConfigKey.EdgeColor => GraphConfigValueType.Color,
            GraphConfigKey.PropagatedEdgeColor => GraphConfigValueType.Color,
            GraphConfigKey.NodeColorMappingColorA => GraphConfigValueType.Color,
            GraphConfigKey.NodeColorMappingColorB => GraphConfigValueType.Color,
            GraphConfigKey.NodeColorMappingColorC => GraphConfigValueType.Color,
            GraphConfigKey.NodeColorMappingBoundaryColorA => GraphConfigValueType.Float,
            GraphConfigKey.NodeColorMappingBoundaryColorB => GraphConfigValueType.Float,
            GraphConfigKey.NodeColorMappingBoundaryColorC => GraphConfigValueType.Float,
            GraphConfigKey.AlphaNodeColorPropagated => GraphConfigValueType.Float,
            GraphConfigKey.AlphaNodeColorUnPropagated => GraphConfigValueType.Float,
            GraphConfigKey.AlphaEdgeColorPropagated => GraphConfigValueType.Float,
            GraphConfigKey.AlphaEdgeColorUnPropagated => GraphConfigValueType.Float,
            GraphConfigKey.NbOntologyColor => GraphConfigValueType.Float,
            GraphConfigKey.MaxDeltaOntologyAlgo => GraphConfigValueType.Float,
            GraphConfigKey.SaturationOntologyColor => GraphConfigValueType.Float,
            GraphConfigKey.ValueOntologyColor => GraphConfigValueType.Float,
            GraphConfigKey.LabelNodgePropagation => GraphConfigValueType.Float,
            GraphConfigKey.ResetPositionNodeOnUpdate => GraphConfigValueType.Bool,
            GraphConfigKey.SeedRandomPosition => GraphConfigValueType.Float,
            GraphConfigKey.GraphModeTransitionTime => GraphConfigValueType.Float,
            GraphConfigKey.DisplayInterSelectedNeighborEdges => GraphConfigValueType.Bool,
            GraphConfigKey.ShowWatch => GraphConfigValueType.Bool,
            GraphConfigKey.SelectedMetricTypeSize => GraphConfigValueType.String,
            GraphConfigKey.SelectedMetricTypeColor => GraphConfigValueType.String,
            GraphConfigKey.DefaultSimulationParameters => GraphConfigValueType.Float,
            GraphConfigKey.DefaultTickDeltaTime => GraphConfigValueType.Float,
            GraphConfigKey.DefaultMaxSimulationTime => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLerpSmooth => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLightSpringForce => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLightCoulombForce => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLightDamping => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLightSpringDistance => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLightCoulombDistance => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLightMaxVelocity => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLightStopVelocity => GraphConfigValueType.Float,
            GraphConfigKey.DefaultDenseSpringForce => GraphConfigValueType.Float,
            GraphConfigKey.DefaultDenseCoulombForce => GraphConfigValueType.Float,
            GraphConfigKey.DefaultDenseDamping => GraphConfigValueType.Float,
            GraphConfigKey.DefaultDenseSpringDistance => GraphConfigValueType.Float,
            GraphConfigKey.DefaultDenseCoulombDistance => GraphConfigValueType.Float,
            GraphConfigKey.DefaultDenseMaxVelocity => GraphConfigValueType.Float,
            GraphConfigKey.DefaultDenseStopVelocity => GraphConfigValueType.Float,
            GraphConfigKey.LensSimulationParameters => GraphConfigValueType.Float,
            GraphConfigKey.LensTickDeltaTime => GraphConfigValueType.Float,
            GraphConfigKey.LensMaxSimulationTime => GraphConfigValueType.Float,
            GraphConfigKey.LensLerpSmooth => GraphConfigValueType.Float,
            GraphConfigKey.LensLightSpringForce => GraphConfigValueType.Float,
            GraphConfigKey.LensLightCoulombForce => GraphConfigValueType.Float,
            GraphConfigKey.LensLightDamping => GraphConfigValueType.Float,
            GraphConfigKey.LensLightSpringDistance => GraphConfigValueType.Float,
            GraphConfigKey.LensLightCoulombDistance => GraphConfigValueType.Float,
            GraphConfigKey.LensLightMaxVelocity => GraphConfigValueType.Float,
            GraphConfigKey.LensLightStopVelocity => GraphConfigValueType.Float,
            GraphConfigKey.LensDenseSpringForce => GraphConfigValueType.Float,
            GraphConfigKey.LensDenseCoulombForce => GraphConfigValueType.Float,
            GraphConfigKey.LensDenseDamping => GraphConfigValueType.Float,
            GraphConfigKey.LensDenseSpringDistance => GraphConfigValueType.Float,
            GraphConfigKey.LensDenseCoulombDistance => GraphConfigValueType.Float,
            GraphConfigKey.LensDenseMaxVelocity => GraphConfigValueType.Float,
            GraphConfigKey.LensDenseStopVelocity => GraphConfigValueType.Float,
            _ => GraphConfigValueType.Float,
        };
    }

    private static Type GetRealType(this GraphConfigKey key)
    {
        return key switch
        {
            GraphConfigKey.GraphMode => typeof(bool),
            GraphConfigKey.SelectionMode => typeof(string),
            GraphConfigKey.SimuParameters => typeof(float),
            GraphConfigKey.LensSimuParameters => typeof(float),
            GraphConfigKey.ImmersionGraphSize => typeof(float),
            GraphConfigKey.DeskGraphSize => typeof(float),
            GraphConfigKey.WatchGraphSize => typeof(float),
            GraphConfigKey.LensGraphSize => typeof(float),
            GraphConfigKey.NodeSizeImmersion => typeof(float),
            GraphConfigKey.NodeSizeDesk => typeof(float),
            GraphConfigKey.NodeSizeWatch => typeof(float),
            GraphConfigKey.NodeSizeLens => typeof(float),
            GraphConfigKey.NodeMinSizeImmersion => typeof(float),
            GraphConfigKey.NodeMaxSizeImmersion => typeof(float),
            GraphConfigKey.NodeMinSizeDesk => typeof(float),
            GraphConfigKey.NodeMaxSizeDesk => typeof(float),
            GraphConfigKey.NodeMinSizeLens => typeof(float),
            GraphConfigKey.NodeMaxSizeLens => typeof(float),
            GraphConfigKey.LabelNodeSizeImmersion => typeof(float),
            GraphConfigKey.LabelNodeSizeDesk => typeof(float),
            GraphConfigKey.LabelNodeSizeLens => typeof(float),
            GraphConfigKey.ShowLabelImmersion => typeof(bool),
            GraphConfigKey.ShowLabelDesk => typeof(bool),
            GraphConfigKey.ShowLabelLens => typeof(bool),
            GraphConfigKey.EdgeThicknessImmersion => typeof(float),
            GraphConfigKey.EdgeThicknessDesk => typeof(float),
            GraphConfigKey.EdgeThicknessLens => typeof(float),
            GraphConfigKey.EdgeThicknessWatch => typeof(float),
            GraphConfigKey.CanSelectEdges => typeof(bool),
            GraphConfigKey.DisplayEdges => typeof(bool),
            GraphConfigKey.NodeColor => typeof(Color),
            GraphConfigKey.NodeColorNoValueMetric => typeof(Color),
            GraphConfigKey.EdgeColor => typeof(Color),
            GraphConfigKey.PropagatedEdgeColor => typeof(Color),
            GraphConfigKey.NodeColorMappingColorA => typeof(Color),
            GraphConfigKey.NodeColorMappingColorB => typeof(Color),
            GraphConfigKey.NodeColorMappingColorC => typeof(Color),
            GraphConfigKey.NodeColorMappingBoundaryColorA => typeof(float),
            GraphConfigKey.NodeColorMappingBoundaryColorB => typeof(float),
            GraphConfigKey.NodeColorMappingBoundaryColorC => typeof(float),
            GraphConfigKey.AlphaNodeColorPropagated => typeof(float),
            GraphConfigKey.AlphaNodeColorUnPropagated => typeof(float),
            GraphConfigKey.AlphaEdgeColorPropagated => typeof(float),
            GraphConfigKey.AlphaEdgeColorUnPropagated => typeof(float),
            GraphConfigKey.NbOntologyColor => typeof(int),
            GraphConfigKey.MaxDeltaOntologyAlgo => typeof(int),
            GraphConfigKey.SaturationOntologyColor => typeof(float),
            GraphConfigKey.ValueOntologyColor => typeof(float),
            GraphConfigKey.LabelNodgePropagation => typeof(int),
            GraphConfigKey.ResetPositionNodeOnUpdate => typeof(bool),
            GraphConfigKey.SeedRandomPosition => typeof(int),
            GraphConfigKey.GraphModeTransitionTime => typeof(float),
            GraphConfigKey.DisplayInterSelectedNeighborEdges => typeof(bool),
            GraphConfigKey.ShowWatch => typeof(bool),
            GraphConfigKey.SelectedMetricTypeSize => typeof(GraphMetricType),
            GraphConfigKey.SelectedMetricTypeColor => typeof(GraphMetricType),
            GraphConfigKey.DefaultSimulationParameters => typeof(float),
            GraphConfigKey.DefaultTickDeltaTime => typeof(float),
            GraphConfigKey.DefaultMaxSimulationTime => typeof(float),
            GraphConfigKey.DefaultLerpSmooth => typeof(float),
            GraphConfigKey.DefaultLightSpringForce => typeof(float),
            GraphConfigKey.DefaultLightCoulombForce => typeof(float),
            GraphConfigKey.DefaultLightDamping => typeof(float),
            GraphConfigKey.DefaultLightSpringDistance => typeof(float),
            GraphConfigKey.DefaultLightCoulombDistance => typeof(float),
            GraphConfigKey.DefaultLightMaxVelocity => typeof(float),
            GraphConfigKey.DefaultLightStopVelocity => typeof(float),
            GraphConfigKey.DefaultDenseSpringForce => typeof(float),
            GraphConfigKey.DefaultDenseCoulombForce => typeof(float),
            GraphConfigKey.DefaultDenseDamping => typeof(float),
            GraphConfigKey.DefaultDenseSpringDistance => typeof(float),
            GraphConfigKey.DefaultDenseCoulombDistance => typeof(float),
            GraphConfigKey.DefaultDenseMaxVelocity => typeof(float),
            GraphConfigKey.DefaultDenseStopVelocity => typeof(float),
            GraphConfigKey.LensSimulationParameters => typeof(float),
            GraphConfigKey.LensTickDeltaTime => typeof(float),
            GraphConfigKey.LensMaxSimulationTime => typeof(float),
            GraphConfigKey.LensLerpSmooth => typeof(float),
            GraphConfigKey.LensLightSpringForce => typeof(float),
            GraphConfigKey.LensLightCoulombForce => typeof(float),
            GraphConfigKey.LensLightDamping => typeof(float),
            GraphConfigKey.LensLightSpringDistance => typeof(float),
            GraphConfigKey.LensLightCoulombDistance => typeof(float),
            GraphConfigKey.LensLightMaxVelocity => typeof(float),
            GraphConfigKey.LensLightStopVelocity => typeof(float),
            GraphConfigKey.LensDenseSpringForce => typeof(float),
            GraphConfigKey.LensDenseCoulombForce => typeof(float),
            GraphConfigKey.LensDenseDamping => typeof(float),
            GraphConfigKey.LensDenseSpringDistance => typeof(float),
            GraphConfigKey.LensDenseCoulombDistance => typeof(float),
            GraphConfigKey.LensDenseMaxVelocity => typeof(float),
            GraphConfigKey.LensDenseStopVelocity => typeof(float),
            _ => typeof(float),
        };
    }

    public static T GetValue<T>(this GraphConfigKey key, GraphConfiguration graphConfig)
    {

        if (typeof(T) == typeof(string))
        {
            return (T)(object)GetStringValue(key, graphConfig);
        }
        else if (typeof(T) == typeof(float))
        {
            return (T)(object)GetFloatValue(key, graphConfig);
        }
        else if (typeof(T) == typeof(bool))
        {
            return (T)(object)GetBoolValue(key, graphConfig);
        }
        else if (typeof(T) == typeof(Color))
        {
            return (T)(object)GetColorValue(key, graphConfig);
        }

        Debug.LogError("No T as " + typeof(T) + " is handled");

        return default(T);
    }

    private static string GetStringValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        return key switch
        {
            GraphConfigKey.SelectionMode => graphConfig.SelectionMode.ToString(),
            GraphConfigKey.SelectedMetricTypeSize => graphConfig.SelectedMetricTypeSize.ToString(),
            GraphConfigKey.SelectedMetricTypeColor => graphConfig.SelectedMetricTypeColor.ToString(),
            _ => "",
        };
    }

    private static float GetFloatValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        return key switch
        {
            GraphConfigKey.ImmersionGraphSize => graphConfig.ImmersionGraphSize,
            GraphConfigKey.DeskGraphSize => graphConfig.DeskGraphSize,
            GraphConfigKey.WatchGraphSize => graphConfig.WatchGraphSize,
            GraphConfigKey.LensGraphSize => graphConfig.LensGraphSize,
            GraphConfigKey.NodeSizeImmersion => graphConfig.NodeSizeImmersion,
            GraphConfigKey.NodeSizeDesk => graphConfig.NodeSizeDesk,
            GraphConfigKey.NodeSizeWatch => graphConfig.NodeSizeWatch,
            GraphConfigKey.NodeSizeLens => graphConfig.NodeSizeLens,
            GraphConfigKey.NodeMinSizeImmersion => graphConfig.NodeMinSizeImmersion,
            GraphConfigKey.NodeMaxSizeImmersion => graphConfig.NodeMaxSizeImmersion,
            GraphConfigKey.NodeMinSizeDesk => graphConfig.NodeMinSizeDesk,
            GraphConfigKey.NodeMaxSizeDesk => graphConfig.NodeMaxSizeDesk,
            GraphConfigKey.NodeMinSizeLens => graphConfig.NodeMinSizeLens,
            GraphConfigKey.NodeMaxSizeLens => graphConfig.NodeMaxSizeLens,
            GraphConfigKey.LabelNodeSizeImmersion => graphConfig.LabelNodeSizeImmersion,
            GraphConfigKey.LabelNodeSizeDesk => graphConfig.LabelNodeSizeDesk,
            GraphConfigKey.LabelNodeSizeLens => graphConfig.LabelNodeSizeLens,
            GraphConfigKey.EdgeThicknessImmersion => graphConfig.EdgeThicknessImmersion,
            GraphConfigKey.EdgeThicknessDesk => graphConfig.EdgeThicknessDesk,
            GraphConfigKey.EdgeThicknessLens => graphConfig.EdgeThicknessLens,
            GraphConfigKey.EdgeThicknessWatch => graphConfig.EdgeThicknessWatch,
            GraphConfigKey.NodeColorMappingBoundaryColorA => graphConfig.NodeColorMapping.BoundaryColorA,
            GraphConfigKey.NodeColorMappingBoundaryColorB => graphConfig.NodeColorMapping.BoundaryColorB,
            GraphConfigKey.NodeColorMappingBoundaryColorC => graphConfig.NodeColorMapping.BoundaryColorC,
            GraphConfigKey.AlphaNodeColorPropagated => graphConfig.AlphaNodeColorPropagated,
            GraphConfigKey.AlphaNodeColorUnPropagated => graphConfig.AlphaNodeColorUnPropagated,
            GraphConfigKey.AlphaEdgeColorPropagated => graphConfig.AlphaEdgeColorPropagated,
            GraphConfigKey.AlphaEdgeColorUnPropagated => graphConfig.AlphaEdgeColorUnPropagated,
            GraphConfigKey.NbOntologyColor => graphConfig.NbOntologyColor,
            GraphConfigKey.MaxDeltaOntologyAlgo => graphConfig.MaxDeltaOntologyAlgo,
            GraphConfigKey.SaturationOntologyColor => graphConfig.SaturationOntologyColor,
            GraphConfigKey.ValueOntologyColor => graphConfig.ValueOntologyColor,
            GraphConfigKey.LabelNodgePropagation => graphConfig.LabelNodgePropagation,
            GraphConfigKey.SeedRandomPosition => graphConfig.SeedRandomPosition,
            GraphConfigKey.GraphModeTransitionTime => graphConfig.GraphModeTransitionTime,
            GraphConfigKey.DefaultTickDeltaTime => graphConfig.SimuParameters.TickDeltaTime,
            GraphConfigKey.DefaultMaxSimulationTime => graphConfig.SimuParameters.MaxSimulationTime,
            GraphConfigKey.DefaultLerpSmooth => graphConfig.SimuParameters.LerpSmooth,
            GraphConfigKey.DefaultLightSpringForce => graphConfig.SimuParameters.LightSpringForce,
            GraphConfigKey.DefaultLightCoulombForce => graphConfig.SimuParameters.LightCoulombForce,
            GraphConfigKey.DefaultLightDamping => graphConfig.SimuParameters.LightDamping,
            GraphConfigKey.DefaultLightSpringDistance => graphConfig.SimuParameters.LightSpringDistance,
            GraphConfigKey.DefaultLightCoulombDistance => graphConfig.SimuParameters.LightCoulombDistance,
            GraphConfigKey.DefaultLightMaxVelocity => graphConfig.SimuParameters.LightMaxVelocity,
            GraphConfigKey.DefaultLightStopVelocity => graphConfig.SimuParameters.LightStopVelocity,
            GraphConfigKey.DefaultDenseSpringForce => graphConfig.SimuParameters.DenseSpringForce,
            GraphConfigKey.DefaultDenseCoulombForce => graphConfig.SimuParameters.DenseCoulombForce,
            GraphConfigKey.DefaultDenseDamping => graphConfig.SimuParameters.DenseDamping,
            GraphConfigKey.DefaultDenseSpringDistance => graphConfig.SimuParameters.DenseSpringDistance,
            GraphConfigKey.DefaultDenseCoulombDistance => graphConfig.SimuParameters.DenseCoulombDistance,
            GraphConfigKey.DefaultDenseMaxVelocity => graphConfig.SimuParameters.DenseMaxVelocity,
            GraphConfigKey.DefaultDenseStopVelocity => graphConfig.SimuParameters.DenseStopVelocity,
            GraphConfigKey.LensTickDeltaTime => graphConfig.LensSimuParameters.TickDeltaTime,
            GraphConfigKey.LensMaxSimulationTime => graphConfig.LensSimuParameters.MaxSimulationTime,
            GraphConfigKey.LensLerpSmooth => graphConfig.LensSimuParameters.LerpSmooth,
            GraphConfigKey.LensLightSpringForce => graphConfig.LensSimuParameters.LightSpringForce,
            GraphConfigKey.LensLightCoulombForce => graphConfig.LensSimuParameters.LightCoulombForce,
            GraphConfigKey.LensLightDamping => graphConfig.LensSimuParameters.LightDamping,
            GraphConfigKey.LensLightSpringDistance => graphConfig.LensSimuParameters.LightSpringDistance,
            GraphConfigKey.LensLightCoulombDistance => graphConfig.LensSimuParameters.LightCoulombDistance,
            GraphConfigKey.LensLightMaxVelocity => graphConfig.LensSimuParameters.LightMaxVelocity,
            GraphConfigKey.LensLightStopVelocity => graphConfig.LensSimuParameters.LightStopVelocity,
            GraphConfigKey.LensDenseSpringForce => graphConfig.LensSimuParameters.DenseSpringForce,
            GraphConfigKey.LensDenseCoulombForce => graphConfig.LensSimuParameters.DenseCoulombForce,
            GraphConfigKey.LensDenseDamping => graphConfig.LensSimuParameters.DenseDamping,
            GraphConfigKey.LensDenseSpringDistance => graphConfig.LensSimuParameters.DenseSpringDistance,
            GraphConfigKey.LensDenseCoulombDistance => graphConfig.LensSimuParameters.DenseCoulombDistance,
            GraphConfigKey.LensDenseMaxVelocity => graphConfig.LensSimuParameters.DenseMaxVelocity,
            GraphConfigKey.LensDenseStopVelocity => graphConfig.LensSimuParameters.DenseStopVelocity,
            _ => 0f,
        };
    }

    private static bool GetBoolValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        return key switch
        {
            GraphConfigKey.GraphMode => (graphConfig.GraphMode == GraphMode.Immersion),
            GraphConfigKey.ShowLabelImmersion => graphConfig.ShowLabelImmersion,
            GraphConfigKey.ShowLabelDesk => graphConfig.ShowLabelDesk,
            GraphConfigKey.CanSelectEdges => graphConfig.CanSelectEdges,
            GraphConfigKey.DisplayEdges => graphConfig.DisplayEdges,
            GraphConfigKey.ResetPositionNodeOnUpdate => graphConfig.ResetPositionNodeOnUpdate,
            GraphConfigKey.DisplayInterSelectedNeighborEdges => graphConfig.DisplayInterSelectedNeighborEdges,
            GraphConfigKey.ShowWatch => graphConfig.ShowWatch,
            _ => false,
        };
    }

    private static Color GetColorValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        return key switch
        {
            GraphConfigKey.NodeColor => graphConfig.NodeColor,
            GraphConfigKey.NodeColorNoValueMetric => graphConfig.NodeColorNoValueMetric,
            GraphConfigKey.EdgeColor => graphConfig.EdgeColor,
            GraphConfigKey.PropagatedEdgeColor => graphConfig.PropagatedEdgeColor,
            GraphConfigKey.NodeColorMappingColorA => graphConfig.NodeColorMapping.ColorA,
            GraphConfigKey.NodeColorMappingColorB => graphConfig.NodeColorMapping.ColorB,
            GraphConfigKey.NodeColorMappingColorC => graphConfig.NodeColorMapping.ColorC,
            _ => Color.white,
        };
    }

    public static T StringToEnum<T>(string enumString) where T : struct, Enum
    {
        if (Enum.TryParse(enumString, true, out T result))

            return result;

        Debug.LogWarning($"StringToEnum<{typeof(T).Name}> couldn't parse the string: {enumString}");

        return  default;
    }

}
