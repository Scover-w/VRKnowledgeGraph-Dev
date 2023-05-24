using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphConfig", menuName = "ScriptableObjects/GraphConfigurationContainerSO", order = 1)]
public class GraphConfigurationContainerSO : ScriptableObject
{
    public GraphConfiguration GraphConfiguration 
    { 
        get
        {
            if(_graphConfiguration == null)
                _graphConfiguration = GraphConfiguration.Load();

            return _graphConfiguration;
        }
    }


    [SerializeField]
    GraphConfiguration _graphConfiguration;

    private void Awake()
    {
        _graphConfiguration = GraphConfiguration.Load();
    }


    private void OnValidate()
    {
        _graphConfiguration.Save();
    }
}
