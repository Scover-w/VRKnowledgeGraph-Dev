using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class GraphConfiguration
{
    [Header("Simulation Parameters")]

    public SimulationParameters SimuParameters = new(false);
    public SimulationParameters LensSimuParameters = new(true);


    [Space(15)]
    [Header("/Styling/")]
    [Space(5)]
    [Header("Graph Size")]
    public float ImmersionGraphSize = 12.75f;
    public float DeskGraphSize = .1f;
    [Space(5)]
    public float WatchGraphSize = .025f;
    public float LensGraphSize = 1.74f;


    [Space(10)]
    [Header("Node Size")]
    public float NodeSizeImmersion = 1f;
    public float NodeSizeDesk = .03f;

    [Space(5)]
    public float NodeSizeWatch = .006f;
    public float NodeSizeLens = .2f;

    [Space(5)]
    public float NodeMinSizeImmersion = 1f;
    public float NodeMaxSizeImmersion = .6f;

    [Space(5)]
    public float NodeMinSizeDesk = .03f;
    public float NodeMaxSizeDesk = .01f;

    [Space(5)]
    public float NodeMinSizeLens = .2f;
    public float NodeMaxSizeLens = .1f;

    [Space(10)]
    [Header("Label")]
    public float LabelNodeSizeImmersion = 5f;
    public float LabelNodeSizeDesk = .1f;

    public float LabelNodeSizeLens = 1.36f;

    public bool ShowLabelImmersion = true;
    public bool ShowLabelDesk = false;

    public bool ShowLabelLens = true;


    [Space(10)]
    [Header("Edge")]
    public float EdgeThicknessImmersion = .05f;
    public float EdgeThicknessDesk = .001f;
    public float EdgeThicknessLens = .005f;
    public float EdgeThicknessWatch = .0002f;
    [Space(5)]
    public bool CanSelectEdges = false;
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
    public Color NodeColorNoValueMetric
    {
        get
        {
            return _nodeColorNoValueMetric.ToUnityColor();
        }
        set
        {
            _nodeColorNoValueMetric = value.ToSystemColor();
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
    private System.Drawing.Color _nodeColor = System.Drawing.Color.FromArgb(255, 255, 255);

    public ColorLerpMapper NodeColorMapping = new();

    
    [JsonProperty("NodeColorNoOntology_")]
    private System.Drawing.Color _nodeColorNoValueMetric = System.Drawing.Color.FromArgb(178, 178, 178);

    [JsonProperty("EdgeColor_")]
    [SerializeField]
    private System.Drawing.Color _edgeColor = System.Drawing.Color.FromArgb(255, 255, 255);

    [JsonProperty("PropagatedEdgeColor_")]
    private System.Drawing.Color _propagatedEdgeColor = System.Drawing.Color.FromArgb(0, 56, 255);

    [Header("/Ontology/")]
    [Range(1, 15)]
    public int NbOntologyColor = 7;
    [Range(0, 15)]
    public int MaxDeltaOntologyAlgo = 1;
    [Range(0f, 1f)]
    public float SaturationOntologyColor = .772f;
    [Range(0f, 1f)]
    public float ValueOntologyColor = .767f;


    [Space(30)]
    [Header("/Miscelaneous/")]
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
        var folderPath = Path.Combine(Application.persistentDataPath, "Data");

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
    LocalClusteringCoefficient,
    Degree,
    Ontology
}


// TODO: InterpolationType enum
// Sigmoid, etc...
