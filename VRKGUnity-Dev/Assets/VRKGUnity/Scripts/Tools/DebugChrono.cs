using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugChrono
{
    public static DebugChrono Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new DebugChrono();
            }
            
            return _instance;
        }
    }

    public static DebugChrono _instance;


    Dictionary<string, DateTime> _chronos;


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

        _chronos.Add(keyChrono, DateTime.Now);
    }

    public void Stop(string keyChrono) 
    { 
        if(!_chronos.TryGetValue(keyChrono,out DateTime start)) 
        {
            Debug.LogWarning("[DebugChrono] Stop() keyChrono " + keyChrono + " not in.");
            return;
        }


        _chronos.Remove(keyChrono);

        var secondsSpan = (DateTime.Now - start).TotalMilliseconds;
        Debug.Log(keyChrono + " has lasted " + secondsSpan + " ms.");
    }

    public float Stop(string keyChrono, bool displayLog)
    {
        if (!_chronos.TryGetValue(keyChrono, out DateTime start))
        {
            Debug.LogWarning("[DebugChrono] Stop() keyChrono " + keyChrono + " not in.");
            return 0f;
        }


        _chronos.Remove(keyChrono);

        var secondsSpan = (float)(DateTime.Now - start).TotalMilliseconds;

        if(displayLog)
            Debug.Log(keyChrono + " has lasted " + secondsSpan + " ms.");

        return (float)(DateTime.Now - start).TotalSeconds;
    }


}
