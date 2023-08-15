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

    public GraphMode GraphMode = GraphMode.Desk;
    public SelectionMode SelectionMode = SelectionMode.Single;

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
    public float NodeMinMaxSizeImmersion = .8f;

    [Space(5)]
    public float NodeMinMaxSizeDesk = .8f;

    [Space(5)]
    public float NodeMinMaxSizeLens = .8f;

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

    [Header("Sound")]
    [Range(0f, 1f)]
    public float GlobalVolume = 1f;
    [Range(0f, 1f)]
    public float SoundEffectVolume = 1f;
    [Range(0f, 1f)]
    public float MusicVolume = 1f;
    [Range(0f, 1f)]
    public float AidenVolume = 1f;

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
            var graphConfig = await JsonConvertHelper.DeserializeObjectAsync<GraphConfiguration>(json);
            Instance = graphConfig;
            Instance.SetDefaultStartValue();
            return graphConfig;
        }
        
        
        var graphConfigB = new GraphConfiguration();
        Instance = graphConfigB;

        await graphConfigB.Save();
        return graphConfigB;
    }

    private void SetDefaultStartValue()
    {
        GraphMode = GraphMode.Desk;
    }

    public async Task Save()
    {
        SetPath();

        string json = await JsonConvertHelper.SerializeObjectAsync(this, Formatting.Indented);
        await File.WriteAllTextAsync(_graphConfigPath, json);
    }


    public bool TrySetValue<T>(GraphConfigKey key, T value)
    {
        switch (value)
        {
            case string s:
                return TrySetValue(key, s);
            case float f:
                return TrySetValue(key, f);
            case bool b:
                return TrySetValue(key, b);
            case Color c:
                return TrySetValue(key, c);
        }

        Debug.LogError("No value with " + typeof(T) + " is handled");

        return false;
    }

    private bool TrySetValue(GraphConfigKey key, string value)
    {
        switch (key)
        {
            case GraphConfigKey.SelectionMode:
                SelectionMode = GraphConfigurationTools.StringToEnum<SelectionMode>(value);
                return true;
            case GraphConfigKey.SelectedMetricTypeSize:
                SelectedMetricTypeSize = GraphConfigurationTools.StringToEnum<GraphMetricType>(value);
                return true;
            case GraphConfigKey.SelectedMetricTypeColor:
                SelectedMetricTypeColor = GraphConfigurationTools.StringToEnum<GraphMetricType>(value);
                return true;
        }

        return false;
    }

    private bool TrySetValue(GraphConfigKey key, float value)
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
            case GraphConfigKey.NodeMinMaxSizeImmersion:
                NodeMinMaxSizeImmersion = value;
                return true;
            case GraphConfigKey.NodeMinMaxSizeDesk:
                NodeMinMaxSizeDesk = value;
                return true;
            case GraphConfigKey.NodeMinMaxSizeLens:
                NodeMinMaxSizeLens = value;
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
            case GraphConfigKey.GlobalVolume:
                GlobalVolume = value;
                return true;
            case GraphConfigKey.SoundEffectVolume:
                SoundEffectVolume = value;
                return true;
            case GraphConfigKey.MusicVolume:
                MusicVolume = value;
                return true;
            case GraphConfigKey.AidenVolume:
                AidenVolume = value;
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
            case GraphConfigKey.DefaultSpringForce:
                SimuParameters.SpringForce = value;
                return true;
            case GraphConfigKey.DefaultCoulombForce:
                SimuParameters.CoulombForce = value;
                return true;
            case GraphConfigKey.DefaultDamping:
                SimuParameters.Damping = value;
                return true;
            case GraphConfigKey.DefaultSpringDistance:
                SimuParameters.SpringDistance = value;
                return true;
            case GraphConfigKey.DefaultCoulombDistance:
                SimuParameters.CoulombDistance = value;
                return true;
            case GraphConfigKey.DefaultMaxVelocity:
                SimuParameters.MaxVelocity = value;
                return true;
            case GraphConfigKey.DefaultStopVelocity:
                SimuParameters.StopVelocity = value;
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
            case GraphConfigKey.LensSpringForce:
                LensSimuParameters.SpringForce = value;
                return true;
            case GraphConfigKey.LensCoulombForce:
                LensSimuParameters.CoulombForce = value;
                return true;
            case GraphConfigKey.LensDamping:
                LensSimuParameters.Damping = value;
                return true;
            case GraphConfigKey.LensSpringDistance:
                LensSimuParameters.SpringDistance = value;
                return true;
            case GraphConfigKey.LensCoulombDistance:
                LensSimuParameters.CoulombDistance = value;
                return true;
            case GraphConfigKey.LensMaxVelocity:
                LensSimuParameters.MaxVelocity = value;
                return true;
            case GraphConfigKey.LensStopVelocity:
                LensSimuParameters.StopVelocity = value;
                return true;
        }
            return false;
    }

    private bool TrySetValue(GraphConfigKey key, bool value)
    {
        switch (key)
        {
            case GraphConfigKey.GraphMode:
                GraphMode = (value) ? GraphMode.Immersion : GraphMode.Desk;
                return true;
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

    private bool TrySetValue(GraphConfigKey key, Color value)
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
#if UNITY_EDITOR
        if (Settings.PersistentDataPath == null)
            Settings.SetPersistentDataPath(Application.persistentDataPath);
#endif

        Debug.Log("GraphConfig SetPAth PersistentDataPath : " + Settings.PersistentDataPath);

        var folderPath = Path.Combine(Settings.PersistentDataPath, "Data");

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
