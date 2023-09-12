using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class SimulationParameters
{
    public float TickDeltaTime = 0.016f;
    public float MaxSimulationTime = 15f;
    public float LerpSmooth = .1f;

    public float SpringForce = 5f;
    public float CoulombForce = .1f;
    public float Damping = 1f;
    public float SpringDistance = 1f;
    public float CoulombDistance = 2f;
    public float MaxVelocity = 10f;
    public float StopVelocity = .19f;

    public SimulationParameters(bool isForLens)
    {
        if (isForLens)
            LerpSmooth = .1f;
    }
}
