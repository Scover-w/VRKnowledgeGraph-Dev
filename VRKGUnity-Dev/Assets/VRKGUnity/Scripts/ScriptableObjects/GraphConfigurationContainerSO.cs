using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphConfig", menuName = "ScriptableObjects/GraphConfigurationContainerSO", order = 1)]
public class GraphConfigurationContainerSO : ScriptableObject
{
    [SerializeField]
    [Space(20)]
    GraphConfiguration _graphConfiguration;

    [Space(20)]
    [Header("Colors")]
    public Color NodeColor = Color.white;
    public Color EdgeColor = Color.white;
    public Color PropagatedEdgeColor = Color.white;

    [Space(10)]
    public Color NodeColorA = Color.white;
    public Color NodeColorB = Color.white;
    public Color NodeColorC = Color.white;

    [Space(10)]
    public Color NodeColorNoValueMetric = Color.white;

    [Header("Viewer Ontology Color")]
    [SerializeField]
    private List<Color> OntologyViewerColors;

    //async void Awake()
    //{
    //    _graphConfiguration = await GraphConfiguration.Load();
    //}

    async void OnEnable()
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


    public async Task<GraphConfiguration> GetGraphConfiguration()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            _graphConfiguration = await GraphConfiguration.Load();
            return _graphConfiguration;
        }
#endif

        _graphConfiguration ??= await GraphConfiguration.Load();

        return _graphConfiguration;
    }



    public async void RefreshWindowsEditor()
    {
#if !UNITY_EDITOR
        return;
#endif

        _graphConfiguration = await GraphConfiguration.Load();
        NodeColor = _graphConfiguration.NodeColor;
        EdgeColor = _graphConfiguration.EdgeColor;
        PropagatedEdgeColor = _graphConfiguration.PropagatedEdgeColor;

        NodeColorA = _graphConfiguration.NodeColorMapping.ColorA;
        NodeColorB = _graphConfiguration.NodeColorMapping.ColorB;
        NodeColorC = _graphConfiguration.NodeColorMapping.ColorC;

        NodeColorNoValueMetric = _graphConfiguration.NodeColorNoValueMetric;
}


    private async void OnValidate()
    {

        if (Application.isPlaying)
        {
            Debug.LogWarning("Tried to save ConfigSo while in play mode");
            return;
        }


        if (_graphConfiguration.ImmersionGraphSize == 1f)
            Debug.LogWarning("Stop");

        _graphConfiguration.EdgeColor = EdgeColor;
        _graphConfiguration.PropagatedEdgeColor = PropagatedEdgeColor;


        _graphConfiguration.NodeColor = NodeColor;

        _graphConfiguration.NodeColorMapping.ColorA = NodeColorA;
        _graphConfiguration.NodeColorMapping.ColorB = NodeColorB;
        _graphConfiguration.NodeColorMapping.ColorC = NodeColorC;

        _graphConfiguration.NodeColorNoValueMetric = NodeColorNoValueMetric;


        OntologyViewerColors = new List<Color>();

        int nbColor = _graphConfiguration.NbOntologyColor;

        float deltaHue = 1f / nbColor;


        for (int i = 0; i < nbColor; i++)
        {
            OntologyViewerColors.Add(Color.HSVToRGB((deltaHue * i) % 1f, _graphConfiguration.SaturationOntologyColor, _graphConfiguration.ValueOntologyColor));
        }

        await _graphConfiguration.Save();
    }
}
