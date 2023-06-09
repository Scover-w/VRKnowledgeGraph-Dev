using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphConfig", menuName = "ScriptableObjects/GraphConfigurationContainerSO", order = 1)]
public class GraphConfigurationContainerSO : ScriptableObject
{
    [SerializeField]
    [Space(20)]
    public GraphConfiguration _graphConfiguration;

    public Color NodeColor = Color.white;
    public Color EdgeColor = Color.white;

    public Color NodeColorA = Color.white;
    public Color NodeColorB = Color.white;
    public Color NodeColorC = Color.white;

    private async void Awake()
    {
        _graphConfiguration = await GraphConfiguration.Load();
    }

    private void OnEnable()
    {
        ForceLoad();
    }


    [ContextMenu("ForceLoad")]
    public async Task ForceLoad()
    {
        Debug.LogWarning("ForceLoad");

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
        Debug.LogWarning("GetGraphConfiguration");

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
        _graphConfiguration.EdgeColor = EdgeColor;
        _graphConfiguration.NodeColor = NodeColor;

        _graphConfiguration.NodeColorMapping.ColorA = NodeColorA;
        _graphConfiguration.NodeColorMapping.ColorB = NodeColorB;
        _graphConfiguration.NodeColorMapping.ColorC = NodeColorC;
        await _graphConfiguration.Save();
    }
}
