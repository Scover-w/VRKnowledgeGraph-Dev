using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GraphConfiguration
{
    [Header("Simulation Parameters")]

    public float TickDeltaTime = 0.016f;
    public float MaxSimulationTime = 15f;

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
    [Space(10)]
    [Header("Graph Size")]
    public float ImmersionGraphSize = 1f;
    public float DeskGraphSize = 1f;
    [Space(5)]
    public float WatchGraphSize = 1f;
    public float LensGraphSize = 1f;


    [Space(10)]
    [Header("Node Size")]
    public float NodeSizeImmersion = 1f;
    public float NodeSizeDesk = 1f;

    [Space(5)]
    public float NodeSizeWatch = 1f;
    public float NodeSizeLens = 1f;

    [Space(5)]
    public float NodeMinSizeImmersion = .8f;
    public float NodeMaxSizeImmersion = .8f;

    [Space(5)]
    public float NodeMinSizeDesk = .8f;
    public float NodeMaxSizeDesk = .8f;

    [Space(5)]
    public float NodeMinSizeLens = .8f;
    public float NodeMaxSizeLens = .8f;

    [Space(10)]
    [Header("Label")]
    public float LabelNodeSizeImmersion = 1f;
    public float LabelNodeSizeDesk = 1f;

    public float LabelNodeSizeLens = 1f;

    public bool ShowLabelImmersion = true;
    public bool ShowLabelDesk = true;

    public bool ShowLabelLens = true;


    [Space(10)]
    [Header("Edge")]
    public float EdgeThicknessImmersion = 1f;
    public float EdgeThicknessDesk = 1f;
    public float EdgeThicknessLens = 1f;
    public float EdgeThicknessWatch = 1f;
    [Space(5)]
    public bool CanSelectEdges = true;
    public bool DisplayEdges = true;


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

    [JsonIgnore]
    public Color PropagatedEdgeColor
    {
        get
        {
            return _propagatedEdgeColor.ToUnityColor();
        }
        set
        {
            _propagatedEdgeColor = value.ToSystemColor();
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

    [JsonProperty("PropagatedEdgeColor_")]
    private System.Drawing.Color _propagatedEdgeColor = System.Drawing.Color.White;

    [Header("Ontology")]
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
    public bool ResetPositionNodeOnUpdate = true;
    public int SeedRandomPosition = 0;
    public float GraphModeTransitionTime = 1f;

    public bool ShowWatch = true;

    public GraphMetricType SelectedMetricTypeSize = GraphMetricType.None;
    public GraphMetricType SelectedMetricTypeColor = GraphMetricType.None;


    private static string _graphConfigPath;

    public static GraphConfiguration Instance;

    public async static Task<GraphConfiguration> Load()
    {
        SetPath();

        if (File.Exists(_graphConfigPath))
        {
            string json = await File.ReadAllTextAsync(_graphConfigPath);
            var graphConfig = JsonConvert.DeserializeObject<GraphConfiguration>(json);
            Instance = graphConfig;
            return graphConfig;
        }
        
        
        var graphConfigB = new GraphConfiguration();
        Instance = graphConfigB;

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
