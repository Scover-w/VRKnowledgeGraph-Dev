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
        if(Instance != null)
            return Instance;

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


    public bool SetValue(GraphConfigKey key, string value)
    {
        switch (key)
        {
            case GraphConfigKey.SelectedMetricTypeSize:
                SelectedMetricTypeSize = GraphConfigurationTools.StringToEnum(value);
                return true;
            case GraphConfigKey.SelectedMetricTypeColor:
                SelectedMetricTypeColor = GraphConfigurationTools.StringToEnum(value);
                return true;
        }

        return false;
    }

    public bool SetValue(GraphConfigKey key, float value)
    {
        switch (key)
        {
            case GraphConfigKey.ImmersionGraphSize:
                ImmersionGraphSize = value;
                return true;
            case GraphConfigKey.DeskGraphSize:
                DeskGraphSize = value;
                return true;
            case GraphConfigKey.WatchGraphSize:
                WatchGraphSize = value;
                return true;
            case GraphConfigKey.LensGraphSize:
                LensGraphSize = value;
                return true;
            case GraphConfigKey.NodeSizeImmersion:
                NodeSizeImmersion = value;
                return true;
            case GraphConfigKey.NodeSizeDesk:
                NodeSizeDesk = value;
                return true;
            case GraphConfigKey.NodeSizeWatch:
                NodeSizeWatch = value;
                return true;
            case GraphConfigKey.NodeSizeLens:
                NodeSizeLens = value;
                return true;
            case GraphConfigKey.NodeMinSizeImmersion:
                NodeMinSizeImmersion = value;
                return true;
            case GraphConfigKey.NodeMaxSizeImmersion:
                NodeMaxSizeImmersion = value;
                return true;
            case GraphConfigKey.NodeMinSizeDesk:
                NodeMinSizeDesk = value;
                return true;
            case GraphConfigKey.NodeMaxSizeDesk:
                NodeMaxSizeDesk = value;
                return true;
            case GraphConfigKey.NodeMinSizeLens:
                NodeMinSizeLens = value;
                return true;
            case GraphConfigKey.NodeMaxSizeLens:
                NodeMaxSizeLens = value;
                return true;
            case GraphConfigKey.LabelNodeSizeImmersion:
                LabelNodeSizeImmersion = value;
                return true;
            case GraphConfigKey.LabelNodeSizeDesk:
                LabelNodeSizeDesk = value;
                return true;
            case GraphConfigKey.LabelNodeSizeLens:
                LabelNodeSizeLens = value;
                return true;
            case GraphConfigKey.EdgeThicknessImmersion:
                EdgeThicknessImmersion = value;
                return true;
            case GraphConfigKey.EdgeThicknessDesk:
                EdgeThicknessDesk = value;
                return true;
            case GraphConfigKey.EdgeThicknessLens:
                EdgeThicknessLens = value;
                return true;
            case GraphConfigKey.EdgeThicknessWatch:
                EdgeThicknessWatch = value;
                return true;
            case GraphConfigKey.NodeColorMappingBoundaryColorA:
                NodeColorMapping.BoundaryColorA = value;
                return true;
            case GraphConfigKey.NodeColorMappingBoundaryColorB:
                NodeColorMapping.BoundaryColorB = value;
                return true;
            case GraphConfigKey.NodeColorMappingBoundaryColorC:
                NodeColorMapping.BoundaryColorC = value;
                return true;
            case GraphConfigKey.AlphaNodeColorPropagated:
                AlphaNodeColorPropagated = value;
                return true;
            case GraphConfigKey.AlphaNodeColorUnPropagated:
                AlphaNodeColorUnPropagated = value;
                return true;
            case GraphConfigKey.AlphaEdgeColorPropagated:
                AlphaEdgeColorPropagated = value;
                return true;
            case GraphConfigKey.AlphaEdgeColorUnPropagated:
                AlphaEdgeColorUnPropagated = value;
                return true;
            case GraphConfigKey.NbOntologyColor:
                NbOntologyColor = Mathf.RoundToInt(value);
                return true;
            case GraphConfigKey.MaxDeltaOntologyAlgo:
                MaxDeltaOntologyAlgo = Mathf.RoundToInt(value);
                return true;
            case GraphConfigKey.SaturationOntologyColor:
                SaturationOntologyColor = value;
                return true;
            case GraphConfigKey.ValueOntologyColor:
                ValueOntologyColor = value;
                return true;
            case GraphConfigKey.LabelNodgePropagation:
                LabelNodgePropagation = Mathf.RoundToInt(value);
                return true;
            case GraphConfigKey.SeedRandomPosition:
                SeedRandomPosition = Mathf.RoundToInt(value);
                return true;
            case GraphConfigKey.GraphModeTransitionTime:
                GraphModeTransitionTime = value;
                return true;
            case GraphConfigKey.DefaultTickDeltaTime:
                SimuParameters.TickDeltaTime = value;
                return true;
            case GraphConfigKey.DefaultMaxSimulationTime:
                SimuParameters.MaxSimulationTime = value;
                return true;
            case GraphConfigKey.DefaultLerpSmooth:
                SimuParameters.LerpSmooth = value;
                return true;
            case GraphConfigKey.DefaultLightSpringForce:
                SimuParameters.LightSpringForce = value;
                return true;
            case GraphConfigKey.DefaultLightCoulombForce:
                SimuParameters.LightCoulombForce = value;
                return true;
            case GraphConfigKey.DefaultLightDamping:
                SimuParameters.LightDamping = value;
                return true;
            case GraphConfigKey.DefaultLightSpringDistance:
                SimuParameters.LightSpringDistance = value;
                return true;
            case GraphConfigKey.DefaultLightCoulombDistance:
                SimuParameters.LightCoulombDistance = value;
                return true;
            case GraphConfigKey.DefaultLightMaxVelocity:
                SimuParameters.LightMaxVelocity = value;
                return true;
            case GraphConfigKey.DefaultLightStopVelocity:
                SimuParameters.LightStopVelocity = value;
                return true;
            case GraphConfigKey.DefaultDenseSpringForce:
                SimuParameters.DenseSpringForce = value;
                return true;
            case GraphConfigKey.DefaultDenseCoulombForce:
                SimuParameters.DenseCoulombForce = value;
                return true;
            case GraphConfigKey.DefaultDenseDamping:
                SimuParameters.DenseDamping = value;
                return true;
            case GraphConfigKey.DefaultDenseSpringDistance:
                SimuParameters.DenseSpringDistance = value;
                return true;
            case GraphConfigKey.DefaultDenseCoulombDistance:
                SimuParameters.DenseCoulombDistance = value;
                return true;
            case GraphConfigKey.DefaultDenseMaxVelocity:
                SimuParameters.DenseMaxVelocity = value;
                return true;
            case GraphConfigKey.DefaultDenseStopVelocity:
                SimuParameters.DenseStopVelocity = value;
                return true;
            case GraphConfigKey.LensTickDeltaTime:
                LensSimuParameters.TickDeltaTime = value;
                return true;
            case GraphConfigKey.LensMaxSimulationTime:
                LensSimuParameters.MaxSimulationTime = value;
                return true;
            case GraphConfigKey.LensLerpSmooth:
                LensSimuParameters.LerpSmooth = value;
                return true;
            case GraphConfigKey.LensLightSpringForce:
                LensSimuParameters.LightSpringForce = value;
                return true;
            case GraphConfigKey.LensLightCoulombForce:
                LensSimuParameters.LightCoulombForce = value;
                return true;
            case GraphConfigKey.LensLightDamping:
                LensSimuParameters.LightDamping = value;
                return true;
            case GraphConfigKey.LensLightSpringDistance:
                LensSimuParameters.LightSpringDistance = value;
                return true;
            case GraphConfigKey.LensLightCoulombDistance:
                LensSimuParameters.LightCoulombDistance = value;
                return true;
            case GraphConfigKey.LensLightMaxVelocity:
                LensSimuParameters.LightMaxVelocity = value;
                return true;
            case GraphConfigKey.LensLightStopVelocity:
                LensSimuParameters.LightStopVelocity = value;
                return true;
            case GraphConfigKey.LensDenseSpringForce:
                LensSimuParameters.DenseSpringForce = value;
                return true;
            case GraphConfigKey.LensDenseCoulombForce:
                LensSimuParameters.DenseCoulombForce = value;
                return true;
            case GraphConfigKey.LensDenseDamping:
                LensSimuParameters.DenseDamping = value;
                return true;
            case GraphConfigKey.LensDenseSpringDistance:
                LensSimuParameters.DenseSpringDistance = value;
                return true;
            case GraphConfigKey.LensDenseCoulombDistance:
                LensSimuParameters.DenseCoulombDistance = value;
                return true;
            case GraphConfigKey.LensDenseMaxVelocity:
                LensSimuParameters.DenseMaxVelocity = value;
                return true;
            case GraphConfigKey.LensDenseStopVelocity:
                LensSimuParameters.DenseStopVelocity = value;
                return true;
        }
            return false;
    }

    public bool SetValue(GraphConfigKey key, bool value)
    {
        switch (key)
        {
            case GraphConfigKey.ShowLabelImmersion:
                ShowLabelImmersion = value;
                return true;
            case GraphConfigKey.ShowLabelDesk:
                ShowLabelDesk = value;
                return true;
            case GraphConfigKey.CanSelectEdges:
                CanSelectEdges = value;
                return true;
            case GraphConfigKey.DisplayEdges:
                DisplayEdges = value;
                return true;
            case GraphConfigKey.ResetPositionNodeOnUpdate:
                ResetPositionNodeOnUpdate = value;
                return true;
            case GraphConfigKey.DisplayInterSelectedNeighborEdges:
                DisplayInterSelectedNeighborEdges = value;
                return true;
            case GraphConfigKey.ShowWatch:
                ShowWatch = value;
                return true;
        }

        return false;
    }

    public bool SetValue(GraphConfigKey key, Color value)
    {
        switch (key)
        {
            case GraphConfigKey.NodeColor:
                NodeColor = value;
                return true;
            case GraphConfigKey.NodeColorNoValueMetric:
                NodeColorNoValueMetric = value;
                return true;
            case GraphConfigKey.EdgeColor:
                EdgeColor = value;
                return true;
            case GraphConfigKey.PropagatedEdgeColor:
                PropagatedEdgeColor = value;
                return true;
            case GraphConfigKey.NodeColorMappingColorA:
                NodeColorMapping.ColorA = value;
                return true;
            case GraphConfigKey.NodeColorMappingColorB:
                NodeColorMapping.ColorB = value;
                return true;
            case GraphConfigKey.NodeColorMappingColorC:
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
