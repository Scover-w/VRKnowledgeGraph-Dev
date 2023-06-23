using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GraphManager : MonoBehaviour
{
    public GraphConfiguration GraphConfiguration { get { return _graphConfiguration; } }
    public Graph Graph { get { return _graph; } }

    public MainGraph MainGraph { get { return _mainGraph; } }
    public SubGraph SubGraph { get { return _subGraph; } }

    public GraphType GraphMode { get { return _graphMode; } }

    public bool IsRunningSimulation { get { return _graphSimulation.IsRunningSimulation; } }

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

    Graph _graph;
    SPARQLAdditiveBuilder _sparqlBuilder;
    DynamicFilterManager _dynamicFilterManager;

    GraphType _graphMode = GraphType.Sub;

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

        try
        {
            _sparqlBuilder = new(graphRepoUris);
            string queryString = _sparqlBuilder.Build();
            var nodges = await NodgesHelper.RetreiveGraph(queryString, _graphRepo);

            _graph = new Graph(this, _graphStyling, nodges, _graphRepo);

            _nodgeSelectionManager.SetNodgeTfs(_graph.NodesDicTf, _graph.EdgesDicTf);

            _graphStyling.StyleGraphForFirstTime();
            _graph.CalculateMetrics(graphRepoUris);
            _graphSimulation.Run(_graph);

        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }
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
        _graphSimulation.Run(_graph);
    }

    public void SimulationStopped()
    {
        _graphStyling.SimulationStopped();
        _subGraph.SimulationStopped();
    }
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