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

    GPSGraphSize,
    LensGraphSize,

    NodeSizeImmersion,
    NodeSizeDesk,

    NodeSizeGPS,
    NodeSizeLens,

    NodeMinMaxSizeImmersion,
    NodeMinMaxSizeDesk,
    NodeMinMaxSizeLens,

    LabelNodeSizeImmersion,
    LabelNodeSizeDesk,

    LabelNodeSizeLens,

    DisplayLabelImmersion,
    DisplayLabelDesk,

    DisplayLabelLens,

    EdgeThicknessImmersion,
    EdgeThicknessDesk,
    EdgeThicknessLens,
    EdgeThicknessGPS,

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

    GlobalVolume,
    SoundEffectVolume,
    MusicVolume,
    AidenVolume,

    LabelNodgePropagation,
    ResetPositionNodeOnUpdate,
    SeedRandomPosition,
    GraphModeTransitionTime,
    DisplayInterSelectedNeighborEdges,

    DisplayGPS,

    SelectedMetricTypeSize,
    SelectedMetricTypeColor,

    // Default SimulationParameters
    DefaultSimulationParameters,

    DefaultTickDeltaTime,
    DefaultMaxSimulationTime,
    DefaultLerpSmooth,

    DefaultSpringForce,
    DefaultCoulombForce,
    DefaultDamping,
    DefaultSpringDistance,
    DefaultCoulombDistance,
    DefaultMaxVelocity,
    DefaultStopVelocity,

    // Lens SimulationParameters
    LensSimulationParameters,

    LensTickDeltaTime,
    LensMaxSimulationTime,
    LensLerpSmooth,

    LensSpringForce,
    LensCoulombForce,
    LensDamping,
    LensSpringDistance,
    LensCoulombDistance,
    LensMaxVelocity,
    LensStopVelocity,
}