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

    [Range(0f, 1f)]
    public float AlphaNodeColorPropagated = 1f;
    [Range(0f, 1f)]
    public float AlphaNodeColorUnPropagated = 1f;


    [JsonProperty("NodeColorNoOntology_")]
    private System.Drawing.Color _nodeColorNoValueMetric = System.Drawing.Color.FromArgb(178, 178, 178);

    [JsonProperty("EdgeColor_")]
    [SerializeField]
    private System.Drawing.Color _edgeColor = System.Drawing.Color.FromArgb(255, 255, 255);

    [Range(0f, 1f)]
    public float AlphaEdgeColorPropagated = 1f;
    [Range(0f, 1f)]
    public float AlphaEdgeColorUnPropagated = 1f;

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
    public int LabelNodgePropagation = 1;//{ get; private set; } = 1;
    public bool ResetPositionNodeOnUpdate = true;
    public int SeedRandomPosition = 0;
    public float GraphModeTransitionTime = 1f;
    public bool DisplayInterSelectedNeighborEdges = false;

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


    public bool SetValue(GraphConfigurationKey key, string value)
    {
        switch (key)
        {
            case GraphConfigurationKey.SelectedMetricTypeSize:
                SelectedMetricTypeSize = GraphConfigurationTools.StringToEnum(value);
                return true;
            case GraphConfigurationKey.SelectedMetricTypeColor:
                SelectedMetricTypeColor = GraphConfigurationTools.StringToEnum(value);
                return true;
        }

        return false;
    }

    public bool SetValue(GraphConfigurationKey key, float value)
    {
        switch (key)
        {
            case GraphConfigurationKey.ImmersionGraphSize:
                ImmersionGraphSize = value;
                return true;
            case GraphConfigurationKey.DeskGraphSize:
                DeskGraphSize = value;
                return true;
            case GraphConfigurationKey.WatchGraphSize:
                WatchGraphSize = value;
                return true;
            case GraphConfigurationKey.LensGraphSize:
                LensGraphSize = value;
                return true;
            case GraphConfigurationKey.NodeSizeImmersion:
                NodeSizeImmersion = value;
                return true;
            case GraphConfigurationKey.NodeSizeDesk:
                NodeSizeDesk = value;
                return true;
            case GraphConfigurationKey.NodeSizeWatch:
                NodeSizeWatch = value;
                return true;
            case GraphConfigurationKey.NodeSizeLens:
                NodeSizeLens = value;
                return true;
            case GraphConfigurationKey.NodeMinSizeImmersion:
                NodeMinSizeImmersion = value;
                return true;
            case GraphConfigurationKey.NodeMaxSizeImmersion:
                NodeMaxSizeImmersion = value;
                return true;
            case GraphConfigurationKey.NodeMinSizeDesk:
                NodeMinSizeDesk = value;
                return true;
            case GraphConfigurationKey.NodeMaxSizeDesk:
                NodeMaxSizeDesk = value;
                return true;
            case GraphConfigurationKey.NodeMinSizeLens:
                NodeMinSizeLens = value;
                return true;
            case GraphConfigurationKey.NodeMaxSizeLens:
                NodeMaxSizeLens = value;
                return true;
            case GraphConfigurationKey.LabelNodeSizeImmersion:
                LabelNodeSizeImmersion = value;
                return true;
            case GraphConfigurationKey.LabelNodeSizeDesk:
                LabelNodeSizeDesk = value;
                return true;
            case GraphConfigurationKey.LabelNodeSizeLens:
                LabelNodeSizeLens = value;
                return true;
            case GraphConfigurationKey.EdgeThicknessImmersion:
                EdgeThicknessImmersion = value;
                return true;
            case GraphConfigurationKey.EdgeThicknessDesk:
                EdgeThicknessDesk = value;
                return true;
            case GraphConfigurationKey.EdgeThicknessLens:
                EdgeThicknessLens = value;
                return true;
            case GraphConfigurationKey.EdgeThicknessWatch:
                EdgeThicknessWatch = value;
                return true;
            case GraphConfigurationKey.NodeColorMappingBoundaryColorA:
                NodeColorMapping.BoundaryColorA = value;
                return true;
            case GraphConfigurationKey.NodeColorMappingBoundaryColorB:
                NodeColorMapping.BoundaryColorB = value;
                return true;
            case GraphConfigurationKey.NodeColorMappingBoundaryColorC:
                NodeColorMapping.BoundaryColorC = value;
                return true;
            case GraphConfigurationKey.AlphaNodeColorPropagated:
                AlphaNodeColorPropagated = value;
                return true;
            case GraphConfigurationKey.AlphaNodeColorUnPropagated:
                AlphaNodeColorUnPropagated = value;
                return true;
            case GraphConfigurationKey.AlphaEdgeColorPropagated:
                AlphaEdgeColorPropagated = value;
                return true;
            case GraphConfigurationKey.AlphaEdgeColorUnPropagated:
                AlphaEdgeColorUnPropagated = value;
                return true;
            case GraphConfigurationKey.NbOntologyColor:
                NbOntologyColor = Mathf.RoundToInt(value);
                return true;
            case GraphConfigurationKey.MaxDeltaOntologyAlgo:
                MaxDeltaOntologyAlgo = Mathf.RoundToInt(value);
                return true;
            case GraphConfigurationKey.SaturationOntologyColor:
                SaturationOntologyColor = value;
                return true;
            case GraphConfigurationKey.ValueOntologyColor:
                ValueOntologyColor = value;
                return true;
            case GraphConfigurationKey.LabelNodgePropagation:
                LabelNodgePropagation = Mathf.RoundToInt(value);
                return true;
            case GraphConfigurationKey.SeedRandomPosition:
                SeedRandomPosition = Mathf.RoundToInt(value);
                return true;
            case GraphConfigurationKey.GraphModeTransitionTime:
                GraphModeTransitionTime = value;
                return true;
            case GraphConfigurationKey.DefaultTickDeltaTime:
                SimuParameters.TickDeltaTime = value;
                return true;
            case GraphConfigurationKey.DefaultMaxSimulationTime:
                SimuParameters.MaxSimulationTime = value;
                return true;
            case GraphConfigurationKey.DefaultLerpSmooth:
                SimuParameters.LerpSmooth = value;
                return true;
            case GraphConfigurationKey.DefaultLightSpringForce:
                SimuParameters.LightSpringForce = value;
                return true;
            case GraphConfigurationKey.DefaultLightCoulombForce:
                SimuParameters.LightCoulombForce = value;
                return true;
            case GraphConfigurationKey.DefaultLightDamping:
                SimuParameters.LightDamping = value;
                return true;
            case GraphConfigurationKey.DefaultLightSpringDistance:
                SimuParameters.LightSpringDistance = value;
                return true;
            case GraphConfigurationKey.DefaultLightCoulombDistance:
                SimuParameters.LightCoulombDistance = value;
                return true;
            case GraphConfigurationKey.DefaultLightMaxVelocity:
                SimuParameters.LightMaxVelocity = value;
                return true;
            case GraphConfigurationKey.DefaultLightStopVelocity:
                SimuParameters.LightStopVelocity = value;
                return true;
            case GraphConfigurationKey.DefaultDenseSpringForce:
                SimuParameters.DenseSpringForce = value;
                return true;
            case GraphConfigurationKey.DefaultDenseCoulombForce:
                SimuParameters.DenseCoulombForce = value;
                return true;
            case GraphConfigurationKey.DefaultDenseDamping:
                SimuParameters.DenseDamping = value;
                return true;
            case GraphConfigurationKey.DefaultDenseSpringDistance:
                SimuParameters.DenseSpringDistance = value;
                return true;
            case GraphConfigurationKey.DefaultDenseCoulombDistance:
                SimuParameters.DenseCoulombDistance = value;
                return true;
            case GraphConfigurationKey.DefaultDenseMaxVelocity:
                SimuParameters.DenseMaxVelocity = value;
                return true;
            case GraphConfigurationKey.DefaultDenseStopVelocity:
                SimuParameters.DenseStopVelocity = value;
                return true;
            case GraphConfigurationKey.LensTickDeltaTime:
                LensSimuParameters.TickDeltaTime = value;
                return true;
            case GraphConfigurationKey.LensMaxSimulationTime:
                LensSimuParameters.MaxSimulationTime = value;
                return true;
            case GraphConfigurationKey.LensLerpSmooth:
                LensSimuParameters.LerpSmooth = value;
                return true;
            case GraphConfigurationKey.LensLightSpringForce:
                LensSimuParameters.LightSpringForce = value;
                return true;
            case GraphConfigurationKey.LensLightCoulombForce:
                LensSimuParameters.LightCoulombForce = value;
                return true;
            case GraphConfigurationKey.LensLightDamping:
                LensSimuParameters.LightDamping = value;
                return true;
            case GraphConfigurationKey.LensLightSpringDistance:
                LensSimuParameters.LightSpringDistance = value;
                return true;
            case GraphConfigurationKey.LensLightCoulombDistance:
                LensSimuParameters.LightCoulombDistance = value;
                return true;
            case GraphConfigurationKey.LensLightMaxVelocity:
                LensSimuParameters.LightMaxVelocity = value;
                return true;
            case GraphConfigurationKey.LensLightStopVelocity:
                LensSimuParameters.LightStopVelocity = value;
                return true;
            case GraphConfigurationKey.LensDenseSpringForce:
                LensSimuParameters.DenseSpringForce = value;
                return true;
            case GraphConfigurationKey.LensDenseCoulombForce:
                LensSimuParameters.DenseCoulombForce = value;
                return true;
            case GraphConfigurationKey.LensDenseDamping:
                LensSimuParameters.DenseDamping = value;
                return true;
            case GraphConfigurationKey.LensDenseSpringDistance:
                LensSimuParameters.DenseSpringDistance = value;
                return true;
            case GraphConfigurationKey.LensDenseCoulombDistance:
                LensSimuParameters.DenseCoulombDistance = value;
                return true;
            case GraphConfigurationKey.LensDenseMaxVelocity:
                LensSimuParameters.DenseMaxVelocity = value;
                return true;
            case GraphConfigurationKey.LensDenseStopVelocity:
                LensSimuParameters.DenseStopVelocity = value;
                return true;
        }
            return false;
    }

    public bool SetValue(GraphConfigurationKey key, bool value)
    {
        switch (key)
        {
            case GraphConfigurationKey.ShowLabelImmersion:
                ShowLabelImmersion = value;
                return true;
            case GraphConfigurationKey.ShowLabelDesk:
                ShowLabelDesk = value;
                return true;
            case GraphConfigurationKey.CanSelectEdges:
                CanSelectEdges = value;
                return true;
            case GraphConfigurationKey.DisplayEdges:
                DisplayEdges = value;
                return true;
            case GraphConfigurationKey.ResetPositionNodeOnUpdate:
                ResetPositionNodeOnUpdate = value;
                return true;
            case GraphConfigurationKey.DisplayInterSelectedNeighborEdges:
                DisplayInterSelectedNeighborEdges = value;
                return true;
            case GraphConfigurationKey.ShowWatch:
                ShowWatch = value;
                return true;
        }

        return false;
    }

    public bool SetValue(GraphConfigurationKey key, Color value)
    {
        switch (key)
        {
            case GraphConfigurationKey.NodeColor:
                NodeColor = value;
                return true;
            case GraphConfigurationKey.NodeColorNoValueMetric:
                NodeColorNoValueMetric = value;
                return true;
            case GraphConfigurationKey.EdgeColor:
                EdgeColor = value;
                return true;
            case GraphConfigurationKey.PropagatedEdgeColor:
                PropagatedEdgeColor = value;
                return true;
            case GraphConfigurationKey.NodeColorMappingColorA:
                NodeColorMapping.ColorA = value;
                return true;
            case GraphConfigurationKey.NodeColorMappingColorB:
                NodeColorMapping.ColorB = value;
                return true;
            case GraphConfigurationKey.NodeColorMappingColorC:
                NodeColorMapping.ColorC = value;
                return true;
        }

        return false;
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
