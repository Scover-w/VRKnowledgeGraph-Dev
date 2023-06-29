using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphConfig", menuName = "ScriptableObjects/GraphConfigurationContainerSO", order = 1)]
public class GraphConfigurationContainerSO : ScriptableObject
{
    [SerializeField]
    [Space(20)]
    public GraphConfiguration _graphConfiguration;

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
    public Color NodeColorNoOntology = Color.white;

    [Header("Viewer Ontology Color")]
    [SerializeField]
    private List<Color> OntologyViewerColors;

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
        _graphConfiguration.PropagatedEdgeColor = PropagatedEdgeColor;


        _graphConfiguration.NodeColor = NodeColor;

        _graphConfiguration.NodeColorMapping.ColorA = NodeColorA;
        _graphConfiguration.NodeColorMapping.ColorB = NodeColorB;
        _graphConfiguration.NodeColorMapping.ColorC = NodeColorC;

        _graphConfiguration.NodeColorNoOntology = NodeColorNoOntology;


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
