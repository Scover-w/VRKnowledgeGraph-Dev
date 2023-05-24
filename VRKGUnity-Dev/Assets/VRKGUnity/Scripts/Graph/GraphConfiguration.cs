using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GraphConfiguration
{
    [Header("Small Number Graph")]
    public float SpringForce = 5f;
    public float CoulombForce = .1f;
    public float Damping = 1f;
    public float SpringDistance = 1f;
    public float CoulombDistance = 2f;
    public float MaxVelocity = 10f;
    public float StopVelocity = .19f;

    [Header("Big Number Graph")]
    public float BigSpringForce = 5f;
    public float BigCoulombForce = .1f;
    public float BigDamping = 1f;
    public float BigSpringDistance = 15f;
    public float BigCoulombDistance = 30f;
    public float BigMaxVelocity = 10f;
    public float BigStopVelocity = 2f;

    
    [Header("Styling")]
    public float DefaultNodeSizeSmallGraph = 1f;
    public float DefaultNodeSizeBigGraph = 1f;

    public float DefaultLabelSizeSmallGraph = 1f;
    public float DefaultLabelSizeBigGraph = 1f;

    [Header("Miscelaneous")]
    public int LabelNodgePropagation = 1;

    public int SeedRandomPosition = 0;
    public bool ResetPositionNodeOnUpdate = true;

    [HideInInspector]
    public GraphMetricType SelectedMetricType = GraphMetricType.None;


    private static string _graphConfigPath;

    public static GraphConfiguration Load()
    {
        SetPath();

        if (File.Exists(_graphConfigPath))
        {
            string json = File.ReadAllText(_graphConfigPath);
            var graphConfig = JsonConvert.DeserializeObject<GraphConfiguration>(json);

            return graphConfig;
        }
        
        
        var graphConfigB = new GraphConfiguration();
        graphConfigB.Save();
        return graphConfigB;
    }

    public void Save()
    {
        SetPath();

        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(_graphConfigPath, json);
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
