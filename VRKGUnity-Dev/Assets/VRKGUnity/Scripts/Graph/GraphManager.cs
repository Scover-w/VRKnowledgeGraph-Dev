using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;



[DefaultExecutionOrder(-1)]
public class GraphManager : MonoBehaviour
{
    public GraphConfiguration GraphConfiguration { get { return _graphConfiguration; } }
    public Graph Graph { get { return _graph; } }

    public MainGraph MainGraph { get { return _mainGraph; } }
    public SubGraph SubGraph { get { return _subGraph; } }

    public bool IsRunningSimulation { get { return _graphSimulation.IsRunningSimulation; } }

    public GraphMode GraphMode { get { return _graphMode; } }

    public delegate void GraphUpdateDel(GraphUpdateType updateType);

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

    GraphMode _graphMode;
    GraphMode _nextGraphMode;
    bool _switchingMode = false;

    async void Start()
    {
        Scene currentScene = gameObject.scene; 
        SceneManager.SetActiveScene(currentScene);

        _graphMode = GraphMode.Desk;
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
        OnGraphUpdate?.Invoke(GraphUpdateType.AfterSimulationHasStopped);
    }

    public void TrySwitchModeToDesk()
    {
        if (IsRunningSimulation || _switchingMode)
        {
            Debug.Log("Couldn't switch Mode");
            return;
        }

        _nextGraphMode = GraphMode.Desk;
        OnGraphUpdate?.Invoke(GraphUpdateType.BeforeSwitchMode);

        Invoke(nameof(AfterSwitchMode), _graphConfiguration.GraphModeTransitionTime);
    }

    public void TrySwitchModeToImmersion()
    {
        if (IsRunningSimulation || _switchingMode)
        {
            Debug.Log("Couldn't switch Mode");
            return;
        }

        _nextGraphMode = GraphMode.Immersion;
        OnGraphUpdate?.Invoke(GraphUpdateType.BeforeSwitchMode);

        Invoke(nameof(AfterSwitchMode), _graphConfiguration.GraphModeTransitionTime);
    }

    private void AfterSwitchMode()
    {
        if (_nextGraphMode == GraphMode.Desk)
            OnGraphUpdate?.Invoke(GraphUpdateType.AfterSwitchModeToDesk);
        else
            OnGraphUpdate?.Invoke(GraphUpdateType.AfterSwitchModeToImmersion);
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
    AfterSimulationHasStopped,

    BeforeSwitchMode,

    AfterSwitchModeToDesk,
    AfterSwitchModeToImmersion
}