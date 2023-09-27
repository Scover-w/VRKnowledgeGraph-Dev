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
    StylingManager _stylingManager;

    [SerializeField]
    MainGraph _mainGraph;

    [SerializeField]
    SubGraph _subGraph;

    [SerializeField]
    NodgeSelectionManager _nodgeSelectionManager;

    [SerializeField]
    NodgePool _nodgePool;

    [SerializeField]
    HistoryFilterManager _historyFilterManager;

    Graph _graph;

    GraphDbRepository _graphRepo;
    GraphConfiguration _graphConfiguration;

    GraphMode _graphMode;
    GraphMode _nextGraphMode;
    bool _isSwitchingMode = false;
    bool _isRetrievingFromDb = false;

    void Start()
    {
        Scene currentScene = gameObject.scene; 
        SceneManager.SetActiveScene(currentScene);

        _referenceHolderSo.GraphManager = this;

        _graphMode = GraphMode.Desk;
        _graphConfiguration = GraphConfiguration.Instance;

        Invoke(nameof(CreateStartGraphAsync), 1f);
    }


    private async void CreateStartGraphAsync()
    {
        _graphRepo = _referenceHolderSo.SelectedGraphDbRepository;
        var graphRepoUris = _graphRepo.GraphDbRepositoryNamespaces;

        SPARQLAdditiveBuilder sparqlBuilder = new();
        string queryString = sparqlBuilder.Build();

        _isRetrievingFromDb = true;
        OnGraphUpdate?.Invoke(GraphUpdateType.RetrievingFromDb);
        var nodges = await NodgesHelper.RetrieveGraph(queryString, _graphRepo);
        _isRetrievingFromDb = false;


        _graph = new Graph(this, _stylingManager, nodges, _nodgePool, _graphRepo);
        _graph.ResetMainNodePositionsTf();

        _graph.CalculateMetrics(graphRepoUris);

        SimulationWillStart();
        _graphSimulation.Run(_graph);
    }


    public void ResimulateGraph()
    {
        if (_graphSimulation.IsRunningSimulation)
            _graphSimulation.ForceStop();


        OnGraphUpdate?.Invoke(GraphUpdateType.RetrievingFromDb); // Need to call it even if no retrieving, to follow the OnGraphUpdateFlux

        _graph.ResetMainNodePositionsTf();
        _graph.CalculateMetrics(_graphRepo.GraphDbRepositoryNamespaces);

        _stylingManager.UpdateStyling(StyleChange.All);

        SimulationWillStart();
        _graphSimulation.Run(_graph);
    }


    public async void UpdateGraphFromHistoryFilter(SPARQLAdditiveBuilder sPARQLAdditiveBuilder)
    {
        if (_graphSimulation.IsRunningSimulation)
            _graphSimulation.ForceStop();


        string query = sPARQLAdditiveBuilder.Build();

        _isRetrievingFromDb = true;
        OnGraphUpdate?.Invoke(GraphUpdateType.RetrievingFromDb);
        var nodges = await NodgesHelper.RetrieveGraph(query, _graphRepo);
        _isRetrievingFromDb = false;

        DebugChrono.Instance.Start("UpdateGraph");
        _graph.UpdateNodges(nodges);
        DebugChrono.Instance.Stop("UpdateGraph");

        _graph.ResetMainNodePositionsTf();
        _graph.CalculateMetrics(_graphRepo.GraphDbRepositoryNamespaces);

        _stylingManager.UpdateStyling(StyleChange.All);

        SimulationWillStart();
        _graphSimulation.Run(_graph);
    }

    public void RecalculateMetrics()
    {
        _graph.CalculateMetrics(_graphRepo.GraphDbRepositoryNamespaces);
    }

    public bool CanSwitchMode()
    {
        return !IsRunningSimulation && !_isSwitchingMode && !_isRetrievingFromDb;
    }


    #region GRAPH_UPDATES_EVENT
    public void SimulationWillStart()
    {
        OnGraphUpdate?.Invoke(GraphUpdateType.BeforeSimulationStart);
    }

    public void SimulationStopped()
    {
        DebugDev.Log("SimulationStopped");
        OnGraphUpdate?.Invoke(GraphUpdateType.AfterSimulationHasStopped);
    }

    public bool TrySwitchModeToDesk()
    {
        if (IsRunningSimulation || _isSwitchingMode)
        {
            Debug.Log("Couldn't switch Mode");
            return false;
        }

        if (_graphMode == GraphMode.Desk)
        {
            Debug.Log("Already in Desk Mode");
            return false;
        }

        _isSwitchingMode = true;
        _nextGraphMode = GraphMode.Desk;
        OnGraphUpdate?.Invoke(GraphUpdateType.BeforeSwitchMode);

        Invoke(nameof(AfterSwitchMode), _graphConfiguration.GraphModeTransitionTime);

        return true;
    }

    public bool TrySwitchModeToImmersion()
    {
        if (IsRunningSimulation || _isSwitchingMode)
        {
            Debug.Log("Couldn't switch Mode");
            return false;
        }

        if (_graphMode == GraphMode.Immersion)
        {
            Debug.Log("Already in Immersion Mode");
            return false;
        }

        _isSwitchingMode = true;
        _nextGraphMode = GraphMode.Immersion;
        OnGraphUpdate?.Invoke(GraphUpdateType.BeforeSwitchMode);

        Invoke(nameof(AfterSwitchMode), _graphConfiguration.GraphModeTransitionTime);

        return true;
    }

    private void AfterSwitchMode()
    {
        _isSwitchingMode = false;

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
    Main,
    Sub
}

public enum GraphName
{
    Desk,
    Lense,
    Immersion,
    GPS
}

public enum GraphUpdateType
{
    RetrievingFromDb,

    BeforeSimulationStart,
    AfterSimulationHasStopped,

    BeforeSwitchMode,

    AfterSwitchModeToDesk,
    AfterSwitchModeToImmersion
}

public delegate void ValueChanged(bool newValue);