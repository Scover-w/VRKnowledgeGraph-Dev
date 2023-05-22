using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public NodeUriRetriever NodeUriRetriever { get { return _nodeUriRetriever; } }
    public Graph Graph { get { return _graph; } }

    [SerializeField]
    GraphSimulation _graphSimulation;

    [SerializeField]
    NodgeCreator _nodgeCreator;

    [SerializeField]
    GraphUI _graphUI;

    [Header("Configuration")]
    public GraphConfiguration GraphConfiguraton;

    Graph _graph;
    SPARQLAdditiveBuilder _sparqlBuilder;
    DynamicFilterManager _dynamicFilterManager;

    NodeUriRetriever _nodeUriRetriever;

    void Start()
    {
        _dynamicFilterManager = new();
        _nodeUriRetriever = new();
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
        var nodges = await _nodgeCreator.RetreiveGraph(queryString, GraphConfiguraton);

        Debug.Log(nodges.NodesDicId.Count);

        _graph = new Graph(this, _graphUI, GraphConfiguraton,nodges);
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

        var nodges = await _nodgeCreator.RetreiveGraph(query, GraphConfiguraton);
        nodges = _dynamicFilterManager.Filter(nodges);

        DebugChrono.Instance.Start("UpdateGraph");
        await _graph.UpdateNodges(nodges);
        DebugChrono.Instance.Stop("UpdateGraph");

        _graphSimulation.Run(_graph);
    }
}
