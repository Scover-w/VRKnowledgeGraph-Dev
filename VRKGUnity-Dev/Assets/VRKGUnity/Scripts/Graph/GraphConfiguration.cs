using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GraphConfiguration
{
    [Header("Simulation Parameters")]
    [Space(5)]
    [Header("Light Graph")]
    public float LightSpringForce = 5f;
    public float LightCoulombForce = .1f;
    public float LightDamping = 1f;
    public float LightSpringDistance = 1f;
    public float LightCoulombDistance = 2f;
    public float LightMaxVelocity = 10f;
    public float LightStopVelocity = .19f;

    [Header("Dense Graph")]
    public float DenseSpringForce = 5f;
    public float DenseCoulombForce = .1f;
    public float DenseDamping = 1f;
    public float DenseSpringDistance = 15f;
    public float DenseCoulombDistance = 30f;
    public float DenseMaxVelocity = 10f;
    public float DenseStopVelocity = 2f;

    [Space(30)]
    [Header("Styling")]
    [Space(5)]
    [Header("Graph Size")]
    public float MiniGraphSize = 1f;
    public float MegaGraphSize = 1f;


    [Space(5)]
    [Header("Node Size")]
    public float NodeSizeMiniGraph = 1f;
    public float NodeSizeMegaGraph = 1f;

    public float NodeMinSizeMiniGraph = .8f;
    public float NodeMaxSizeMiniGraph = .8f;

    public float NodeMinSizeMegaGraph = .8f;
    public float NodeMaxSizeMegaGraph = .8f;

    [Space(5)]
    [Header("Label Size")]
    public float LabelSizeMiniGraph = 1f;
    public float LabelSizeMegaGraph = 1f;


    [Space(5)]
    [Header("Edge Thickness")]
    public float EdgeThicknessMiniGraph = 1f;
    public float EdgeThicknessMegaGraph = 1f;


    [JsonIgnore]
    public Color NodeColor
    {
        get
        {
            return _nodeColor.ToUnityColor();
        }
        set
        {
            _nodeColor = value.ToSystemColor();
        }
    }

    [JsonIgnore]
    public Color NodeColorNoOntology
    {
        get
        {
            return _nodeColorNoOntology.ToUnityColor();
        }
        set
        {
            _nodeColorNoOntology = value.ToSystemColor();
        }
    }

    [JsonIgnore]
    public Color EdgeColor
    {
        get
        {
            return _edgeColor.ToUnityColor();
        }
        set
        {
            _edgeColor = value.ToSystemColor();
        }
    }



    [JsonProperty("NodeColor_")]
    private System.Drawing.Color _nodeColor = System.Drawing.Color.White;

    public ColorLerpMapper NodeColorMapping;

    
    [JsonProperty("NodeColorNoOntology_")]
    private System.Drawing.Color _nodeColorNoOntology = System.Drawing.Color.White;

    [JsonProperty("EdgeColor_")]
    [SerializeField]
    private System.Drawing.Color _edgeColor = System.Drawing.Color.White;

    [Header("Ontology Color")]
    [Range(1, 15)]
    public int NbOntologyColor;
    [Range(0, 15)]
    public int MaxDeltaOntologyAlgo;
    [Range(0f, 1f)]
    public float SaturationOntologyColor;
    [Range(0f, 1f)]
    public float ValueOntologyColor;


    [Space(30)]
    [Header("Miscelaneous")]
    [Space(5)]
    public int LabelNodgePropagation = 1;

    public int SeedRandomPosition = 0;
    public bool ResetPositionNodeOnUpdate = true;
    public bool CanSelectEdges = true;

    public GraphMetricType SelectedMetricTypeSize = GraphMetricType.None;
    public GraphMetricType SelectedMetricTypeColor = GraphMetricType.None;


    private static string _graphConfigPath;


    public async static Task<GraphConfiguration> Load()
    {
        SetPath();

        if (File.Exists(_graphConfigPath))
        {
            string json = await File.ReadAllTextAsync(_graphConfigPath);
            var graphConfig = JsonConvert.DeserializeObject<GraphConfiguration>(json);
            return graphConfig;
        }
        
        
        var graphConfigB = new GraphConfiguration();
        await graphConfigB.Save();
        return graphConfigB;
    }

    public async Task Save()
    {
        SetPath();

        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        await File.WriteAllTextAsync(_graphConfigPath, json);
    }


    private static void SetPath()
    {
        var folderPath = Path.Combine(Application.dataPath, "VRKGUnity", "Data");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        _graphConfigPath = Path.Combine(folderPath, "GraphConfiguration.json");
    }

}

[Serializable]
public enum GraphMetricType
{
    None,
    AverageShortestPath,
    BetweennessCentrality,
    ClosenessCentrality,
    ClusteringCoefficient,
    Degree,
    Ontology
}


// TODO: InterpolationType enum
// Sigmoid, etc...
