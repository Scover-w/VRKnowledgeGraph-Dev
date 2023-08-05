using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public static class GraphConfigurationTools
{

    public static bool IsGoodType(this GraphConfigurationKey key, Type askedType)
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


    private static Type GetRealType(this GraphConfigurationKey key)
    {
        switch (key)
        {
            case GraphConfigurationKey.SimuParameters:
                return typeof(float);
            case GraphConfigurationKey.LensSimuParameters:
                return typeof(float);
            case GraphConfigurationKey.ImmersionGraphSize:
                return typeof(float);
            case GraphConfigurationKey.DeskGraphSize:
                return typeof(float);
            case GraphConfigurationKey.WatchGraphSize:
                return typeof(float);
            case GraphConfigurationKey.LensGraphSize:
                return typeof(float);
            case GraphConfigurationKey.NodeSizeImmersion:
                return typeof(float);
            case GraphConfigurationKey.NodeSizeDesk:
                return typeof(float);
            case GraphConfigurationKey.NodeSizeWatch:
                return typeof(float);
            case GraphConfigurationKey.NodeSizeLens:
                return typeof(float);
            case GraphConfigurationKey.NodeMinSizeImmersion:
                return typeof(float);
            case GraphConfigurationKey.NodeMaxSizeImmersion:
                return typeof(float);
            case GraphConfigurationKey.NodeMinSizeDesk:
                return typeof(float);
            case GraphConfigurationKey.NodeMaxSizeDesk:
                return typeof(float);
            case GraphConfigurationKey.NodeMinSizeLens:
                return typeof(float);
            case GraphConfigurationKey.NodeMaxSizeLens:
                return typeof(float);
            case GraphConfigurationKey.LabelNodeSizeImmersion:
                return typeof(float);
            case GraphConfigurationKey.LabelNodeSizeDesk:
                return typeof(float);
            case GraphConfigurationKey.LabelNodeSizeLens:
                return typeof(float);
            case GraphConfigurationKey.ShowLabelImmersion:
                return typeof(bool);
            case GraphConfigurationKey.ShowLabelDesk:
                return typeof(bool);
            case GraphConfigurationKey.ShowLabelLens:
                return typeof(bool);
            case GraphConfigurationKey.EdgeThicknessImmersion:
                return typeof(float);
            case GraphConfigurationKey.EdgeThicknessDesk:
                return typeof(float);
            case GraphConfigurationKey.EdgeThicknessLens:
                return typeof(float);
            case GraphConfigurationKey.EdgeThicknessWatch:
                return typeof(float);
            case GraphConfigurationKey.CanSelectEdges:
                return typeof(bool);
            case GraphConfigurationKey.DisplayEdges:
                return typeof(bool);
            case GraphConfigurationKey.NodeColor:
                return typeof(Color);
            case GraphConfigurationKey.NodeColorNoValueMetric:
                return typeof(Color);
            case GraphConfigurationKey.EdgeColor:
                return typeof(Color);
            case GraphConfigurationKey.PropagatedEdgeColor:
                return typeof(Color);
            case GraphConfigurationKey.NodeColorMappingColorA:
                return typeof(Color);
            case GraphConfigurationKey.NodeColorMappingColorB:
                return typeof(Color);
            case GraphConfigurationKey.NodeColorMappingColorC:
                return typeof(Color);
            case GraphConfigurationKey.NodeColorMappingBoundaryColorA:
                return typeof(float);
            case GraphConfigurationKey.NodeColorMappingBoundaryColorB:
                return typeof(float);
            case GraphConfigurationKey.NodeColorMappingBoundaryColorC:
                return typeof(float);
            case GraphConfigurationKey.AlphaNodeColorPropagated:
                return typeof(float);
            case GraphConfigurationKey.AlphaNodeColorUnPropagated:
                return typeof(float);
            case GraphConfigurationKey.AlphaEdgeColorPropagated:
                return typeof(float);
            case GraphConfigurationKey.AlphaEdgeColorUnPropagated:
                return typeof(float);
            case GraphConfigurationKey.NbOntologyColor:
                return typeof(int);
            case GraphConfigurationKey.MaxDeltaOntologyAlgo:
                return typeof(int);
            case GraphConfigurationKey.SaturationOntologyColor:
                return typeof(float);
            case GraphConfigurationKey.ValueOntologyColor:
                return typeof(float);
            case GraphConfigurationKey.LabelNodgePropagation:
                return typeof(int);
            case GraphConfigurationKey.ResetPositionNodeOnUpdate:
                return typeof(bool);
            case GraphConfigurationKey.SeedRandomPosition:
                return typeof(int);
            case GraphConfigurationKey.GraphModeTransitionTime:
                return typeof(float);
            case GraphConfigurationKey.DisplayInterSelectedNeighborEdges:
                return typeof(bool);
            case GraphConfigurationKey.ShowWatch:
                return typeof(bool);
            case GraphConfigurationKey.SelectedMetricTypeSize:
                return typeof(GraphMetricType);
            case GraphConfigurationKey.SelectedMetricTypeColor:
                return typeof(GraphMetricType);
            case GraphConfigurationKey.DefaultSimulationParameters:
                return typeof(float);
            case GraphConfigurationKey.DefaultTickDeltaTime:
                return typeof(float);
            case GraphConfigurationKey.DefaultMaxSimulationTime:
                return typeof(float);
            case GraphConfigurationKey.DefaultLerpSmooth:
                return typeof(float);
            case GraphConfigurationKey.DefaultLightSpringForce:
                return typeof(float);
            case GraphConfigurationKey.DefaultLightCoulombForce:
                return typeof(float);
            case GraphConfigurationKey.DefaultLightDamping:
                return typeof(float);
            case GraphConfigurationKey.DefaultLightSpringDistance:
                return typeof(float);
            case GraphConfigurationKey.DefaultLightCoulombDistance:
                return typeof(float);
            case GraphConfigurationKey.DefaultLightMaxVelocity:
                return typeof(float);
            case GraphConfigurationKey.DefaultLightStopVelocity:
                return typeof(float);
            case GraphConfigurationKey.DefaultDenseSpringForce:
                return typeof(float);
            case GraphConfigurationKey.DefaultDenseCoulombForce:
                return typeof(float);
            case GraphConfigurationKey.DefaultDenseDamping:
                return typeof(float);
            case GraphConfigurationKey.DefaultDenseSpringDistance:
                return typeof(float);
            case GraphConfigurationKey.DefaultDenseCoulombDistance:
                return typeof(float);
            case GraphConfigurationKey.DefaultDenseMaxVelocity:
                return typeof(float);
            case GraphConfigurationKey.DefaultDenseStopVelocity:
                return typeof(float);
            case GraphConfigurationKey.LensSimulationParameters:
                return typeof(float);
            case GraphConfigurationKey.LensTickDeltaTime:
                return typeof(float);
            case GraphConfigurationKey.LensMaxSimulationTime:
                return typeof(float);
            case GraphConfigurationKey.LensLerpSmooth:
                return typeof(float);
            case GraphConfigurationKey.LensLightSpringForce:
                return typeof(float);
            case GraphConfigurationKey.LensLightCoulombForce:
                return typeof(float);
            case GraphConfigurationKey.LensLightDamping:
                return typeof(float);
            case GraphConfigurationKey.LensLightSpringDistance:
                return typeof(float);
            case GraphConfigurationKey.LensLightCoulombDistance:
                return typeof(float);
            case GraphConfigurationKey.LensLightMaxVelocity:
                return typeof(float);
            case GraphConfigurationKey.LensLightStopVelocity:
                return typeof(float);
            case GraphConfigurationKey.LensDenseSpringForce:
                return typeof(float);
            case GraphConfigurationKey.LensDenseCoulombForce:
                return typeof(float);
            case GraphConfigurationKey.LensDenseDamping:
                return typeof(float);
            case GraphConfigurationKey.LensDenseSpringDistance:
                return typeof(float);
            case GraphConfigurationKey.LensDenseCoulombDistance:
                return typeof(float);
            case GraphConfigurationKey.LensDenseMaxVelocity:
                return typeof(float);
            case GraphConfigurationKey.LensDenseStopVelocity:
                return typeof(float);
            default:
                return typeof(float);
        }
    }


    public static string GetStringValue(this GraphConfigurationKey key, GraphConfiguration graphConfig)
    {
        switch (key)
        {
            case GraphConfigurationKey.SelectedMetricTypeSize:
                return graphConfig.SelectedMetricTypeSize.ToString();
            case GraphConfigurationKey.SelectedMetricTypeColor:
                return graphConfig.SelectedMetricTypeColor.ToString();
        }

        return "";
    }

    public static float GetFloatValue(this GraphConfigurationKey key, GraphConfiguration graphConfig)
    {
        switch (key)
        {
            case GraphConfigurationKey.ImmersionGraphSize:
                return graphConfig.ImmersionGraphSize;
            case GraphConfigurationKey.DeskGraphSize:
                return graphConfig.DeskGraphSize;
            case GraphConfigurationKey.WatchGraphSize:
                return graphConfig.WatchGraphSize;
            case GraphConfigurationKey.LensGraphSize:
                return graphConfig.LensGraphSize;
            case GraphConfigurationKey.NodeSizeImmersion:
                return graphConfig.NodeSizeImmersion;
            case GraphConfigurationKey.NodeSizeDesk:
                return graphConfig.NodeSizeDesk;
            case GraphConfigurationKey.NodeSizeWatch:
                return graphConfig.NodeSizeWatch;
            case GraphConfigurationKey.NodeSizeLens:
                return graphConfig.NodeSizeLens;
            case GraphConfigurationKey.NodeMinSizeImmersion:
                return graphConfig.NodeMinSizeImmersion;
            case GraphConfigurationKey.NodeMaxSizeImmersion:
                return graphConfig.NodeMaxSizeImmersion;
            case GraphConfigurationKey.NodeMinSizeDesk:
                return graphConfig.NodeMinSizeDesk;
            case GraphConfigurationKey.NodeMaxSizeDesk:
                return graphConfig.NodeMaxSizeDesk;
            case GraphConfigurationKey.NodeMinSizeLens:
                return graphConfig.NodeMinSizeLens;
            case GraphConfigurationKey.NodeMaxSizeLens:
                return graphConfig.NodeMaxSizeLens;
            case GraphConfigurationKey.LabelNodeSizeImmersion:
                return graphConfig.LabelNodeSizeImmersion;
            case GraphConfigurationKey.LabelNodeSizeDesk:
                return graphConfig.LabelNodeSizeDesk;
            case GraphConfigurationKey.LabelNodeSizeLens:
                return graphConfig.LabelNodeSizeLens;
            case GraphConfigurationKey.EdgeThicknessImmersion:
                return graphConfig.EdgeThicknessImmersion;
            case GraphConfigurationKey.EdgeThicknessDesk:
                return graphConfig.EdgeThicknessDesk;
            case GraphConfigurationKey.EdgeThicknessLens:
                return graphConfig.EdgeThicknessLens;
            case GraphConfigurationKey.EdgeThicknessWatch:
                return graphConfig.EdgeThicknessWatch;
            case GraphConfigurationKey.NodeColorMappingBoundaryColorA:
                return graphConfig.NodeColorMapping.BoundaryColorA;
            case GraphConfigurationKey.NodeColorMappingBoundaryColorB:
                return graphConfig.NodeColorMapping.BoundaryColorB;
            case GraphConfigurationKey.NodeColorMappingBoundaryColorC:
                return graphConfig.NodeColorMapping.BoundaryColorC;
            case GraphConfigurationKey.AlphaNodeColorPropagated:
                return graphConfig.AlphaNodeColorPropagated;
            case GraphConfigurationKey.AlphaNodeColorUnPropagated:
                return graphConfig.AlphaNodeColorUnPropagated;
            case GraphConfigurationKey.AlphaEdgeColorPropagated:
                return graphConfig.AlphaEdgeColorPropagated;
            case GraphConfigurationKey.AlphaEdgeColorUnPropagated:
                return graphConfig.AlphaEdgeColorUnPropagated;
            case GraphConfigurationKey.NbOntologyColor:
                return graphConfig.NbOntologyColor;
            case GraphConfigurationKey.MaxDeltaOntologyAlgo:
                return graphConfig.MaxDeltaOntologyAlgo;
            case GraphConfigurationKey.SaturationOntologyColor:
                return graphConfig.SaturationOntologyColor;
            case GraphConfigurationKey.ValueOntologyColor:
                return graphConfig.ValueOntologyColor;
            case GraphConfigurationKey.LabelNodgePropagation:
                return graphConfig.LabelNodgePropagation;
            case GraphConfigurationKey.SeedRandomPosition:
                return graphConfig.SeedRandomPosition;
            case GraphConfigurationKey.GraphModeTransitionTime:
                return graphConfig.GraphModeTransitionTime;
            case GraphConfigurationKey.DefaultTickDeltaTime:
                return graphConfig.SimuParameters.TickDeltaTime;
            case GraphConfigurationKey.DefaultMaxSimulationTime:
                return graphConfig.SimuParameters.MaxSimulationTime;
            case GraphConfigurationKey.DefaultLerpSmooth:
                return graphConfig.SimuParameters.LerpSmooth;
            case GraphConfigurationKey.DefaultLightSpringForce:
                return graphConfig.SimuParameters.LightSpringForce;
            case GraphConfigurationKey.DefaultLightCoulombForce:
                return graphConfig.SimuParameters.LightCoulombForce;
            case GraphConfigurationKey.DefaultLightDamping:
                return graphConfig.SimuParameters.LightDamping;
            case GraphConfigurationKey.DefaultLightSpringDistance:
                return graphConfig.SimuParameters.LightSpringDistance;
            case GraphConfigurationKey.DefaultLightCoulombDistance:
                return graphConfig.SimuParameters.LightCoulombDistance;
            case GraphConfigurationKey.DefaultLightMaxVelocity:
                return graphConfig.SimuParameters.LightMaxVelocity;
            case GraphConfigurationKey.DefaultLightStopVelocity:
                return graphConfig.SimuParameters.LightStopVelocity;
            case GraphConfigurationKey.DefaultDenseSpringForce:
                return graphConfig.SimuParameters.DenseSpringForce;
            case GraphConfigurationKey.DefaultDenseCoulombForce:
                return graphConfig.SimuParameters.DenseCoulombForce;
            case GraphConfigurationKey.DefaultDenseDamping:
                return graphConfig.SimuParameters.DenseDamping;
            case GraphConfigurationKey.DefaultDenseSpringDistance:
                return graphConfig.SimuParameters.DenseSpringDistance;
            case GraphConfigurationKey.DefaultDenseCoulombDistance:
                return graphConfig.SimuParameters.DenseCoulombDistance;
            case GraphConfigurationKey.DefaultDenseMaxVelocity:
                return graphConfig.SimuParameters.DenseMaxVelocity;
            case GraphConfigurationKey.DefaultDenseStopVelocity:
                return graphConfig.SimuParameters.DenseStopVelocity;
            case GraphConfigurationKey.LensTickDeltaTime:
                return graphConfig.LensSimuParameters.TickDeltaTime;
            case GraphConfigurationKey.LensMaxSimulationTime:
                return graphConfig.LensSimuParameters.MaxSimulationTime;
            case GraphConfigurationKey.LensLerpSmooth:
                return graphConfig.LensSimuParameters.LerpSmooth;
            case GraphConfigurationKey.LensLightSpringForce:
                return graphConfig.LensSimuParameters.LightSpringForce;
            case GraphConfigurationKey.LensLightCoulombForce:
                return graphConfig.LensSimuParameters.LightCoulombForce;
            case GraphConfigurationKey.LensLightDamping:
                return graphConfig.LensSimuParameters.LightDamping;
            case GraphConfigurationKey.LensLightSpringDistance:
                return graphConfig.LensSimuParameters.LightSpringDistance;
            case GraphConfigurationKey.LensLightCoulombDistance:
                return graphConfig.LensSimuParameters.LightCoulombDistance;
            case GraphConfigurationKey.LensLightMaxVelocity:
                return graphConfig.LensSimuParameters.LightMaxVelocity;
            case GraphConfigurationKey.LensLightStopVelocity:
                return graphConfig.LensSimuParameters.LightStopVelocity;
            case GraphConfigurationKey.LensDenseSpringForce:
                return graphConfig.LensSimuParameters.DenseSpringForce;
            case GraphConfigurationKey.LensDenseCoulombForce:
                return graphConfig.LensSimuParameters.DenseCoulombForce;
            case GraphConfigurationKey.LensDenseDamping:
                return graphConfig.LensSimuParameters.DenseDamping;
            case GraphConfigurationKey.LensDenseSpringDistance:
                return graphConfig.LensSimuParameters.DenseSpringDistance;
            case GraphConfigurationKey.LensDenseCoulombDistance:
                return graphConfig.LensSimuParameters.DenseCoulombDistance;
            case GraphConfigurationKey.LensDenseMaxVelocity:
                return graphConfig.LensSimuParameters.DenseMaxVelocity;
            case GraphConfigurationKey.LensDenseStopVelocity:
                return graphConfig.LensSimuParameters.DenseStopVelocity;
        }
        
        return 0f;
    }

    public static bool GetBoolValue(this GraphConfigurationKey key, GraphConfiguration graphConfig)
    {
        switch (key)
        {
            case GraphConfigurationKey.ShowLabelImmersion:
                return graphConfig.ShowLabelImmersion;
            case GraphConfigurationKey.ShowLabelDesk:
                return graphConfig.ShowLabelDesk;
            case GraphConfigurationKey.CanSelectEdges:
                return graphConfig.CanSelectEdges;
            case GraphConfigurationKey.DisplayEdges:
                return graphConfig.DisplayEdges;
            case GraphConfigurationKey.ResetPositionNodeOnUpdate:
                return graphConfig.ResetPositionNodeOnUpdate;
            case GraphConfigurationKey.DisplayInterSelectedNeighborEdges:
                return graphConfig.DisplayInterSelectedNeighborEdges;
            case GraphConfigurationKey.ShowWatch:
                return graphConfig.ShowWatch;
        }

        return false;
    }

    public static Color GetColorValue(this GraphConfigurationKey key, GraphConfiguration graphConfig)
    {
        switch (key)
        {
            case GraphConfigurationKey.NodeColor:
                return graphConfig.NodeColor;
            case GraphConfigurationKey.NodeColorNoValueMetric:
                return graphConfig.NodeColorNoValueMetric;
            case GraphConfigurationKey.EdgeColor:
                return graphConfig.EdgeColor;
            case GraphConfigurationKey.PropagatedEdgeColor:
                return graphConfig.PropagatedEdgeColor;
            case GraphConfigurationKey.NodeColorMappingColorA:
                return graphConfig.NodeColorMapping.ColorA;
            case GraphConfigurationKey.NodeColorMappingColorB:
                return graphConfig.NodeColorMapping.ColorB;
            case GraphConfigurationKey.NodeColorMappingColorC:
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
