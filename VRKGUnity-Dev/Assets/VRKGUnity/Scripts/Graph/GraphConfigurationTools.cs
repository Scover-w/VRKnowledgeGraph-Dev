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
        switch (key)
        {
            case GraphConfigKey.SimuParameters:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensSimuParameters:
                return GraphConfigValueType.Float;
            case GraphConfigKey.ImmersionGraphSize:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DeskGraphSize:
                return GraphConfigValueType.Float;
            case GraphConfigKey.WatchGraphSize:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensGraphSize:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeSizeImmersion:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeSizeDesk:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeSizeWatch:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeSizeLens:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeMinSizeImmersion:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeMaxSizeImmersion:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeMinSizeDesk:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeMaxSizeDesk:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeMinSizeLens:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeMaxSizeLens:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LabelNodeSizeImmersion:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LabelNodeSizeDesk:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LabelNodeSizeLens:
                return GraphConfigValueType.Float;
            case GraphConfigKey.ShowLabelImmersion:
                return GraphConfigValueType.Bool;
            case GraphConfigKey.ShowLabelDesk:
                return GraphConfigValueType.Bool;
            case GraphConfigKey.ShowLabelLens:
                return GraphConfigValueType.Bool;
            case GraphConfigKey.EdgeThicknessImmersion:
                return GraphConfigValueType.Float;
            case GraphConfigKey.EdgeThicknessDesk:
                return GraphConfigValueType.Float;
            case GraphConfigKey.EdgeThicknessLens:
                return GraphConfigValueType.Float;
            case GraphConfigKey.EdgeThicknessWatch:
                return GraphConfigValueType.Float;
            case GraphConfigKey.CanSelectEdges:
                return GraphConfigValueType.Bool;
            case GraphConfigKey.DisplayEdges:
                return GraphConfigValueType.Bool;
            case GraphConfigKey.NodeColor:
                return GraphConfigValueType.Color;
            case GraphConfigKey.NodeColorNoValueMetric:
                return GraphConfigValueType.Color;
            case GraphConfigKey.EdgeColor:
                return GraphConfigValueType.Color;
            case GraphConfigKey.PropagatedEdgeColor:
                return GraphConfigValueType.Color;
            case GraphConfigKey.NodeColorMappingColorA:
                return GraphConfigValueType.Color;
            case GraphConfigKey.NodeColorMappingColorB:
                return GraphConfigValueType.Color;
            case GraphConfigKey.NodeColorMappingColorC:
                return GraphConfigValueType.Color;
            case GraphConfigKey.NodeColorMappingBoundaryColorA:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeColorMappingBoundaryColorB:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NodeColorMappingBoundaryColorC:
                return GraphConfigValueType.Float;
            case GraphConfigKey.AlphaNodeColorPropagated:
                return GraphConfigValueType.Float;
            case GraphConfigKey.AlphaNodeColorUnPropagated:
                return GraphConfigValueType.Float;
            case GraphConfigKey.AlphaEdgeColorPropagated:
                return GraphConfigValueType.Float;
            case GraphConfigKey.AlphaEdgeColorUnPropagated:
                return GraphConfigValueType.Float;
            case GraphConfigKey.NbOntologyColor:
                return GraphConfigValueType.Float;
            case GraphConfigKey.MaxDeltaOntologyAlgo:
                return GraphConfigValueType.Float;
            case GraphConfigKey.SaturationOntologyColor:
                return GraphConfigValueType.Float;
            case GraphConfigKey.ValueOntologyColor:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LabelNodgePropagation:
                return GraphConfigValueType.Float;
            case GraphConfigKey.ResetPositionNodeOnUpdate:
                return GraphConfigValueType.Bool;
            case GraphConfigKey.SeedRandomPosition:
                return GraphConfigValueType.Float;
            case GraphConfigKey.GraphModeTransitionTime:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DisplayInterSelectedNeighborEdges:
                return GraphConfigValueType.Bool;
            case GraphConfigKey.ShowWatch:
                return GraphConfigValueType.Bool;
            case GraphConfigKey.SelectedMetricTypeSize:
                return GraphConfigValueType.String;
            case GraphConfigKey.SelectedMetricTypeColor:
                return GraphConfigValueType.String;
            case GraphConfigKey.DefaultSimulationParameters:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultTickDeltaTime:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultMaxSimulationTime:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultLerpSmooth:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultLightSpringForce:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultLightCoulombForce:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultLightDamping:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultLightSpringDistance:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultLightCoulombDistance:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultLightMaxVelocity:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultLightStopVelocity:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultDenseSpringForce:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultDenseCoulombForce:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultDenseDamping:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultDenseSpringDistance:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultDenseCoulombDistance:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultDenseMaxVelocity:
                return GraphConfigValueType.Float;
            case GraphConfigKey.DefaultDenseStopVelocity:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensSimulationParameters:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensTickDeltaTime:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensMaxSimulationTime:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensLerpSmooth:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensLightSpringForce:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensLightCoulombForce:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensLightDamping:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensLightSpringDistance:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensLightCoulombDistance:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensLightMaxVelocity:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensLightStopVelocity:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensDenseSpringForce:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensDenseCoulombForce:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensDenseDamping:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensDenseSpringDistance:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensDenseCoulombDistance:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensDenseMaxVelocity:
                return GraphConfigValueType.Float;
            case GraphConfigKey.LensDenseStopVelocity:
                return GraphConfigValueType.Float;
            default:
                return GraphConfigValueType.Float;
        }
    }

    private static Type GetRealType(this GraphConfigKey key)
    {
        switch (key)
        {
            case GraphConfigKey.SelectedMode:
                return typeof(bool);
            case GraphConfigKey.SimuParameters:
                return typeof(float);
            case GraphConfigKey.LensSimuParameters:
                return typeof(float);
            case GraphConfigKey.ImmersionGraphSize:
                return typeof(float);
            case GraphConfigKey.DeskGraphSize:
                return typeof(float);
            case GraphConfigKey.WatchGraphSize:
                return typeof(float);
            case GraphConfigKey.LensGraphSize:
                return typeof(float);
            case GraphConfigKey.NodeSizeImmersion:
                return typeof(float);
            case GraphConfigKey.NodeSizeDesk:
                return typeof(float);
            case GraphConfigKey.NodeSizeWatch:
                return typeof(float);
            case GraphConfigKey.NodeSizeLens:
                return typeof(float);
            case GraphConfigKey.NodeMinSizeImmersion:
                return typeof(float);
            case GraphConfigKey.NodeMaxSizeImmersion:
                return typeof(float);
            case GraphConfigKey.NodeMinSizeDesk:
                return typeof(float);
            case GraphConfigKey.NodeMaxSizeDesk:
                return typeof(float);
            case GraphConfigKey.NodeMinSizeLens:
                return typeof(float);
            case GraphConfigKey.NodeMaxSizeLens:
                return typeof(float);
            case GraphConfigKey.LabelNodeSizeImmersion:
                return typeof(float);
            case GraphConfigKey.LabelNodeSizeDesk:
                return typeof(float);
            case GraphConfigKey.LabelNodeSizeLens:
                return typeof(float);
            case GraphConfigKey.ShowLabelImmersion:
                return typeof(bool);
            case GraphConfigKey.ShowLabelDesk:
                return typeof(bool);
            case GraphConfigKey.ShowLabelLens:
                return typeof(bool);
            case GraphConfigKey.EdgeThicknessImmersion:
                return typeof(float);
            case GraphConfigKey.EdgeThicknessDesk:
                return typeof(float);
            case GraphConfigKey.EdgeThicknessLens:
                return typeof(float);
            case GraphConfigKey.EdgeThicknessWatch:
                return typeof(float);
            case GraphConfigKey.CanSelectEdges:
                return typeof(bool);
            case GraphConfigKey.DisplayEdges:
                return typeof(bool);
            case GraphConfigKey.NodeColor:
                return typeof(Color);
            case GraphConfigKey.NodeColorNoValueMetric:
                return typeof(Color);
            case GraphConfigKey.EdgeColor:
                return typeof(Color);
            case GraphConfigKey.PropagatedEdgeColor:
                return typeof(Color);
            case GraphConfigKey.NodeColorMappingColorA:
                return typeof(Color);
            case GraphConfigKey.NodeColorMappingColorB:
                return typeof(Color);
            case GraphConfigKey.NodeColorMappingColorC:
                return typeof(Color);
            case GraphConfigKey.NodeColorMappingBoundaryColorA:
                return typeof(float);
            case GraphConfigKey.NodeColorMappingBoundaryColorB:
                return typeof(float);
            case GraphConfigKey.NodeColorMappingBoundaryColorC:
                return typeof(float);
            case GraphConfigKey.AlphaNodeColorPropagated:
                return typeof(float);
            case GraphConfigKey.AlphaNodeColorUnPropagated:
                return typeof(float);
            case GraphConfigKey.AlphaEdgeColorPropagated:
                return typeof(float);
            case GraphConfigKey.AlphaEdgeColorUnPropagated:
                return typeof(float);
            case GraphConfigKey.NbOntologyColor:
                return typeof(int);
            case GraphConfigKey.MaxDeltaOntologyAlgo:
                return typeof(int);
            case GraphConfigKey.SaturationOntologyColor:
                return typeof(float);
            case GraphConfigKey.ValueOntologyColor:
                return typeof(float);
            case GraphConfigKey.LabelNodgePropagation:
                return typeof(int);
            case GraphConfigKey.ResetPositionNodeOnUpdate:
                return typeof(bool);
            case GraphConfigKey.SeedRandomPosition:
                return typeof(int);
            case GraphConfigKey.GraphModeTransitionTime:
                return typeof(float);
            case GraphConfigKey.DisplayInterSelectedNeighborEdges:
                return typeof(bool);
            case GraphConfigKey.ShowWatch:
                return typeof(bool);
            case GraphConfigKey.SelectedMetricTypeSize:
                return typeof(GraphMetricType);
            case GraphConfigKey.SelectedMetricTypeColor:
                return typeof(GraphMetricType);
            case GraphConfigKey.DefaultSimulationParameters:
                return typeof(float);
            case GraphConfigKey.DefaultTickDeltaTime:
                return typeof(float);
            case GraphConfigKey.DefaultMaxSimulationTime:
                return typeof(float);
            case GraphConfigKey.DefaultLerpSmooth:
                return typeof(float);
            case GraphConfigKey.DefaultLightSpringForce:
                return typeof(float);
            case GraphConfigKey.DefaultLightCoulombForce:
                return typeof(float);
            case GraphConfigKey.DefaultLightDamping:
                return typeof(float);
            case GraphConfigKey.DefaultLightSpringDistance:
                return typeof(float);
            case GraphConfigKey.DefaultLightCoulombDistance:
                return typeof(float);
            case GraphConfigKey.DefaultLightMaxVelocity:
                return typeof(float);
            case GraphConfigKey.DefaultLightStopVelocity:
                return typeof(float);
            case GraphConfigKey.DefaultDenseSpringForce:
                return typeof(float);
            case GraphConfigKey.DefaultDenseCoulombForce:
                return typeof(float);
            case GraphConfigKey.DefaultDenseDamping:
                return typeof(float);
            case GraphConfigKey.DefaultDenseSpringDistance:
                return typeof(float);
            case GraphConfigKey.DefaultDenseCoulombDistance:
                return typeof(float);
            case GraphConfigKey.DefaultDenseMaxVelocity:
                return typeof(float);
            case GraphConfigKey.DefaultDenseStopVelocity:
                return typeof(float);
            case GraphConfigKey.LensSimulationParameters:
                return typeof(float);
            case GraphConfigKey.LensTickDeltaTime:
                return typeof(float);
            case GraphConfigKey.LensMaxSimulationTime:
                return typeof(float);
            case GraphConfigKey.LensLerpSmooth:
                return typeof(float);
            case GraphConfigKey.LensLightSpringForce:
                return typeof(float);
            case GraphConfigKey.LensLightCoulombForce:
                return typeof(float);
            case GraphConfigKey.LensLightDamping:
                return typeof(float);
            case GraphConfigKey.LensLightSpringDistance:
                return typeof(float);
            case GraphConfigKey.LensLightCoulombDistance:
                return typeof(float);
            case GraphConfigKey.LensLightMaxVelocity:
                return typeof(float);
            case GraphConfigKey.LensLightStopVelocity:
                return typeof(float);
            case GraphConfigKey.LensDenseSpringForce:
                return typeof(float);
            case GraphConfigKey.LensDenseCoulombForce:
                return typeof(float);
            case GraphConfigKey.LensDenseDamping:
                return typeof(float);
            case GraphConfigKey.LensDenseSpringDistance:
                return typeof(float);
            case GraphConfigKey.LensDenseCoulombDistance:
                return typeof(float);
            case GraphConfigKey.LensDenseMaxVelocity:
                return typeof(float);
            case GraphConfigKey.LensDenseStopVelocity:
                return typeof(float);
            default:
                return typeof(float);
        }
    }


    public static string GetStringValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        switch (key)
        {
            case GraphConfigKey.SelectedMetricTypeSize:
                return graphConfig.SelectedMetricTypeSize.ToString();
            case GraphConfigKey.SelectedMetricTypeColor:
                return graphConfig.SelectedMetricTypeColor.ToString();
        }

        return "";
    }

    public static float GetFloatValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        switch (key)
        {
            case GraphConfigKey.ImmersionGraphSize:
                return graphConfig.ImmersionGraphSize;
            case GraphConfigKey.DeskGraphSize:
                return graphConfig.DeskGraphSize;
            case GraphConfigKey.WatchGraphSize:
                return graphConfig.WatchGraphSize;
            case GraphConfigKey.LensGraphSize:
                return graphConfig.LensGraphSize;
            case GraphConfigKey.NodeSizeImmersion:
                return graphConfig.NodeSizeImmersion;
            case GraphConfigKey.NodeSizeDesk:
                return graphConfig.NodeSizeDesk;
            case GraphConfigKey.NodeSizeWatch:
                return graphConfig.NodeSizeWatch;
            case GraphConfigKey.NodeSizeLens:
                return graphConfig.NodeSizeLens;
            case GraphConfigKey.NodeMinSizeImmersion:
                return graphConfig.NodeMinSizeImmersion;
            case GraphConfigKey.NodeMaxSizeImmersion:
                return graphConfig.NodeMaxSizeImmersion;
            case GraphConfigKey.NodeMinSizeDesk:
                return graphConfig.NodeMinSizeDesk;
            case GraphConfigKey.NodeMaxSizeDesk:
                return graphConfig.NodeMaxSizeDesk;
            case GraphConfigKey.NodeMinSizeLens:
                return graphConfig.NodeMinSizeLens;
            case GraphConfigKey.NodeMaxSizeLens:
                return graphConfig.NodeMaxSizeLens;
            case GraphConfigKey.LabelNodeSizeImmersion:
                return graphConfig.LabelNodeSizeImmersion;
            case GraphConfigKey.LabelNodeSizeDesk:
                return graphConfig.LabelNodeSizeDesk;
            case GraphConfigKey.LabelNodeSizeLens:
                return graphConfig.LabelNodeSizeLens;
            case GraphConfigKey.EdgeThicknessImmersion:
                return graphConfig.EdgeThicknessImmersion;
            case GraphConfigKey.EdgeThicknessDesk:
                return graphConfig.EdgeThicknessDesk;
            case GraphConfigKey.EdgeThicknessLens:
                return graphConfig.EdgeThicknessLens;
            case GraphConfigKey.EdgeThicknessWatch:
                return graphConfig.EdgeThicknessWatch;
            case GraphConfigKey.NodeColorMappingBoundaryColorA:
                return graphConfig.NodeColorMapping.BoundaryColorA;
            case GraphConfigKey.NodeColorMappingBoundaryColorB:
                return graphConfig.NodeColorMapping.BoundaryColorB;
            case GraphConfigKey.NodeColorMappingBoundaryColorC:
                return graphConfig.NodeColorMapping.BoundaryColorC;
            case GraphConfigKey.AlphaNodeColorPropagated:
                return graphConfig.AlphaNodeColorPropagated;
            case GraphConfigKey.AlphaNodeColorUnPropagated:
                return graphConfig.AlphaNodeColorUnPropagated;
            case GraphConfigKey.AlphaEdgeColorPropagated:
                return graphConfig.AlphaEdgeColorPropagated;
            case GraphConfigKey.AlphaEdgeColorUnPropagated:
                return graphConfig.AlphaEdgeColorUnPropagated;
            case GraphConfigKey.NbOntologyColor:
                return graphConfig.NbOntologyColor;
            case GraphConfigKey.MaxDeltaOntologyAlgo:
                return graphConfig.MaxDeltaOntologyAlgo;
            case GraphConfigKey.SaturationOntologyColor:
                return graphConfig.SaturationOntologyColor;
            case GraphConfigKey.ValueOntologyColor:
                return graphConfig.ValueOntologyColor;
            case GraphConfigKey.LabelNodgePropagation:
                return graphConfig.LabelNodgePropagation;
            case GraphConfigKey.SeedRandomPosition:
                return graphConfig.SeedRandomPosition;
            case GraphConfigKey.GraphModeTransitionTime:
                return graphConfig.GraphModeTransitionTime;
            case GraphConfigKey.DefaultTickDeltaTime:
                return graphConfig.SimuParameters.TickDeltaTime;
            case GraphConfigKey.DefaultMaxSimulationTime:
                return graphConfig.SimuParameters.MaxSimulationTime;
            case GraphConfigKey.DefaultLerpSmooth:
                return graphConfig.SimuParameters.LerpSmooth;
            case GraphConfigKey.DefaultLightSpringForce:
                return graphConfig.SimuParameters.LightSpringForce;
            case GraphConfigKey.DefaultLightCoulombForce:
                return graphConfig.SimuParameters.LightCoulombForce;
            case GraphConfigKey.DefaultLightDamping:
                return graphConfig.SimuParameters.LightDamping;
            case GraphConfigKey.DefaultLightSpringDistance:
                return graphConfig.SimuParameters.LightSpringDistance;
            case GraphConfigKey.DefaultLightCoulombDistance:
                return graphConfig.SimuParameters.LightCoulombDistance;
            case GraphConfigKey.DefaultLightMaxVelocity:
                return graphConfig.SimuParameters.LightMaxVelocity;
            case GraphConfigKey.DefaultLightStopVelocity:
                return graphConfig.SimuParameters.LightStopVelocity;
            case GraphConfigKey.DefaultDenseSpringForce:
                return graphConfig.SimuParameters.DenseSpringForce;
            case GraphConfigKey.DefaultDenseCoulombForce:
                return graphConfig.SimuParameters.DenseCoulombForce;
            case GraphConfigKey.DefaultDenseDamping:
                return graphConfig.SimuParameters.DenseDamping;
            case GraphConfigKey.DefaultDenseSpringDistance:
                return graphConfig.SimuParameters.DenseSpringDistance;
            case GraphConfigKey.DefaultDenseCoulombDistance:
                return graphConfig.SimuParameters.DenseCoulombDistance;
            case GraphConfigKey.DefaultDenseMaxVelocity:
                return graphConfig.SimuParameters.DenseMaxVelocity;
            case GraphConfigKey.DefaultDenseStopVelocity:
                return graphConfig.SimuParameters.DenseStopVelocity;
            case GraphConfigKey.LensTickDeltaTime:
                return graphConfig.LensSimuParameters.TickDeltaTime;
            case GraphConfigKey.LensMaxSimulationTime:
                return graphConfig.LensSimuParameters.MaxSimulationTime;
            case GraphConfigKey.LensLerpSmooth:
                return graphConfig.LensSimuParameters.LerpSmooth;
            case GraphConfigKey.LensLightSpringForce:
                return graphConfig.LensSimuParameters.LightSpringForce;
            case GraphConfigKey.LensLightCoulombForce:
                return graphConfig.LensSimuParameters.LightCoulombForce;
            case GraphConfigKey.LensLightDamping:
                return graphConfig.LensSimuParameters.LightDamping;
            case GraphConfigKey.LensLightSpringDistance:
                return graphConfig.LensSimuParameters.LightSpringDistance;
            case GraphConfigKey.LensLightCoulombDistance:
                return graphConfig.LensSimuParameters.LightCoulombDistance;
            case GraphConfigKey.LensLightMaxVelocity:
                return graphConfig.LensSimuParameters.LightMaxVelocity;
            case GraphConfigKey.LensLightStopVelocity:
                return graphConfig.LensSimuParameters.LightStopVelocity;
            case GraphConfigKey.LensDenseSpringForce:
                return graphConfig.LensSimuParameters.DenseSpringForce;
            case GraphConfigKey.LensDenseCoulombForce:
                return graphConfig.LensSimuParameters.DenseCoulombForce;
            case GraphConfigKey.LensDenseDamping:
                return graphConfig.LensSimuParameters.DenseDamping;
            case GraphConfigKey.LensDenseSpringDistance:
                return graphConfig.LensSimuParameters.DenseSpringDistance;
            case GraphConfigKey.LensDenseCoulombDistance:
                return graphConfig.LensSimuParameters.DenseCoulombDistance;
            case GraphConfigKey.LensDenseMaxVelocity:
                return graphConfig.LensSimuParameters.DenseMaxVelocity;
            case GraphConfigKey.LensDenseStopVelocity:
                return graphConfig.LensSimuParameters.DenseStopVelocity;
        }
        
        return 0f;
    }

    public static bool GetBoolValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        switch (key)
        {
            case GraphConfigKey.ShowLabelImmersion:
                return graphConfig.ShowLabelImmersion;
            case GraphConfigKey.ShowLabelDesk:
                return graphConfig.ShowLabelDesk;
            case GraphConfigKey.CanSelectEdges:
                return graphConfig.CanSelectEdges;
            case GraphConfigKey.DisplayEdges:
                return graphConfig.DisplayEdges;
            case GraphConfigKey.ResetPositionNodeOnUpdate:
                return graphConfig.ResetPositionNodeOnUpdate;
            case GraphConfigKey.DisplayInterSelectedNeighborEdges:
                return graphConfig.DisplayInterSelectedNeighborEdges;
            case GraphConfigKey.ShowWatch:
                return graphConfig.ShowWatch;
        }

        return false;
    }

    public static Color GetColorValue(this GraphConfigKey key, GraphConfiguration graphConfig)
    {
        switch (key)
        {
            case GraphConfigKey.NodeColor:
                return graphConfig.NodeColor;
            case GraphConfigKey.NodeColorNoValueMetric:
                return graphConfig.NodeColorNoValueMetric;
            case GraphConfigKey.EdgeColor:
                return graphConfig.EdgeColor;
            case GraphConfigKey.PropagatedEdgeColor:
                return graphConfig.PropagatedEdgeColor;
            case GraphConfigKey.NodeColorMappingColorA:
                return graphConfig.NodeColorMapping.ColorA;
            case GraphConfigKey.NodeColorMappingColorB:
                return graphConfig.NodeColorMapping.ColorB;
            case GraphConfigKey.NodeColorMappingColorC:
                return graphConfig.NodeColorMapping.ColorC;
        }


        return Color.white;
    }

    public static GraphMetricType StringToEnum(string metricType)
    {
        if (Enum.TryParse(metricType, out GraphMetricType result))
 
            return result;

        Debug.LogWarning("StringToEnum GraphMetricType couldn't parse the string");

        return GraphMetricType.None;
    }

}
