using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GraphManager : MonoBehaviour
{
    public NodeUriRetriever NodeUriRetriever { get { return _nodeUriRetriever; } }
    public GraphConfiguration GraphConfiguration { get { return _graphConfiguration; } }
    public Graph Graph { get { return _graph; } }

    [SerializeField]
    GraphSimulation _graphSimulation;

    [SerializeField]
    NodgeCreator _nodgeCreator;

    [SerializeField]
    GraphUI _graphUI;

    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainerSO;

    Graph _graph;
    SPARQLAdditiveBuilder _sparqlBuilder;
    DynamicFilterManager _dynamicFilterManager;

    NodeUriRetriever _nodeUriRetriever;

    GraphConfiguration _graphConfiguration;

    void Start()
    {
        _dynamicFilterManager = new();
        _nodeUriRetriever = new();
        _graphConfiguration = _graphConfigurationContainerSO.GraphConfiguration;

        Invoke(nameof(CreateStartGraphAsync), 1f);
    }

    void Update()
    {
        if (_graph == null)
            return;

        _graph.Update();
    }

    private async Task CreateStartGraphAsync()
    {
        _sparqlBuilder = new();
        string queryString = _sparqlBuilder.Build();
        var nodges = await _nodgeCreator.RetreiveGraph(queryString, _graphConfiguration);

        _graph = new Graph(this, _graphUI, _graphConfiguration, nodges);

        _graph.CalculateMetrics();
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
        if (_graphSimulation.IsRunning)
            _graphSimulation.ForceStop();

        var nodges = await _nodgeCreator.RetreiveGraph(query, _graphConfiguration);
        nodges = _dynamicFilterManager.Filter(nodges);

        DebugChrono.Instance.Start("UpdateGraph");
        await _graph.UpdateNodges(nodges);
        DebugChrono.Instance.Stop("UpdateGraph");

        _graph.CalculateMetrics();
        _graphSimulation.Run(_graph);
    }
}