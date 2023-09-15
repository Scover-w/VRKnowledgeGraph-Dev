using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LoadRepoUI : MonoBehaviour
{
    [SerializeField]
    MainMenuUI _mainMenuUI;

    [SerializeField]
    GameObject _repoListGo;

    [SerializeField]
    GameObject _newRepoGo;

    [SerializeField]
    GameObject _confirmationAskGo;

    [SerializeField]
    GameObject _informationDialogGo;

    [SerializeField]
    GameObject _loadingDialogGo;

    [SerializeField]
    TMP_Text _infoTitleTxt;

    [SerializeField]
    TMP_Text _infoTxt;


    [Header("VR")]
    [SerializeField]
    ScrollViewUI _scrollUI;

    [SerializeField]
    GameObject _scrollItemPf;

    [SerializeField]
    InputUI _graphDbRepoIdInput;

    [SerializeField]
    InputUI _graphDbServerUrlInput;

    [SerializeField]
    InputUI _omekaUrlInput;


    [Header("PC")]
#if UNITY_EDITOR
    [SerializeField]
    RectTransform _scrollRectPC;

    [SerializeField]
    GameObject _scrollItemPCPf;

    [SerializeField]
    TMP_InputField _repoIdPCInput;

    [SerializeField]
    TMP_InputField _serverUrlPCInput;

    [SerializeField]
    TMP_InputField _omekaUrlPCInput;

    Dictionary<GraphDbRepository, RepositoryItemPCUI> _repositoriesDictPC;
#endif

    Dictionary<GraphDbRepository, RepositoryItemUI> _repositoriesDict;
    GraphDbRepository _selectedRepository;

    GraphDbRepositories _graphDbRepositories;

    LoadRepoPage _previousPage;
    LoadRepoPage _displayedPage;

    private void Start()
    {
        LoadRepositories();
    }



    private void OnEnable()
    {
        DisplayPage(LoadRepoPage.RepoList);
    }


    private async void LoadRepositories()
    {
        _graphDbRepositories = await GraphDbRepositories.Load();
        var repositories = _graphDbRepositories.Repositories;

        _repositoriesDict = new();

#if UNITY_EDITOR
        _repositoriesDictPC = new();
#endif

        if (repositories.Count == 0)
            return;

        foreach (GraphDbRepository repo in repositories)
        {
            AddRepository(repo);
        }
    }

    private void AddRepository(GraphDbRepository repo)
    {
        if (_scrollUI != null)
        {
            RepositoryItemUI itemUI = CreateItem(repo);
            _repositoriesDict.Add(repo, itemUI);
        }
#if UNITY_EDITOR
        else if(_scrollRectPC != null)
        {
            RepositoryItemPCUI itemUI = CreateItemPC(repo);
            _repositoriesDictPC.Add(repo, itemUI);
        }
#endif
    }

    private RepositoryItemUI CreateItem(GraphDbRepository repository)
    {
        var go = Instantiate(_scrollItemPf, _scrollUI.ItemContainer);

        var repoItemUI = go.GetComponent<RepositoryItemUI>();
        repoItemUI.Load(repository, this);



        _scrollUI.AddItem(repoItemUI.ScrollItemUI);

        return repoItemUI;
    }

#if UNITY_EDITOR
    private RepositoryItemPCUI CreateItemPC(GraphDbRepository repository)
    {
        var go = Instantiate(_scrollItemPCPf, _scrollRectPC);

        var repoItemUI = go.GetComponent<RepositoryItemPCUI>();
        repoItemUI.Load(repository, this);

        return repoItemUI;
    }
#endif


    public void SelectRepoFromItem(GraphDbRepository repositoryClicked)
    {
        SelectRepo(repositoryClicked);
    }

    public void NewRepositoryClick()
    {
        DisplayPage(LoadRepoPage.NewModifyRepo);

        SetInputValue("cap44", "http://localhost:7200/", "https://epotec.univ-nantes.fr/api/items?item_set_id=26705");
        //SetInputValue("", "", "");
    }


    private void TryCreateGraphDb()
    {
        (string graphDbRepoId, string graphDbServerUrl, string omekaUrl) = GetInputValue();

        if(graphDbRepoId.Length == 0)
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "Veuillez fournir un nom de dépôt GraphDB.";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }

        if(graphDbServerUrl.Length == 0)
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "Veuillez fournir l'URL du serveur GraphDB.";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }

        if (omekaUrl.Length == 0)
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "Veuillez fournir un URL Omeka-S.";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }

        if (!(graphDbServerUrl.Contains("http://") || graphDbServerUrl.Contains("https://")) )
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "L'URL du serveur est erronée. Veuillez vous assurer qu'elle respecte le format d'URL standard.";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }

        if (!(omekaUrl.Contains("http://") || omekaUrl.Contains("https://")))
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "L'URL d'Omeka-S est erronée. Veuillez vous assurer qu'elle respecte le format d'URL standard.";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }


        TryConnectRepo(graphDbRepoId, graphDbServerUrl, omekaUrl);
    }

    private async void TryConnectRepo(string graphDbRepoId, string graphDbServerUrl, string omekaUrl)
    {
        DisplayPage(LoadRepoPage.LoadingDialog);
        bool couldConnectToGraphDb = await GraphDBAPI.DoRepositoryExist(graphDbServerUrl, graphDbRepoId);

        if(!couldConnectToGraphDb)
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "Impossible de se connecter au dépôt. Veuillez vous assurer que vous disposez d'une connexion Internet active et que les informations sont correctes.";
            _previousPage = LoadRepoPage.NewModifyRepo;
            DisplayPage(LoadRepoPage.InformationDialog, false);
            return;
        }

        string result = await HttpHelper.Retrieve(omekaUrl);

        if(result.Length == 0)
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "Impossible de se connecter a Omeka-S. Veuillez vous assurer que vous disposez d'une connexion Internet active et que les informations sont correctes.";
            _previousPage = LoadRepoPage.NewModifyRepo;
            DisplayPage(LoadRepoPage.InformationDialog, false);
            return;
        }

        GraphDbRepository dbRepository = new(graphDbServerUrl, graphDbRepoId, omekaUrl);
        _graphDbRepositories.Add(dbRepository);
        _graphDbRepositories.Select(dbRepository);

        AddRepository(dbRepository);
        SelectRepo(dbRepository);

        _mainMenuUI.DisplayMainMenu();
    }

    private void SelectRepo(GraphDbRepository graphDbRepository)
    {
        if(_scrollUI != null)
            SelectRepoVR(graphDbRepository);
#if UNITY_EDITOR
        else if(_scrollRectPC != null)
            SelectRepoPC(graphDbRepository);
#endif
    }

    private void SelectRepoVR(GraphDbRepository graphDbRepository)
    {
        if(_selectedRepository == graphDbRepository)
        {
            TryDeleteRepo();
            return;
        }

        _selectedRepository = graphDbRepository;

        _mainMenuUI.RepoSelected(_selectedRepository);
    }

#if UNITY_EDITOR
    private void SelectRepoPC(GraphDbRepository graphDbRepository)
    {
        _selectedRepository = graphDbRepository;
        foreach (var kvp in _repositoriesDictPC)
        {
            bool isSelected = kvp.Key == _selectedRepository;
            kvp.Value.Select(isSelected);

            if (!isSelected)
                continue;

            _mainMenuUI.RepoSelected(_selectedRepository);
        }
    }
#endif

    #region Click
    public void ConfirmCreateRepoClick()
    {
        TryCreateGraphDb();
    }
    
    public void CancelConfirmationClick()
    {
        DisplayPage(LoadRepoPage.RepoList);
    }

    public async void ConfirmDeletionClick()
    {

        if (_scrollUI != null)
            await ConfirmDeletion();
#if UNITY_EDITOR
        else if (_scrollRectPC != null)
            await ConfirmDeletionPC();
#endif
    }

    private async Task ConfirmDeletion()
    {
        if (!_repositoriesDict.TryGetValue(_selectedRepository, out RepositoryItemUI itemUI))
        {
            Debug.LogWarning("Unexpected to not find the associated itemUI.");
        }
        else
        {
            _repositoriesDict.Remove(_selectedRepository);
            _scrollUI.RemoveItem(itemUI.ScrollItemUI);
        }


        await _graphDbRepositories.Remove(_selectedRepository);

        _selectedRepository = null;
        SelectRepo(null);

        _mainMenuUI.RepoSelected(null);

        DisplayPage(LoadRepoPage.RepoList);
    }

#if UNITY_EDITOR
    private async Task ConfirmDeletionPC()
    {
        if (!_repositoriesDictPC.TryGetValue(_selectedRepository, out RepositoryItemPCUI itemUI))
        {
            Debug.LogWarning("Unexpected to not find the associated itemUI.");
        }
        else
        {
            _repositoriesDictPC.Remove(_selectedRepository);
            Destroy(itemUI.gameObject);
        }


        await _graphDbRepositories.Remove(_selectedRepository);

        _selectedRepository = null;
        SelectRepo(null);

        _mainMenuUI.RepoSelected(null);

        DisplayPage(LoadRepoPage.RepoList);
    }
#endif

    public void CancelCreationClick()
    {
        DisplayPage(LoadRepoPage.RepoList);
    }

    public void CancelDeletionClick()
    {
        DisplayPage(LoadRepoPage.NewModifyRepo);
    }

    public void CloseInfoClick()
    {
        DisplayPage(_previousPage);
    }

    public void TryDeleteRepo()
    {
        DisplayPage(LoadRepoPage.ConfirmationAsk);
    }
    #endregion

    #region Tools
    private void DisplayPage(LoadRepoPage page, bool setPreviousPage = true)
    {
        if(setPreviousPage)
            _previousPage = _displayedPage;

        _displayedPage = page;
        _repoListGo.SetActive(page == LoadRepoPage.RepoList);
        _newRepoGo.SetActive(page == LoadRepoPage.NewModifyRepo);
        _confirmationAskGo.SetActive(page == LoadRepoPage.ConfirmationAsk);
        _informationDialogGo.SetActive(page == LoadRepoPage.InformationDialog);
        _loadingDialogGo.SetActive(page == LoadRepoPage.LoadingDialog);
    }

    

    private void SetInputValue(string graphDbRepoId, string graphDbServerUrl, string omekaUrl)
    {
        if(_graphDbRepoIdInput != null)
            _graphDbRepoIdInput.Value = graphDbRepoId;

        if(_graphDbServerUrlInput != null)
            _graphDbServerUrlInput.Value = graphDbServerUrl;

        if(_omekaUrlInput != null)
            _omekaUrlInput.Value = omekaUrl;

#if UNITY_EDITOR
        if(_repoIdPCInput != null)
            _repoIdPCInput.text = graphDbRepoId;

        if(_serverUrlPCInput != null)
            _serverUrlPCInput.text = graphDbServerUrl;

        if (_omekaUrlPCInput != null)
            _omekaUrlPCInput.text = omekaUrl;
#endif
    }

    private (string repoId, string serverUrl, string omekaUrl) GetInputValue()
    {
        string repoId = "";
        string serverUrl = "";
        string omekaUrl = "";

        if (_graphDbRepoIdInput != null)
            repoId = _graphDbRepoIdInput.Value;

        if (_graphDbServerUrlInput != null)
            serverUrl = _graphDbServerUrlInput.Value;

        if (_omekaUrlInput != null)
            omekaUrl = _omekaUrlInput.Value;

#if UNITY_EDITOR
        if (_repoIdPCInput != null)
            repoId = _repoIdPCInput.text;

        if (_serverUrlPCInput != null)
            serverUrl = _serverUrlPCInput.text;
        
        if (_omekaUrlPCInput != null)
            omekaUrl = _omekaUrlPCInput.text;
#endif

        return (repoId, serverUrl, omekaUrl);
    }
    #endregion

    private enum LoadRepoPage
    {
        RepoList,
        NewModifyRepo,
        ConfirmationAsk,
        InformationDialog,
        LoadingDialog
    }
}
