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
            GraphConfigKey.GPSGraphSize => GraphConfigValueType.Float,
            GraphConfigKey.LensGraphSize => GraphConfigValueType.Float,
            GraphConfigKey.NodeSizeImmersion => GraphConfigValueType.Float,
            GraphConfigKey.NodeSizeDesk => GraphConfigValueType.Float,
            GraphConfigKey.NodeSizeGPS => GraphConfigValueType.Float,
            GraphConfigKey.NodeSizeLens => GraphConfigValueType.Float,
            GraphConfigKey.NodeMinMaxSizeImmersion => GraphConfigValueType.Float,
            GraphConfigKey.NodeMinMaxSizeDesk => GraphConfigValueType.Float,
            GraphConfigKey.NodeMinMaxSizeLens => GraphConfigValueType.Float,
            GraphConfigKey.LabelNodeSizeImmersion => GraphConfigValueType.Float,
            GraphConfigKey.LabelNodeSizeDesk => GraphConfigValueType.Float,
            GraphConfigKey.LabelNodeSizeLens => GraphConfigValueType.Float,
            GraphConfigKey.DisplayLabelImmersion => GraphConfigValueType.Bool,
            GraphConfigKey.DisplayLabelDesk => GraphConfigValueType.Bool,
            GraphConfigKey.DisplayLabelLens => GraphConfigValueType.Bool,
            GraphConfigKey.EdgeThicknessImmersion => GraphConfigValueType.Float,
            GraphConfigKey.EdgeThicknessDesk => GraphConfigValueType.Float,
            GraphConfigKey.EdgeThicknessLens => GraphConfigValueType.Float,
            GraphConfigKey.EdgeThicknessGPS => GraphConfigValueType.Float,
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
            GraphConfigKey.GlobalVolume => GraphConfigValueType.Float,
            GraphConfigKey.SoundEffectVolume => GraphConfigValueType.Float,
            GraphConfigKey.MusicVolume => GraphConfigValueType.Float,
            GraphConfigKey.AidenVolume => GraphConfigValueType.Float,
            GraphConfigKey.LabelNodgePropagation => GraphConfigValueType.Float,
            GraphConfigKey.ResetPositionNodeOnUpdate => GraphConfigValueType.Bool,
            GraphConfigKey.SeedRandomPosition => GraphConfigValueType.Float,
            GraphConfigKey.GraphModeTransitionTime => GraphConfigValueType.Float,
            GraphConfigKey.DisplayInterSelectedNeighborEdges => GraphConfigValueType.Bool,
            GraphConfigKey.DisplayGPS => GraphConfigValueType.Bool,
            GraphConfigKey.SelectedMetricTypeSize => GraphConfigValueType.String,
            GraphConfigKey.SelectedMetricTypeColor => GraphConfigValueType.String,
            GraphConfigKey.DefaultSimulationParameters => GraphConfigValueType.Float,
            GraphConfigKey.DefaultTickDeltaTime => GraphConfigValueType.Float,
            GraphConfigKey.DefaultMaxSimulationTime => GraphConfigValueType.Float,
            GraphConfigKey.DefaultLerpSmooth => GraphConfigValueType.Float,
            GraphConfigKey.DefaultSpringForce => GraphConfigValueType.Float,
            GraphConfigKey.DefaultCoulombForce => GraphConfigValueType.Float,
            GraphConfigKey.DefaultDamping => GraphConfigValueType.Float,
            GraphConfigKey.DefaultSpringDistance => GraphConfigValueType.Float,
            GraphConfigKey.DefaultCoulombDistance => GraphConfigValueType.Float,
            GraphConfigKey.DefaultMaxVelocity => GraphConfigValueType.Float,
            GraphConfigKey.DefaultStopVelocity => GraphConfigValueType.Float,
            GraphConfigKey.LensSimulationParameters => GraphConfigValueType.Float,
            GraphConfigKey.LensTickDeltaTime => GraphConfigValueType.Float,
            GraphConfigKey.LensMaxSimulationTime => GraphConfigValueType.Float,
            GraphConfigKey.LensLerpSmooth => GraphConfigValueType.Float,
            GraphConfigKey.LensSpringForce => GraphConfigValueType.Float,
            GraphConfigKey.LensCoulombForce => GraphConfigValueType.Float,
            GraphConfigKey.LensDamping => GraphConfigValueType.Float,
            GraphConfigKey.LensSpringDistance => GraphConfigValueType.Float,
            GraphConfigKey.LensCoulombDistance => GraphConfigValueType.Float,
            GraphConfigKey.LensMaxVelocity => GraphConfigValueType.Float,
            GraphConfigKey.LensStopVelocity => GraphConfigValueType.Float,
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
            GraphConfigKey.GPSGraphSize => typeof(float),
            GraphConfigKey.LensGraphSize => typeof(float),
            GraphConfigKey.NodeSizeImmersion => typeof(float),
            GraphConfigKey.NodeSizeDesk => typeof(float),
            GraphConfigKey.NodeSizeGPS => typeof(float),
            GraphConfigKey.NodeSizeLens => typeof(float),
            GraphConfigKey.NodeMinMaxSizeImmersion => typeof(float),
            GraphConfigKey.NodeMinMaxSizeDesk => typeof(float),
            GraphConfigKey.NodeMinMaxSizeLens => typeof(float),
            GraphConfigKey.LabelNodeSizeImmersion => typeof(float),
            GraphConfigKey.LabelNodeSizeDesk => typeof(float),
            GraphConfigKey.LabelNodeSizeLens => typeof(float),
            GraphConfigKey.DisplayLabelImmersion => typeof(bool),
            GraphConfigKey.DisplayLabelDesk => typeof(bool),
            GraphConfigKey.DisplayLabelLens => typeof(bool),
            GraphConfigKey.EdgeThicknessImmersion => typeof(float),
            GraphConfigKey.EdgeThicknessDesk => typeof(float),
            GraphConfigKey.EdgeThicknessLens => typeof(float),
            GraphConfigKey.EdgeThicknessGPS => typeof(float),
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
            GraphConfigKey.GlobalVolume => typeof(float),
            GraphConfigKey.SoundEffectVolume => typeof(float),
            GraphConfigKey.MusicVolume => typeof(float),
            GraphConfigKey.AidenVolume => typeof(float),
            GraphConfigKey.LabelNodgePropagation => typeof(int),
            GraphConfigKey.ResetPositionNodeOnUpdate => typeof(bool),
            GraphConfigKey.SeedRandomPosition => typeof(int),
            GraphConfigKey.GraphModeTransitionTime => typeof(float),
            GraphConfigKey.DisplayInterSelectedNeighborEdges => typeof(bool),
            GraphConfigKey.DisplayGPS => typeof(bool),
            GraphConfigKey.SelectedMetricTypeSize => typeof(GraphMetricType),
            GraphConfigKey.SelectedMetricTypeColor => typeof(GraphMetricType),
            GraphConfigKey.DefaultSimulationParameters => typeof(float),
            GraphConfigKey.DefaultTickDeltaTime => typeof(float),
            GraphConfigKey.DefaultMaxSimulationTime => typeof(float),
            GraphConfigKey.DefaultLerpSmooth => typeof(float),
            GraphConfigKey.DefaultSpringForce => typeof(float),
            GraphConfigKey.DefaultCoulombForce => typeof(float),
            GraphConfigKey.DefaultDamping => typeof(float),
            GraphConfigKey.DefaultSpringDistance => typeof(float),
            GraphConfigKey.DefaultCoulombDistance => typeof(float),
            GraphConfigKey.DefaultMaxVelocity => typeof(float),
            GraphConfigKey.DefaultStopVelocity => typeof(float),
            GraphConfigKey.LensSimulationParameters => typeof(float),
            GraphConfigKey.LensTickDeltaTime => typeof(float),
            GraphConfigKey.LensMaxSimulationTime => typeof(float),
            GraphConfigKey.LensLerpSmooth => typeof(float),
            GraphConfigKey.LensSpringForce => typeof(float),
            GraphConfigKey.LensCoulombForce => typeof(float),
            GraphConfigKey.LensDamping => typeof(float),
            GraphConfigKey.LensSpringDistance => typeof(float),
            GraphConfigKey.LensCoulombDistance => typeof(float),
            GraphConfigKey.LensMaxVelocity => typeof(float),
            GraphConfigKey.LensStopVelocity => typeof(float),
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
            GraphConfigKey.GPSGraphSize => graphConfig.GPSGraphSize,
            GraphConfigKey.LensGraphSize => graphConfig.LensGraphSize,
            GraphConfigKey.NodeSizeImmersion => graphConfig.NodeSizeImmersion,
            GraphConfigKey.NodeSizeDesk => graphConfig.NodeSizeDesk,
            GraphConfigKey.NodeSizeGPS => graphConfig.NodeSizeGPS,
            GraphConfigKey.NodeSizeLens => graphConfig.NodeSizeLens,
            GraphConfigKey.NodeMinMaxSizeImmersion => graphConfig.NodeMinMaxSizeImmersion,
            GraphConfigKey.NodeMinMaxSizeDesk => graphConfig.NodeMinMaxSizeDesk,
            GraphConfigKey.NodeMinMaxSizeLens => graphConfig.NodeMinMaxSizeLens,
            GraphConfigKey.LabelNodeSizeImmersion => graphConfig.LabelNodeSizeImmersion,
            GraphConfigKey.LabelNodeSizeDesk => graphConfig.LabelNodeSizeDesk,
            GraphConfigKey.LabelNodeSizeLens => graphConfig.LabelNodeSizeLens,
            GraphConfigKey.EdgeThicknessImmersion => graphConfig.EdgeThicknessImmersion,
            GraphConfigKey.EdgeThicknessDesk => graphConfig.EdgeThicknessDesk,
            GraphConfigKey.EdgeThicknessLens => graphConfig.EdgeThicknessLens,
            GraphConfigKey.EdgeThicknessGPS => graphConfig.EdgeThicknessGPS,
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
            GraphConfigKey.GlobalVolume => graphConfig.GlobalVolume,
            GraphConfigKey.SoundEffectVolume => graphConfig.SoundEffectVolume,
            GraphConfigKey.MusicVolume => graphConfig.MusicVolume,
            GraphConfigKey.AidenVolume => graphConfig.AidenVolume,
            GraphConfigKey.LabelNodgePropagation => graphConfig.LabelNodgePropagation,
            GraphConfigKey.SeedRandomPosition => graphConfig.SeedRandomPosition,
            GraphConfigKey.GraphModeTransitionTime => graphConfig.GraphModeTransitionTime,
            GraphConfigKey.DefaultTickDeltaTime => graphConfig.SimuParameters.TickDeltaTime,
            GraphConfigKey.DefaultMaxSimulationTime => graphConfig.SimuParameters.MaxSimulationTime,
            GraphConfigKey.DefaultLerpSmooth => graphConfig.SimuParameters.LerpSmooth,
            GraphConfigKey.DefaultSpringForce => graphConfig.SimuParameters.SpringForce,
            GraphConfigKey.DefaultCoulombForce => graphConfig.SimuParameters.CoulombForce,
            GraphConfigKey.DefaultDamping => graphConfig.SimuParameters.Damping,
            GraphConfigKey.DefaultSpringDistance => graphConfig.SimuParameters.SpringDistance,
            GraphConfigKey.DefaultCoulombDistance => graphConfig.SimuParameters.CoulombDistance,
            GraphConfigKey.DefaultMaxVelocity => graphConfig.SimuParameters.MaxVelocity,
            GraphConfigKey.DefaultStopVelocity => graphConfig.SimuParameters.StopVelocity,
            GraphConfigKey.LensTickDeltaTime => graphConfig.LensSimuParameters.TickDeltaTime,
            GraphConfigKey.LensMaxSimulationTime => graphConfig.LensSimuParameters.MaxSimulationTime,
            GraphConfigKey.LensLerpSmooth => graphConfig.LensSimuParameters.LerpSmooth,
            GraphConfigKey.LensSpringForce => graphConfig.LensSimuParameters.SpringForce,
            GraphConfigKey.LensCoulombForce => graphConfig.LensSimuParameters.CoulombForce,
            GraphConfigKey.LensDamping => graphConfig.LensSimuParameters.Damping,
            GraphConfigKey.LensSpringDistance => graphConfig.LensSimuParameters.SpringDistance,
            GraphConfigKey.LensCoulombDistance => graphConfig.LensSimuParameters.CoulombDistance,
            GraphConfigKey.LensMaxVelocity => graphConfig.LensSimuParameters.MaxVelocity,
            GraphConfigKey.LensStopVelocity => graphConfig.LensSimuParameters.StopVelocity,
            _ => 0f,
        };
    }

    private static bool GetBoolValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        return key switch
        {
            GraphConfigKey.GraphMode => (graphConfig.GraphMode == GraphMode.Immersion),
            GraphConfigKey.DisplayLabelImmersion => graphConfig.DisplayLabelImmersion,
            GraphConfigKey.DisplayLabelDesk => graphConfig.DisplayLabelDesk,
            GraphConfigKey.CanSelectEdges => graphConfig.CanSelectEdges,
            GraphConfigKey.DisplayEdges => graphConfig.DisplayEdges,
            GraphConfigKey.ResetPositionNodeOnUpdate => graphConfig.ResetPositionNodeOnUpdate,
            GraphConfigKey.DisplayInterSelectedNeighborEdges => graphConfig.DisplayInterSelectedNeighborEdges,
            GraphConfigKey.DisplayGPS => graphConfig.DisplayGPS,
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
}
