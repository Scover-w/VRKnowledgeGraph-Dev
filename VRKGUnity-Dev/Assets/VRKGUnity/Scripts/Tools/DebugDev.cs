using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDev : MonoBehaviour
{
    static bool _debug = true;


    public static void Log(object message)
    {
        if (!_debug)
            return;

        Debug.Log(message);
    }

    public static void LogWarning(object message)
    {
        if (!_debug)
            return;

        Debug.LogWarning(message);
    }

    public static void LogError(object message)
    {
        if (!_debug)
            return;

        Debug.LogError(message);
    }

}
