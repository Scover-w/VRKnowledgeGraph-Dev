using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public float SmallGraphSize = 1f;
    public float BigGraphSize = 1f;


    [Space(5)]
    [Header("Node Size")]
    public float NodeSizeSmallGraph = 1f;
    public float NodeSizeBigGraph = 1f;

    public float NodeMinSizeSmallGraph = .8f;
    public float NodeMaxSizeSmallGraph = .8f;

    public float NodeMinSizeBigGraph = .8f;
    public float NodeMaxSizeBigGraph = .8f;

    [Space(5)]
    [Header("Label Size")]
    public float LabelSizeSmallGraph = 1f;
    public float LabelSizeBigGraph = 1f;


    [Space(5)]
    [Header("Edge Thickness")]
    public float EdgeThicknessSmallGraph = 1f;
    public float EdgeThicknessBigGraph = 1f;


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
    public Color NodeMappingAColor
    {
        get
        {
            return _nodeMappingAColor.ToUnityColor();
        }
        set
        {
            _nodeMappingAColor = value.ToSystemColor();
        }
    }

    [JsonIgnore]
    public Color NodeMappingBColor
    {
        get
        {
            return _nodeMappingBColor.ToUnityColor();
        }
        set
        {
            _nodeMappingBColor = value.ToSystemColor();
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
    public Color EdgeMappingAColor
    {
        get
        {
            return _edgeMappingAColor.ToUnityColor();
        }
        set
        {
            _edgeMappingAColor = value.ToSystemColor();
        }
    }

    [JsonIgnore]
    public Color EdgeMappingBColor
    {
        get
        {
            return _edgeMappingBColor.ToUnityColor();
        }
        set
        {
            _edgeMappingBColor = value.ToSystemColor();
        }
    }



    [JsonProperty("NodeColor_")]
    private System.Drawing.Color _nodeColor;

    [JsonProperty("NodeMappingAColor_")]
    private System.Drawing.Color _nodeMappingAColor = System.Drawing.Color.White;

    [JsonProperty("NodeMappingBColor_")]
    private System.Drawing.Color _nodeMappingBColor = System.Drawing.Color.White;

    [JsonProperty("EdgeColor_")]
    private System.Drawing.Color _edgeColor = System.Drawing.Color.White;

    [JsonProperty("EdgeMappingAColor_")]
    private System.Drawing.Color _edgeMappingAColor = System.Drawing.Color.White;

    [JsonProperty("EdgeMappingBColor_")]
    private System.Drawing.Color _edgeMappingBColor = System.Drawing.Color.White;

    [Space(30)]
    [Header("Miscelaneous")]
    [Space(5)]
    public int LabelNodgePropagation = 1;

    public int SeedRandomPosition = 0;
    public bool ResetPositionNodeOnUpdate = true;
    public bool CanSelectEdges = true;

    [HideInInspector]
    public GraphMetricType SelectedMetricTypeSize = GraphMetricType.None;

    [HideInInspector]
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
    Degree
}

// TODO: InterpolationType enum
// Sigmoid, etc...
