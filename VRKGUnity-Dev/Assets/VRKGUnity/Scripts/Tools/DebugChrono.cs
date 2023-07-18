using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DebugChrono
{
    public static DebugChrono Instance
    {
        get
        {
            _instance ??= new DebugChrono();
            
            return _instance;
        }
    }

    public static DebugChrono _instance;

    readonly Dictionary<string, Stopwatch> _chronos;


    public DebugChrono()
    {
        _chronos = new();
    }


    public void Start(string keyChrono)
    {
        if(_chronos.ContainsKey(keyChrono)) 
        {
            Debug.LogWarning("[DebugChrono] Start() keyChrono " + keyChrono + " already used.");
            _chronos.Remove(keyChrono);
        }

        Stopwatch stopwatch = new();
        stopwatch.Start();
        _chronos.Add(keyChrono, stopwatch);
    }

    public void Stop(string keyChrono) 
    { 
        if(!_chronos.TryGetValue(keyChrono,out Stopwatch stopwatch)) 
        {
            Debug.LogWarning("[DebugChrono] Stop() keyChrono " + keyChrono + " not in.");
            return;
        }

        stopwatch.Stop();
        _chronos.Remove(keyChrono);


        TimeSpan ts = stopwatch.Elapsed;
        var msSpan = ts.TotalMilliseconds;
        Debug.Log(keyChrono + " has lasted " + msSpan + " ms.");
    }

    public float Stop(string keyChrono, bool displayLog)
    {
        if (!_chronos.TryGetValue(keyChrono, out Stopwatch stopwatch))
        {
            Debug.LogWarning("[DebugChrono] Stop() keyChrono " + keyChrono + " not in.");
            return 0f;
        }

        stopwatch.Stop();
        _chronos.Remove(keyChrono);

        TimeSpan ts = stopwatch.Elapsed;
        var msSpan = ts.TotalMilliseconds;

        if (displayLog)
            Debug.Log(keyChrono + " has lasted " + msSpan + " ms.");

        return (float)ts.TotalSeconds;
    }


}
