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
            node.MegaStyler.StyleNodeForFirstTime();
        }

        var edgeDicId = graph.EdgesDicId;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MegaStyler.StyleEdgeForFirstTime();
        }
    }

    public void SimulationStopped()
    {
        var graph = _graphManager.Graph;
        var edgeDicId = graph.EdgesDicId;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MegaStyler.SetColliderAfterEndSimu();
        }
    }

    public void StyleGraph(StyleChange styleChange)
    {
        var graph = _graphManager.Graph;

        if (graph == null)
            return;

        var nodesDicId = graph.NodesDicId;

        NodeStyler.GraphConfiguration = _graphConfiguration;
        EdgeStyler.GraphConfiguration = _graphConfiguration;

        bool isRunningSim = _graphManager.IsRunningSimulation;

        bool hasChangedMegaGraph = styleChange.HasChanged(StyleChangeType.MegaGraph);
        bool hasChangedMiniGraph = styleChange.HasChanged(StyleChangeType.MiniGraph);

        if (styleChange.HasChanged(StyleChangeType.Node))
        {
            foreach (var idAndNode in nodesDicId)
            {
                var node = idAndNode.Value;
                
                if(hasChangedMegaGraph)
                    node.MegaStyler.StyleNode(styleChange, isRunningSim);

                if(hasChangedMiniGraph)
                    node.MiniStyler.StyleNode(styleChange, isRunningSim);
            }
        }

        if (!styleChange.HasChanged(StyleChangeType.Edge))
            return;

        var edgeDicId = graph.EdgesDicId;

        foreach(var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;

            if (hasChangedMegaGraph)
                edge.MegaStyler.StyleEdge(styleChange, isRunningSim);

            if (hasChangedMiniGraph)
                edge.MiniStyler.StyleEdge(styleChange, isRunningSim);
        }
    }
}
