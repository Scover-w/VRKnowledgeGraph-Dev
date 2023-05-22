using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GraphConfiguration
{
    [Header("Small Graph")]
    public float SpringForce = 5f;
    public float CoulombForce = .1f;
    public float Damping = 1f;
    public float SpringDistance = 1f;
    public float CoulombDistance = 2f;
    public float MaxVelocity = 10f;
    public float StopVelocity = .19f;

    [Header("Big Graph")]
    public float BigSpringForce = 5f;
    public float BigCoulombForce = .1f;
    public float BigDamping = 1f;
    public float BigSpringDistance = 15f;
    public float BigCoulombDistance = 30f;
    public float BigMaxVelocity = 10f;
    public float BigStopVelocity = 2f;


    public int LabelNodgePropagation = 1;

    public int SeedRandomPosition = 0;
    public bool ResetPositionNodeOnUpdate = true;
}
