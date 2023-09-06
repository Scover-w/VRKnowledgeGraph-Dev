using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DataSynchroManager : MonoBehaviour
{
    [SerializeField]
    GraphConfigurationContainerSO _graphConfigContainerSo;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    LoadingBarUI _loadingBarUI;

    [SerializeField]
    GraphSimulation _graphSimu;

    [SerializeField]
    TMP_Text _logTxt;

    GraphDbRepository _graphRepo;
    GraphDBAPI _graphDbAPI;

    GraphConfiguration _graphConfiguration;

    UnityMainThreadDispatcher _unityThreadDispatcherInstance;

    JObject _data;
    StringBuilder _logSb = new StringBuilder();

    public ConcurrentQueue<LoadingDistantUriData> DataQueue = new();

    int _nbDistantUri;

    private async void Start()
    {
        _graphRepo = _referenceHolderSo.SelectedGraphDbRepository;
        _graphDbAPI = _graphRepo.GraphDBAPI;

        _graphConfiguration = await _graphConfigContainerSo.GetGraphConfiguration();
        Invoke(nameof(SyncData), .2f);
    }


    private void Update()
    {
        UpdateLoadingDistantUri();
    }

    private void UpdateLoadingDistantUri()
    {
        LoadingDistantUriData lastdistantData = null;

        while (DataQueue.TryDequeue(out LoadingDistantUriData distantData))
        {
            lastdistantData = distantData;

            if (!lastdistantData.IsFirst)
                continue;

            _nbDistantUri = lastdistantData.Value;
            _loadingBarUI.Refresh(.2f, "Récupération des Uris distantes 0/" + _nbDistantUri + ".");
        }

        if (lastdistantData == null)
            return;

        if (lastdistantData.IsFirst)
            return;

        float progression = lastdistantData.Value / (float)_nbDistantUri;

        _loadingBarUI.Refresh(.2f + Mathf.Lerp(0f, .6f, progression), "Récupération des Uris distantes " + lastdistantData.Value + "/" + _nbDistantUri + ".");

    }

    private async void SyncData()
    {
        _unityThreadDispatcherInstance = UnityMainThreadDispatcher.Instance();

        _loadingBarUI.Refresh(0f, "Mise à jour des données Omeka dans GaphDb");
        UpdateGraphDbFromOmeka();

        _loadingBarUI.Refresh(.1f, "Mise à jour des données GraphDb en local");
        IReadOnlyDictionary<string, OntologyTree> ontoUris = null;


        ontoUris = await UpdateLocalRepoFromGraphDbServer();


        _loadingBarUI.Refresh(.2f, "Récupération des espaces de noms distants");

        await Task.Run(async () =>
        {
            await RetrieveDistantUri(ontoUris);
        });


        _loadingBarUI.Refresh(1f, "Chargement de la scène Graphe");
        var lifeSceneManager = _referenceHolderSo.LifeCycleSceneManagerSA.Value;

        lifeSceneManager.LoadScene(Scenes.KG);
    }

    private void UpdateGraphDbFromOmeka()
    {
        // TODO : Need to check the api
        
    }

    private async Task<IReadOnlyDictionary<string, OntologyTree>> UpdateLocalRepoFromGraphDbServer()
    {
        await _graphRepo.LoadChilds();
        var repoUris = _graphRepo.GraphDbRepositoryNamespaces;

        string queryString = new SPARQLAdditiveBuilder().Build();

        var json = await _graphDbAPI.SelectQuery(queryString, true);

        _data = await JsonConvertHelper.DeserializeObjectAsync<JObject>(json);
        Debug.Log(_data);
        await repoUris.RetrieveNewNamespaces(_data, _graphDbAPI, this);
        await repoUris.CreateOntologyTrees(_graphDbAPI, this);

        var readOntoTreeDict = repoUris.OntoTreeDict;

        return readOntoTreeDict;
    }

    private async Task RetrieveDistantUri(IReadOnlyDictionary<string, OntologyTree> readOntoTreeDict)
    {
        var graphDistantUri = _graphRepo.GraphDbRepositoryDistantUris;
        await graphDistantUri.RetrieveNames(_data, readOntoTreeDict, this, _graphRepo.GraphDbRepositoryNamespaces);
    }

    public void AddLog(string logMessage)
    {
        _unityThreadDispatcherInstance.Enqueue(() => AddLogInUnityThread(logMessage));
    }

    private void AddLogInUnityThread(string logMessage)
    {
        var newLine = Environment.NewLine;
        _logSb.Insert(0, logMessage + newLine);

        string txt = _logSb.ToString();

        string[] lines = txt.Split(new[] { newLine }, StringSplitOptions.None);
        if (lines.Length > 12)
        {
            int lineLength = lines[lines.Length - 2].Length;
            int lastLineIndex = txt.LastIndexOf(newLine) - lineLength;
            if (lastLineIndex >= 0)
            {
                _logSb.Remove(lastLineIndex, lineLength);
            }
        }

        _logTxt.text = _logSb.ToString();
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