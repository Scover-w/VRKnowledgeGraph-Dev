using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

    [SerializeField]
    TextAsset _defaultPrefixs;

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

        _loadingBarUI.Refresh(0f, "Vérification de la connexion internet");

        await WaitConnectionToInternet();

        _loadingBarUI.Refresh(0f, "Vérification de la connexion au serveur GraphDb");
        await WaitRepositoryConnection();

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

    private async Task WaitConnectionToInternet()
    {
        bool isConnectedToInternet = false;
        bool isFirstTest = true;

        while (!isConnectedToInternet)
        {

            if (!isFirstTest)
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(5000);
                });

            }

            isConnectedToInternet = await HttpHelper.IsConnectedToInternet();     

            if(!isConnectedToInternet)
            {
                _loadingBarUI.Refresh(0f, "Connexion échouée. Veuillez vous connecter à internet.");
                isFirstTest = false;
            }
        }
    }

    private async Task WaitRepositoryConnection()
    {
        bool isConnectedToRepo = false;
        bool isFirstTest = true;

        while (!isConnectedToRepo)
        {

            if (!isFirstTest)
            {
                await Task.Run(() =>
                {
                    Thread.Sleep(5000);
                });

            }

            var status = await _graphDbAPI.DoRepositoryExist();
            isFirstTest = false;

            switch (status)
            {
                case RepositoryStatus.Exist:
                    isConnectedToRepo = true;
                    break;
                case RepositoryStatus.ExistButUnreadable:
                    _loadingBarUI.Refresh(0f, "Le dépôt ne possède pas les autorisations de lecture requises pour communiquer avec celui-ci. Veuillez modifier vos droits ou changer d'identifants.");
                    break;
                case RepositoryStatus.Nonexistent:
                    _loadingBarUI.Refresh(0f, "Le dépôt n'existe pas dans la base de données.");
                    break;
                case RepositoryStatus.Unauthorized:
                    _loadingBarUI.Refresh(0f, "Connection au dépôt non autorisé. Veuillez modifier vos droits ou changer d'identifants.");
                    break;
                case RepositoryStatus.CouldntConnect:
                    _loadingBarUI.Refresh(0f, "Impossible de se connecter à la base de donnée. Veuillez vous assurer que le serveur est en ligne.");
                    break;
                case RepositoryStatus.Failed:
                    _loadingBarUI.Refresh(0f, "Connexion échouée.");
                    break;
            }
        }
    }

    private async Task<IReadOnlyDictionary<string, OntologyTree>> UpdateLocalRepoFromGraphDbServer()
    {
        await _graphRepo.LoadChilds();
        var repoUris = _graphRepo.GraphDbRepositoryNamespaces;

        string queryString = new SPARQLAdditiveBuilder().Build();

        var json = await _graphDbAPI.SelectQuery(queryString, true);
        Debug.Log("UpdateLocalRepoFromGraphDbServer json : ");
        Debug.Log(json);
        _data = await JsonConvertHelper.DeserializeObjectAsync<JObject>(json);
        Debug.Log(_data);
        await repoUris.RetrieveNewNamespaces(_data, _graphDbAPI, this, _defaultPrefixs);
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