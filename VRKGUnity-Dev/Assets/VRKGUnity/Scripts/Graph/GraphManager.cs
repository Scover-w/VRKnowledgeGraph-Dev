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
    MainGraph _mainGraph;

    [SerializeField]
    SubGraph _subGraph;

    [SerializeField]
    NodgeSelectionManager _nodgeSelectionManager;

    [SerializeField]
    NodgePool _nodgePool;

    [SerializeField]
    DynamicFilterManager _dynamicFilterManager;

    Graph _graph;
    SPARQLAdditiveBuilder _sparqlBuilder;

    GraphDbRepository _graphRepo;
    GraphConfiguration _graphConfiguration;

    GraphMode _graphMode;
    GraphMode _nextGraphMode;
    bool _switchingMode = false;

    void Start()
    {
        Scene currentScene = gameObject.scene; 
        SceneManager.SetActiveScene(currentScene);

        _graphMode = GraphMode.Desk;
        _graphConfiguration = GraphConfiguration.Instance;

        Invoke(nameof(CreateStartGraphAsync), 1f);
    }

    private async void CreateStartGraphAsync()
    {
        _graphRepo = _referenceHolderSo.SelectedGraphDbRepository;
        var graphRepoUris = _graphRepo.GraphDbRepositoryNamespaces;

        _sparqlBuilder = new();
        string queryString = _sparqlBuilder.Build();
        var nodges = await NodgesHelper.RetrieveGraph(queryString, _graphRepo);

        _graph = new Graph(this, _graphStyling, nodges, _nodgePool, _graphRepo);

        _graph.CalculateMetrics(graphRepoUris);

        SimulationWillStart();
        _graphSimulation.Run(_graph);
    }


    [ContextMenu("Update Graph")]
    public async void UpdateGraph()
    {
        if (_graphSimulation.IsRunningSimulation)
            _graphSimulation.ForceStop();


        var filters = _dynamicFilterManager.ApplyFilters();
        _sparqlBuilder.Add(filters);

        string query = _sparqlBuilder.Build();

        var nodges = await NodgesHelper.RetrieveGraph(query, _graphRepo);

        DebugChrono.Instance.Start("UpdateGraph");
        _graph.UpdateNodges(nodges);
        DebugChrono.Instance.Stop("UpdateGraph");

        _graph.CalculateMetrics(_graphRepo.GraphDbRepositoryNamespaces);

        SimulationWillStart();
        _graphSimulation.Run(_graph);
    }


    public void ResetAll()
    {
        _referenceHolderSo.AppManagerSA.Value.ReloadKG();
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

        _switchingMode = true;
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

        _switchingMode = true;
        _nextGraphMode = GraphMode.Immersion;
        OnGraphUpdate?.Invoke(GraphUpdateType.BeforeSwitchMode);

        Invoke(nameof(AfterSwitchMode), _graphConfiguration.GraphModeTransitionTime);
    }

    private void AfterSwitchMode()
    {
        _switchingMode = false;

        if (_nextGraphMode == GraphMode.Desk)
        {
            _graphMode = GraphMode.Desk;
            OnGraphUpdate?.Invoke(GraphUpdateType.AfterSwitchModeToDesk);
        }
        else
        {
            _graphMode = GraphMode.Immersion;
            OnGraphUpdate?.Invoke(GraphUpdateType.AfterSwitchModeToImmersion);
        }
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