using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GraphConfigKey
{
    GraphMode,
    SelectionMode,

    SimuParameters,
    LensSimuParameters,

    ImmersionGraphSize,
    DeskGraphSize,

    WatchGraphSize,
    LensGraphSize,

    NodeSizeImmersion,
    NodeSizeDesk,

    NodeSizeWatch,
    NodeSizeLens,

    NodeMinMaxSizeImmersion,
    NodeMinMaxSizeDesk,
    NodeMinMaxSizeLens,

    LabelNodeSizeImmersion,
    LabelNodeSizeDesk,

    LabelNodeSizeLens,

    ShowLabelImmersion,
    ShowLabelDesk,

    ShowLabelLens,

    EdgeThicknessImmersion,
    EdgeThicknessDesk,
    EdgeThicknessLens,
    EdgeThicknessWatch,

    CanSelectEdges,
    DisplayEdges,

    NodeColor,
    NodeColorNoValueMetric,
    EdgeColor,
    PropagatedEdgeColor,

    NodeColorMapping,
    NodeColorMappingColorA,
    NodeColorMappingColorB,
    NodeColorMappingColorC,

    NodeColorMappingBoundaryColorA,
    NodeColorMappingBoundaryColorB,
    NodeColorMappingBoundaryColorC,


    AlphaNodeColorPropagated,
    AlphaNodeColorUnPropagated,


    AlphaEdgeColorPropagated,
    AlphaEdgeColorUnPropagated,


    NbOntologyColor,
    MaxDeltaOntologyAlgo,
    SaturationOntologyColor,
    ValueOntologyColor,

    LabelNodgePropagation,
    ResetPositionNodeOnUpdate,
    SeedRandomPosition,
    GraphModeTransitionTime,
    DisplayInterSelectedNeighborEdges,

    ShowWatch,

    SelectedMetricTypeSize,
    SelectedMetricTypeColor,

    // Default SimulationParameters
    DefaultSimulationParameters,

    DefaultTickDeltaTime,
    DefaultMaxSimulationTime,
    DefaultLerpSmooth,

    DefaultLightSpringForce,
    DefaultLightCoulombForce,
    DefaultLightDamping,
    DefaultLightSpringDistance,
    DefaultLightCoulombDistance,
    DefaultLightMaxVelocity,
    DefaultLightStopVelocity,

    DefaultDenseSpringForce,
    DefaultDenseCoulombForce,
    DefaultDenseDamping,
    DefaultDenseSpringDistance,
    DefaultDenseCoulombDistance,
    DefaultDenseMaxVelocity,
    DefaultDenseStopVelocity,

    // Lens SimulationParameters
    LensSimulationParameters,

    LensTickDeltaTime,
    LensMaxSimulationTime,
    LensLerpSmooth,

    LensLightSpringForce,
    LensLightCoulombForce,
    LensLightDamping,
    LensLightSpringDistance,
    LensLightCoulombDistance,
    LensLightMaxVelocity,
    LensLightStopVelocity,

    LensDenseSpringForce,
    LensDenseCoulombForce,
    LensDenseDamping,
    LensDenseSpringDistance,
    LensDenseCoulombDistance,
    LensDenseMaxVelocity,
    LensDenseStopVelocity
}