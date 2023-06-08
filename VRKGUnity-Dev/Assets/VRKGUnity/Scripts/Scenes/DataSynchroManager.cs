using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DataSynchroManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolder;

    GraphDbRepository _repository;
    GraphDBAPI _graphDbAPI;

    JObject _data;

    private void Start()
    {
        _repository = _referenceHolder.SelectedGraphDbRepository;

        _graphDbAPI = _repository.GraphDBAPI;

        Invoke(nameof(SyncData), .2f);
    }


    private async void SyncData()
    {
        UpdateGraphDbFromOmeka();
        await UpdateGraphDbRepoFromGraphDbServer();
        await RetrieveDistantUri();

        var lifeSceneManager = _referenceHolder.LifeCycleSceneManagerSA.Value;

        lifeSceneManager.LoadScene(Scenes.KG);
    }

    private void UpdateGraphDbFromOmeka()
    {
        // TODO : Need to check the api
    }

    private async Task UpdateGraphDbRepoFromGraphDbServer()
    {
        await _repository.LoadChilds();

        var repoUris = _repository.GraphDbRepositoryUris;

        var json = await _graphDbAPI.Query("select * where { \r\n\t?s ?p ?o .\r\n}");
        _data = JsonConvert.DeserializeObject<JObject>(json);

        await repoUris.RetrieveNewUris(_data, _graphDbAPI);

    }

    private async Task RetrieveDistantUri()
    {
        var graphDistantUri = _repository.GraphDbRepositoryDistantUris;

        await graphDistantUri.RetrieveNames(_data);
    }
}
