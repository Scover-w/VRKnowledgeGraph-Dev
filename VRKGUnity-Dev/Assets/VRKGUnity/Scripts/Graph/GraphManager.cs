using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public delegate void GraphUpdateDel(GraphUpdateType updateType);

[DefaultExecutionOrder(-1)]
public class GraphManager : MonoBehaviour
{
    public GraphConfiguration GraphConfiguration { get { return _graphConfiguration; } }
    public Graph Graph { get { return _graph; } }

    public MainGraph MainGraph { get { return _mainGraph; } }
    public SubGraph SubGraph { get { return _subGraph; } }

    public bool IsRunningSimulation { get { return _graphSimulation.IsRunningSimulation; } }

    public GraphUpdateDel OnGraphUpdate;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphSimulation _graphSimulation;

    [SerializeField]
    GraphUI _graphUI;

    [SerializeField]
    GraphStyling _graphStyling;

    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainerSO;

    [SerializeField]
    MainGraph _mainGraph;

    [SerializeField]
    SubGraph _subGraph;

    [SerializeField]
    NodgeSelectionManager _nodgeSelectionManager;

    [SerializeField]
    NodgePool _nodgePool;

    Graph _graph;
    SPARQLAdditiveBuilder _sparqlBuilder;
    DynamicFilterManager _dynamicFilterManager;

    GraphDbRepository _graphRepo;
    GraphConfiguration _graphConfiguration;

    async void Start()
    {
        _dynamicFilterManager = new();
        _graphConfiguration = await _graphConfigurationContainerSO.GetGraphConfiguration();

        Invoke(nameof(CreateStartGraphAsync), 1f);
    }

    private async Task CreateStartGraphAsync()
    {
        _graphRepo = _referenceHolderSo.SelectedGraphDbRepository;
        var graphRepoUris = _graphRepo.GraphDbRepositoryUris;

        _sparqlBuilder = new(graphRepoUris);
        string queryString = _sparqlBuilder.Build();
        var nodges = await NodgesHelper.RetreiveGraph(queryString, _graphRepo);

        _graph = new Graph(this, _graphStyling, nodges, _nodgePool);

        _nodgeSelectionManager.SetNodgeTfs(_graph.NodesDicTf);

        _graph.CalculateMetrics(graphRepoUris);

        SimulationWillStart();
        _graphSimulation.Run(_graph);
    }


    public void Add(SPARQLQuery query)
    {
        _sparqlBuilder.Add(query);
        string queryString = _sparqlBuilder.Build();
        Debug.Log(queryString);
        UpdateGraph(queryString);
    }

    public async void UpdateGraph(string query)
    {
        if (_graphSimulation.IsRunningSimulation)
            _graphSimulation.ForceStop();

        var nodges = await NodgesHelper.RetreiveGraph(query, _graphRepo);
        nodges = _dynamicFilterManager.Filter(nodges);

        DebugChrono.Instance.Start("UpdateGraph");
        await _graph.UpdateNodges(nodges);
        DebugChrono.Instance.Stop("UpdateGraph");

        _graph.CalculateMetrics(_graphRepo.GraphDbRepositoryUris);

        SimulationWillStart();
        _graphSimulation.Run(_graph);
    }


    #region GRAPH_UPDATES_EVENT
    public void SimulationWillStart()
    {
        OnGraphUpdate?.Invoke(GraphUpdateType.BeforeSimulationStart);
    }

    public void SimulationStopped()
    {
        OnGraphUpdate?.Invoke(GraphUpdateType.SimulationHasStopped);
    }

    public void TrySwitchModeToDesk()
    {
        if (IsRunningSimulation)
            return;

        OnGraphUpdate?.Invoke(GraphUpdateType.SwitchModeToDesk);
    }

    public void TrySwitchModeToImmersion()
    {
        if (IsRunningSimulation)
            return;

        OnGraphUpdate?.Invoke(GraphUpdateType.SwitchModeToImmersion);
    }
    #endregion
}

public enum GraphType
{
    Sub,
    Main
}

public enum GraphMode
{
    Desk,
    Immersion
}

public enum GraphUpdateType
{
    BeforeSimulationStart,
    SimulationHasStopped,
    SwitchModeToDesk,
    SwitchModeToImmersion
}