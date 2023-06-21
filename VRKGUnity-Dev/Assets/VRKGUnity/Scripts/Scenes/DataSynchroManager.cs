using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    int nbTriple = -1;

    private void Start()
    {
        _repository = _referenceHolder.SelectedGraphDbRepository;

        _graphDbAPI = _repository.GraphDBAPI;

        Invoke(nameof(SyncData), .2f);
    }


    private async void SyncData()
    {
        UpdateGraphDbFromOmeka();


        IReadOnlyDictionary<string, OntologyTree> ontoUris = null;

        await Task.Run(async () =>
        {
            ontoUris = await UpdateGraphDbRepoFromGraphDbServer();
        });


        await Task.Run(async () =>
        {
            await RetrieveDistantUri(ontoUris);
        });

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

        await graphDistantUri.RetrieveNames(_data, readOntoTreeDict);
    }
}
