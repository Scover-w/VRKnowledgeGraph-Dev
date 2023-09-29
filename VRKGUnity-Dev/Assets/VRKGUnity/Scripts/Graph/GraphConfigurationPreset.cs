using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows to store start presets for some <see cref="GraphConfiguration"/> properties that need to be readable values in the UI (like 1 and not 0.0001) 
/// </summary>
public class GraphConfigurationPreset
{
    public const float ImmersionGraphSize = 12.75f;
    public const float DeskGraphSize = .1f;

    public const float GPSGraphSize = .025f;
    public const float LensGraphSize = 1.74f;



    public const float NodeSizeImmersion = 1f;
    public const float NodeSizeDesk = .03f;


    public const float NodeSizeGPS = .006f;
    public const float NodeSizeLens = .2f;


    public const float LabelNodeSizeImmersion = 5f;
    public const float LabelNodeSizeDesk = .45f;

    public const float LabelNodeSizeLens = 1.36f;


    public const float EdgeThicknessImmersion = .015f;
    public const float EdgeThicknessDesk = .001f;
    public const float EdgeThicknessLens = .005f;
    public const float EdgeThicknessGPS = .0002f;
}
