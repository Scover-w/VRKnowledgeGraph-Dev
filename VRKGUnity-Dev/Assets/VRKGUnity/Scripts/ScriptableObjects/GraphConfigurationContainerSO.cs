using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphConfig", menuName = "ScriptableObjects/GraphConfigurationContainerSO", order = 1)]
public class GraphConfigurationContainerSO : ScriptableObject
{
    [SerializeField]
    [Space(20)]
    public GraphConfiguration _graphConfiguration;

    private async void Awake()
    {
        Debug.LogWarning("so tamere awake");
        _graphConfiguration = await GraphConfiguration.Load();
    }

    private void OnEnable()
    {
        Debug.LogWarning("so tamere OnEnable");

        ForceLoad();
    }

    [ContextMenu("ForceLoad")]
    public async Task ForceLoad()
    {
        Debug.LogWarning("so tamere ForceLoad");

        try
        {
            _graphConfiguration = await GraphConfiguration.Load();
            Debug.Log(_graphConfiguration.NodeColorMapping.BoundaryColorA);
        }
        catch(Exception ex) 
        {
            Debug.LogError(ex);
        }
    }

    public async void Save()
    {
        if (_graphConfiguration == null) return;

        await Task.Run(async () =>
        {
            await _graphConfiguration.Save();
        });
    }


    public async Task<GraphConfiguration> GetGraphConfiguration()
    {
        Debug.LogWarning("so tamere GetGraphConfiguration");

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            _graphConfiguration = await GraphConfiguration.Load();
            return _graphConfiguration;
        }
#endif



        if (_graphConfiguration == null)
            _graphConfiguration = await GraphConfiguration.Load();

        return _graphConfiguration;
    }


    private async void OnValidate()
    {
        await _graphConfiguration.Save();
    }
}
