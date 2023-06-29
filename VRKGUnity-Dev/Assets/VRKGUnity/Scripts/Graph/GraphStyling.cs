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
            StyleGraphBeforeFirstSimu();
            return;
        }
    }

    private void StyleGraphBeforeFirstSimu()
    {
        var graph = _graphManager.Graph;
        var nodesDicId = graph.NodesDicId;

        NodeStyler.GraphConfiguration = _graphConfiguration;
        EdgeStyler.GraphConfiguration = _graphConfiguration;

        StyleChange styleChange = new StyleChange().Add(StyleChangeType.All);

        foreach (var idAndNode in nodesDicId)
        {
            var node = idAndNode.Value;
            node.MainGraphStyler.StyleNodeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
        }

        var edgeDicId = graph.EdgesDicId;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MainGraphStyler.StyleEdgeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
        }
    }

    private void SimulationStopped()
    {
        var graph = _graphManager.Graph;
        var edgeDicId = graph.EdgesDicId;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MainGraphStyler.SetColliderAfterEndSimu(_graphManager.GraphMode);
        }
    }

    public void StyleGraph(StyleChange styleChange, GraphMode graphMode)
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
                    node.MainGraphStyler.StyleNode(styleChange, isRunningSim, graphMode);

                if(hasChangedSubGraph)
                    node.SubGraphStyler.StyleNode(styleChange, isRunningSim, graphMode);
            }
        }

        if (!styleChange.HasChanged(StyleChangeType.Edge))
            return;

        var edgeDicId = graph.EdgesDicId;

        foreach(var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;

            if (hasChangedMainGraph)
                edge.MainGraphStyler.StyleEdge(styleChange, isRunningSim, graphMode);

            if (hasChangedSubGraph)
                edge.SubGraphStyler.StyleEdge(styleChange, isRunningSim, graphMode);
        }
    }
}
