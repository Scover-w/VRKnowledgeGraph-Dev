using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlasticGui.WorkspaceWindow.BranchExplorer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataSynchroManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    LoadingBarUI _loadingBarUI;

    [SerializeField]
    GraphSimulation _graphSimu;

    GraphDbRepository _graphRepo;
    GraphDBAPI _graphDbAPI;

    JObject _data;

    public ConcurrentQueue<LoadingDistantUriData> DataQueue = new ConcurrentQueue<LoadingDistantUriData>();

    bool _needUpdateLoadingBar = false;
    int _nbDistantUri;

    private void Start()
    {
        _graphRepo = _referenceHolderSo.SelectedGraphDbRepository;

        _graphDbAPI = _graphRepo.GraphDBAPI;

        Invoke(nameof(SyncData), .2f);
    }


    private void Update()
    {
        UpdateLoadingDistantUri();
    }

    private void UpdateLoadingDistantUri()
    {
        LoadingDistantUriData distantData = null;
        LoadingDistantUriData lastdistantData = null;

        while (DataQueue.TryDequeue(out distantData))
        {
            lastdistantData = distantData;

            if (!lastdistantData.IsFirst)
                continue;

            _nbDistantUri = lastdistantData.Value;
            _loadingBarUI.Refresh(.2f, "Retrieve Distant Uri 0/" + _nbDistantUri + ".");
        }

        if (lastdistantData == null)
            return;

        if (lastdistantData.IsFirst)
            return;

        float progression = lastdistantData.Value / (float)_nbDistantUri;

        _loadingBarUI.Refresh(.2f + Mathf.Lerp(0f, .6f, progression), "Retrieve Distant Uri " + lastdistantData.Value + "/" + _nbDistantUri + ".");

    }

    private async void SyncData()
    {
        _loadingBarUI.Refresh(0f, "Update GraphDb From Omeka");
        UpdateGraphDbFromOmeka();

        _loadingBarUI.Refresh(.1f, "Update GraphDbRepo From GraphDbServer");
        IReadOnlyDictionary<string, OntologyTree> ontoUris = null;

        await Task.Run(async () =>
        {
            ontoUris = await UpdateGraphDbRepoFromGraphDbServer();
        });

        _loadingBarUI.Refresh(.15f, "Block New Namespaces");

        await Task.Run(async () =>
        {
            await BlockOntologyToBeRetreived();
        });

        _loadingBarUI.Refresh(.2f, "Retrieve Distant Namespace");

        await Task.Run(async () =>
        {
            await RetrieveDistantUri(ontoUris);
        });


        _loadingBarUI.Refresh(.8f, "Calculate Max Bounds");
        // Calculate Max Bounds
        await CalculateMaxBound();


        _loadingBarUI.Refresh(1f, "Loading Scene");
        var lifeSceneManager = _referenceHolderSo.LifeCycleSceneManagerSA.Value;

        lifeSceneManager.LoadScene(Scenes.KG);
    }

    private void UpdateGraphDbFromOmeka()
    {
        // TODO : Need to check the api
        
    }

    private async Task<IReadOnlyDictionary<string, OntologyTree>> UpdateGraphDbRepoFromGraphDbServer()
    {
        await _graphRepo.LoadChilds();
        var repoUris = _graphRepo.GraphDbRepositoryUris;

        var json = await _graphDbAPI.SelectQuery("select * where { ?s ?p ?o .}");
        _data = JsonConvert.DeserializeObject<JObject>(json);

        await repoUris.RetrieveNewNamespaces(_data, _graphDbAPI);
        await repoUris.CreateOntologyTrees(_graphDbAPI);

        var readOntoTreeDict = repoUris.OntoTreeDict;

        return readOntoTreeDict;
    }

    private async Task BlockOntologyToBeRetreived()
    {
        var json = await _graphDbAPI.SelectQuery("select * where { ?s ?p ?o .}");
        var dataAfterAddedOntology = JsonConvert.DeserializeObject<JObject>(json);

        var dataNodeIds = _data.ExtractNodes();
        var dataAfterNodeIds = dataAfterAddedOntology.ExtractNodes();
        dataAfterNodeIds.RemoveNodes(dataNodeIds);


        await _graphRepo.GraphDbRepositoryUris.BlockNamespaceToBeRetrieved(dataAfterNodeIds);
        await _graphRepo.GraphDbRepositoryDistantUris.BlockNodesToBeRetrieved(dataAfterNodeIds);
    }

    private async Task RetrieveDistantUri(IReadOnlyDictionary<string, OntologyTree> readOntoTreeDict)
    {
        var graphDistantUri = _graphRepo.GraphDbRepositoryDistantUris;
        await graphDistantUri.RetrieveNames(_data, readOntoTreeDict, this);
    }

    private async Task CalculateMaxBound()
    {
        SPARQLAdditiveBuilder sparqlBuilder = new(_graphRepo.GraphDbRepositoryUris);
        string queryString = sparqlBuilder.Build();



        Nodges nodges = await NodgesHelper.RetreiveGraph(queryString, _graphRepo);
        NodgesSimuData nodgesSimuData = new NodgesSimuData(nodges);
        await _graphSimu.Run(nodgesSimuData);

        float maxDistance = GetMaxDistance(nodgesSimuData);
        _referenceHolderSo.MaxDistanceGraph = maxDistance;
    }

    private float GetMaxDistance(NodgesSimuData nodgesSimuData)
    {
        float maxSqrDistance = 0f;

        var nodesSimuData = nodgesSimuData.NodeSimuDatas;

        foreach (var nodeSimuData in nodesSimuData.Values)
        {
            var sqrDistance = nodeSimuData.Position.sqrMagnitude;

            if (sqrDistance > maxSqrDistance)
                maxSqrDistance = sqrDistance;
        }

        return Mathf.Sqrt(maxSqrDistance);
    }

}


public class LoadingDistantUriData
{
    public bool IsFirst;
    public int Value;

    public LoadingDistantUriData(int value, bool isFirst = false)
    {
        IsFirst = isFirst;
        Value = value;
    }
}