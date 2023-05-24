using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphConfig", menuName = "ScriptableObjects/GraphConfigurationContainerSO", order = 1)]
public class GraphConfigurationContainerSO : ScriptableObject
{
    
    [Tooltip("Goes in the graph configuration. It's just that UnityEngine.Color can't be Serialized.")]
    [Header("Colors graph")]
    [Space(5)]
    public Color NodeColor = Color.white;

    public Color NodeMappingAColor = Color.white;

    public Color NodeMappingBColor = Color.white;

    public Color EdgeColor = Color.white;

    public Color EdgeMappingAColor = Color.white;

    public Color EdgeMappingBColor = Color.white;

    

    [SerializeField]
    [Space(20)]
    GraphConfiguration _graphConfiguration;

    private async void Awake()
    {
        _graphConfiguration = await GraphConfiguration.Load();
    }

    public async void Save()
    {
        if (_graphConfiguration == null) return;

        await Task.Run(async () =>
        {
            await _graphConfiguration.Save();
        });
    }


    public async Task<GraphConfiguration >GetGraphConfiguration()
    {
        if (_graphConfiguration == null)
            _graphConfiguration = await GraphConfiguration.Load();

        return _graphConfiguration;
    }


    private async void OnValidate()
    {
        _graphConfiguration.NodeColor = NodeColor;  
        _graphConfiguration.NodeMappingAColor = NodeMappingAColor;
        _graphConfiguration.NodeMappingBColor = NodeMappingBColor;
        _graphConfiguration.EdgeColor = EdgeColor;
        _graphConfiguration.EdgeMappingAColor = EdgeMappingAColor;
        _graphConfiguration.EdgeMappingBColor = EdgeMappingBColor;

        await _graphConfiguration.Save();
    }
}
