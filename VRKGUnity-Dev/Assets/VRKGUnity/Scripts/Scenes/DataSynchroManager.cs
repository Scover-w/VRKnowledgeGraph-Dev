using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataSynchroManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolder;

    [SerializeField]
    LoadingBarUI _loadingBarUI;

    GraphDbRepository _repository;
    GraphDBAPI _graphDbAPI;

    JObject _data;

    public ConcurrentQueue<LoadingDistantUriData> DataQueue = new ConcurrentQueue<LoadingDistantUriData>();

    bool _needUpdateLoadingBar = false;
    int _nbDistantUri;

    private void Start()
    {
        _repository = _referenceHolder.SelectedGraphDbRepository;

        _graphDbAPI = _repository.GraphDBAPI;

        Invoke(nameof(SyncData), .2f);
    }


    private void Update()
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

        _loadingBarUI.Refresh(.2f + Mathf.Lerp(0f,.8f, progression), "Retrieve Distant Uri "  + lastdistantData.Value + "/" + _nbDistantUri + ".");

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

        _loadingBarUI.Refresh(.2f, "Retrieve Distant Uri");
        await RetrieveDistantUri(ontoUris);

        _loadingBarUI.Refresh(1f, "Loading Scene");
        var lifeSceneManager = _referenceHolder.LifeCycleSceneManagerSA.Value;

        lifeSceneManager.LoadScene(Scenes.KG);
    }

    private void UpdateGraphDbFromOmeka()
    {
        // TODO : Need to check the api
        
    }

    private async Task<IReadOnlyDictionary<string, OntologyTree>> UpdateGraphDbRepoFromGraphDbServer()
    {
        await _repository.LoadChilds();

        var repoUris = _repository.GraphDbRepositoryUris;

        var json = await _graphDbAPI.SelectQuery("select * where { ?s ?p ?o .}");
        _data = JsonConvert.DeserializeObject<JObject>(json);

        await repoUris.RetrieveNewUris(_data, _graphDbAPI);
        await repoUris.CreateOntologyTrees(_graphDbAPI);

        var readOntoTreeDict = repoUris.OntoTreeDict;
        return readOntoTreeDict;
    }

    private async Task RetrieveDistantUri(IReadOnlyDictionary<string, OntologyTree> readOntoTreeDict)
    {
        var graphDistantUri = _repository.GraphDbRepositoryDistantUris;

        await graphDistantUri.RetrieveNames(_data, readOntoTreeDict, this);
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