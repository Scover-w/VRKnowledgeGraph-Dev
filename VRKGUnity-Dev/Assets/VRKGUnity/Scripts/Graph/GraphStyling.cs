using UnityEngine;

public class GraphStyling : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainerSO;

    GraphConfiguration _graphConfiguration;

    bool _isFirstSimu = true;

    async void Start()
    {
        _graphConfiguration = await _graphConfigurationContainerSO.GetGraphConfiguration();
        _graphManager.OnGraphUpdate += OnGraphUpdated;
    }

    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        switch (updateType)
        {
            case GraphUpdateType.BeforeSimulationStart:
                BeforeSimulationStart();
                break;
            case GraphUpdateType.AfterSimulationHasStopped:
                SimulationStopped();
                break;
            case GraphUpdateType.BeforeSwitchMode:
                break;
            case GraphUpdateType.AfterSwitchModeToImmersion:
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                break;
        }         
    }

    private void BeforeSimulationStart()
    {
        if(_isFirstSimu)
        {
            _isFirstSimu = false;
            StyleGraphForFirstTime();
            return;
        }
    }

    private void StyleGraphForFirstTime()
    {
        var graph = _graphManager.Graph;
        var nodesDicId = graph.NodesDicId;

        NodeStyler.GraphConfiguration = _graphConfiguration;
        EdgeStyler.GraphConfiguration = _graphConfiguration;

        foreach (var idAndNode in nodesDicId)
        {
            var node = idAndNode.Value;
            node.MainGraphStyler.StyleNodeForFirstTime();
        }

        var edgeDicId = graph.EdgesDicId;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MainGraphStyler.StyleEdgeForFirstTime();
        }
    }

    private void SimulationStopped()
    {
        var graph = _graphManager.Graph;
        var edgeDicId = graph.EdgesDicId;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MainGraphStyler.SetColliderAfterEndSimu();
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

        bool hasChangedMainGraph = styleChange.HasChanged(StyleChangeType.MainGraph);
        bool hasChangedSubGraph = styleChange.HasChanged(StyleChangeType.SubGraph);

        if (styleChange.HasChanged(StyleChangeType.Node))
        {
            foreach (var idAndNode in nodesDicId)
            {
                var node = idAndNode.Value;
                
                if(hasChangedMainGraph)
                    node.MainGraphStyler.StyleNode(styleChange, isRunningSim);

                if(hasChangedSubGraph)
                    node.SubGraphStyler.StyleNode(styleChange, isRunningSim);
            }
        }

        if (!styleChange.HasChanged(StyleChangeType.Edge))
            return;

        var edgeDicId = graph.EdgesDicId;

        foreach(var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;

            if (hasChangedMainGraph)
                edge.MainGraphStyler.StyleEdge(styleChange, isRunningSim);

            if (hasChangedSubGraph)
                edge.SubGraphStyler.StyleEdge(styleChange, isRunningSim);
        }
    }
}
