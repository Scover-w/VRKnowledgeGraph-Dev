using System.Collections;
using UnityEngine;

public class GraphStyling : MonoBehaviour
{
    [SerializeField]
    EasingType _easingType = EasingType.EaseInOutQuint;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainerSO;

    GraphConfiguration _graphConfiguration;

    GraphMode _graphMode = Settings.DEFAULT_GRAPH_MODE;

    EasingDel _easingFunction;

    bool _isFirstSimu = true;

    async void Start()
    {
        _graphConfiguration = await _graphConfigurationContainerSO.GetGraphConfiguration();
        _graphManager.OnGraphUpdate += OnGraphUpdated;

        _easingFunction = Easing.GetEasing(_easingType);
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
                BeforeSwitchMode();
                break;
            case GraphUpdateType.AfterSwitchModeToImmersion:
                AfterSwitchModeToImmersion();
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                AfterSwitchModeToDesk();
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
            node.MainNodeStyler.StyleNodeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
            node.SubNodeStyler.StyleNodeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
        }

        var edgeDicId = graph.EdgesDicId;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MainEdgeStyler.StyleEdgeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
            edge.SubEdgeStyler.StyleEdgeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
        }
    }

    private void SimulationStopped()
    {
        var graph = _graphManager.Graph;
        var edgeDicId = graph.EdgesDicId;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MainEdgeStyler.SetColliderAfterEndSimu(_graphManager.GraphMode);
        }
    }

    private void BeforeSwitchMode()
    {
        StartCoroutine(StylingTransitionMode());
    }

    IEnumerator StylingTransitionMode()
    {
        bool isNextDesk = (_graphMode != GraphMode.Desk);

        var graph = _graphManager.Graph;
        var nodesDicId = graph.NodesDicId;
        var edgesDicId = graph.EdgesDicId;

        float speed = 1f / _graphConfiguration.GraphModeTransitionTime;
        float time = 0f;

       
        while (time < 1f)
        {
            float easedT = _easingFunction(time);

            foreach (Node node in nodesDicId.Values)
            {
                node.MainNodeStyler.StyleTransitionNode(easedT, isNextDesk);
            }

            foreach (Edge edge in edgesDicId.Values)
            {
                edge.MainEdgeStyler.StyleTransitionEdge(easedT, isNextDesk);
            }

            yield return null;

            time += Time.deltaTime * speed;
        }



        StyleChange styleChange = new StyleChange().Add(StyleChangeType.MainGraph)
                                                    .Add(StyleChangeType.SubGraph)
                                                   .Add(StyleChangeType.DeskMode)
                                                   .Add(StyleChangeType.ImmersionMode)
                                                   .Add(StyleChangeType.Edge)
                                                   .Add(StyleChangeType.Node)
                                                   .Add(StyleChangeType.Position)
                                                   .Add(StyleChangeType.Size);

        GraphMode nextGraph = isNextDesk ? GraphMode.Desk : GraphMode.Immersion;

        foreach (Node node in nodesDicId.Values)
        {
            node.MainNodeStyler.StyleNode(styleChange, nextGraph);
            node.SubNodeStyler.StyleNode(styleChange, nextGraph);
        }

        foreach (Edge edge in edgesDicId.Values)
        {
            edge.MainEdgeStyler.StyleEdge(styleChange, nextGraph);
            edge.SubEdgeStyler.StyleEdge(styleChange, nextGraph);
        }

    }

    private void AfterSwitchModeToImmersion()
    {
        _graphMode = GraphMode.Immersion;


    }

    private void AfterSwitchModeToDesk()
    {
        _graphMode = GraphMode.Desk;
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
                    node.MainNodeStyler.StyleNode(styleChange, graphMode, isRunningSim);

                if(hasChangedSubGraph)
                    node.SubNodeStyler.StyleNode(styleChange, graphMode, isRunningSim);
            }
        }

        if (!styleChange.HasChanged(StyleChangeType.Edge))
            return;

        var edgeDicId = graph.EdgesDicId;

        foreach(var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;

            if (hasChangedMainGraph)
                edge.MainEdgeStyler.StyleEdge(styleChange, graphMode, isRunningSim);

            if (hasChangedSubGraph)
                edge.SubEdgeStyler.StyleEdge(styleChange, graphMode, isRunningSim);
        }
    }
}
