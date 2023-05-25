using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphStyling : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainerSO;

    GraphConfiguration _graphConfiguration;

    async void Start()
    {
        _graphConfiguration = await _graphConfigurationContainerSO.GetGraphConfiguration();
    }


    public void StyleGraphForFirstTime()
    {
        var graph = _graphManager.Graph;
        var nodesDicId = graph.NodesDicId;

        NodeStyler.GraphConfiguration = _graphConfiguration;
        EdgeStyler.GraphConfiguration = _graphConfiguration;

        foreach (var idAndNode in nodesDicId)
        {
            var node = idAndNode.Value;
            node.NodeStyler.StyleNodeForFirstTime();
        }
    }

    public void StyleGraph()
    {
        var graph = _graphManager.Graph;

        if (graph == null)
            return;

        var nodesDicId = graph.NodesDicId;

        NodeStyler.GraphConfiguration = _graphConfiguration;
        EdgeStyler.GraphConfiguration = _graphConfiguration;

        bool isRunningSim = _graphManager.IsRunningSimulation;

        foreach (var idAndNode in nodesDicId)
        {
            var node = idAndNode.Value;
            node.NodeStyler.StyleNode(isRunningSim);
        }

        var edgeDicId = graph.EdgesDicId;

        foreach(var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.EdgeStyler.StyleEdge(isRunningSim);
        }
    }
}
