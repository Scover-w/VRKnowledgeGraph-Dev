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

        foreach (var idAndNode in nodesDicId)
        {
            var node = idAndNode.Value;
            node.NodeStyler.StyleNode();
        }
    }

    // TODO : Animate the color change
}
