using System;
using System.Collections;
using System.Collections.Generic;
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
}
