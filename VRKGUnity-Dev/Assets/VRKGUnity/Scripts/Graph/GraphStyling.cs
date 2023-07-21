using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphStyling : MonoBehaviour
{
    [SerializeField]
    EasingType _easingType = EasingType.EaseInOutQuint;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgeSelectionManager _nodgeSelectionManager;

    GraphConfiguration _graphConfiguration;

    GraphMode _graphMode = Settings.DEFAULT_GRAPH_MODE;

    EasingDel _easingFunction;

    bool _isFirstSimu = true;

    void Start()
    {
        _graphConfiguration = GraphConfiguration.Instance;
        _graphManager.OnGraphUpdate += OnGraphUpdated;

        _easingFunction = Easing.GetEasing(_easingType);

        Invoke(nameof(DelayedSubscription), .5f);
    }

    private void DelayedSubscription()
    {
        _nodgeSelectionManager.OnNodgesPropagated += OnNodgesPropagated;
        _nodgeSelectionManager.OnNodgesNewlyUnPropagated += OnNodgesPropagated;
    }

    #region OnGraphUpdated
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
        var nodesDicId = graph.NodesDicUID;

        NodeStyler.GraphConfiguration = _graphConfiguration;
        EdgeStyler.GraphConfiguration = _graphConfiguration;

        StyleChange styleChange = StyleChange.All;

        foreach (var idAndNode in nodesDicId)
        {
            var node = idAndNode.Value;
            node.MainStyler.StyleNodeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
            node.SubStyler.StyleNodeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
        }

        var edgeDicId = graph.EdgesDicUID;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MainStyler.StyleEdgeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
            edge.SubStyler.StyleEdgeBeforeFirstSimu(styleChange, Settings.DEFAULT_GRAPH_MODE);
        }
    }

    private void SimulationStopped()
    {
        var graph = _graphManager.Graph;
        var edgeDicId = graph.EdgesDicUID;

        foreach (var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;
            edge.MainStyler.SetColliderAfterEndSimu(_graphManager.GraphMode);
        }

        SetSubNodePositionsAfterSimu();
    }

    private void SetSubNodePositionsAfterSimu()
    {
        var graph = _graphManager.Graph;
        var nodesDicId = graph.NodesDicUID;
        var edgeDicId = graph.EdgesDicUID;

        NodeStyler.GraphConfiguration = _graphConfiguration;
        EdgeStyler.GraphConfiguration = _graphConfiguration;

        StyleChange styleChange = StyleChange.SubGraph.Add(StyleChange.DeskMode)
                                                    .Add(StyleChange.ImmersionMode)
                                                    .Add(StyleChange.Position);

        foreach (Node node in nodesDicId.Values)
        {
            node.SubStyler.StyleNode(styleChange, _graphMode, false);
        }

        foreach (Edge edge in edgeDicId.Values)
        {
            edge.SubStyler.StyleEdge(styleChange, _graphMode, false);
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
        var nodesDicId = graph.NodesDicUID;
        var edgesDicId = graph.EdgesDicUID;

        float speed = 1f / _graphConfiguration.GraphModeTransitionTime;
        float time = 0f;

       
        while (time < 1f)
        {
            float easedT = _easingFunction(time);

            foreach (Node node in nodesDicId.Values)
            {
                node.MainStyler.StyleTransitionNode(easedT, isNextDesk);
            }

            foreach (Edge edge in edgesDicId.Values)
            {
                edge.MainStyler.StyleTransitionEdge(easedT, isNextDesk);
            }

            yield return null;

            time += Time.deltaTime * speed;
        }



        StyleChange styleChange = StyleChange.MainGraph.Add(StyleChange.SubGraph)
                                                   .Add(StyleChange.DeskMode)
                                                   .Add(StyleChange.ImmersionMode)
                                                   .Add(StyleChange.Edge)
                                                   .Add(StyleChange.Node)
                                                   .Add(StyleChange.Position)
                                                   .Add(StyleChange.Size);

        GraphMode nextGraph = isNextDesk ? GraphMode.Desk : GraphMode.Immersion;

        foreach (Node node in nodesDicId.Values)
        {
            node.MainStyler.StyleNode(styleChange, nextGraph);
            node.SubStyler.StyleNode(styleChange, nextGraph);
        }

        foreach (Edge edge in edgesDicId.Values)
        {
            edge.MainStyler.StyleEdge(styleChange, nextGraph);
            edge.SubStyler.StyleEdge(styleChange, nextGraph);
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
    #endregion


    public void OnNodgesPropagated(Nodges nodges)
    {
        StyleChange styleChange = StyleChange.MainGraph.Add(StyleChange.SubGraph)
                                                    .Add(StyleChange.DeskMode)
                                                    .Add(StyleChange.ImmersionMode)
                                                    .Add(StyleChange.Node)
                                                    .Add(StyleChange.Edge)
                                                    .Add(StyleChange.Color);

        bool isRunningSim = _graphManager.IsRunningSimulation;
        GraphMode graphMode = _graphManager.GraphMode;

        List<Node> nodes = nodges.Nodes;

        foreach(Node node in nodes) 
        {
            node.MainStyler.StyleNode(styleChange, graphMode, isRunningSim);
            node.SubStyler.StyleNode(styleChange, graphMode, isRunningSim);
        }


        List<Edge> edges = nodges.Edges;

        foreach (Edge edge in edges)
        {
            edge.MainStyler.StyleEdge(styleChange, graphMode, isRunningSim);
            edge.SubStyler.StyleEdge(styleChange, graphMode, isRunningSim);
        }
    }


    public void StyleGraph(StyleChange styleChange, GraphMode graphMode)
    {
        var graph = _graphManager.Graph;

        if (graph == null)
            return;

        var nodesDicId = graph.NodesDicUID;

        NodeStyler.GraphConfiguration = _graphConfiguration;
        EdgeStyler.GraphConfiguration = _graphConfiguration;

        bool isRunningSim = _graphManager.IsRunningSimulation;

        bool hasChangedMainGraph = styleChange.HasChanged(StyleChange.MainGraph);
        bool hasChangedSubGraph = styleChange.HasChanged(StyleChange.SubGraph);

        if (styleChange.HasChanged(StyleChange.Node))
        {
            foreach (var idAndNode in nodesDicId)
            {
                var node = idAndNode.Value;
                
                if(hasChangedMainGraph)
                    node.MainStyler.StyleNode(styleChange, graphMode, isRunningSim);

                if(hasChangedSubGraph)
                    node.SubStyler.StyleNode(styleChange, graphMode, isRunningSim);
            }
        }

        if (!styleChange.HasChanged(StyleChange.Edge))
            return;

        var edgeDicId = graph.EdgesDicUID;

        foreach(var idAndEdge in edgeDicId)
        {
            var edge = idAndEdge.Value;

            if (hasChangedMainGraph)
                edge.MainStyler.StyleEdge(styleChange, graphMode, isRunningSim);

            if (hasChangedSubGraph)
                edge.SubStyler.StyleEdge(styleChange, graphMode, isRunningSim);
        }
    }
}
