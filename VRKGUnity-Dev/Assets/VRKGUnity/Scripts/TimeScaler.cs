using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    [Range(0,1)]
    public float TimeScale = 1.0f;


    private void OnValidate()
    {
        Time.timeScale = TimeScale;
    }
}
