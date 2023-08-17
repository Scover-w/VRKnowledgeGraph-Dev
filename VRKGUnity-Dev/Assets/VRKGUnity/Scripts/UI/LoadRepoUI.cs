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
    ScrollViewUI _scrollUI;

#if UNITY_EDITOR
    [SerializeField]
    RectTransform _scrollRectPC;
#endif

    [SerializeField]
    GameObject _scrollItemPf;

#if UNITY_EDITOR
    [SerializeField]
    GameObject _scrollItemPCPf;
#endif

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
    InputUI _repoIdInput;

    [SerializeField]
    InputUI _serverUrlinput;

#if UNITY_EDITOR
    [SerializeField]
    TMP_InputField _repoIdPCInput;

    [SerializeField]
    TMP_InputField _serverUrlPCInput;

    Dictionary<GraphDbRepository, RepositoryItemPCUI> _repositoriesDictPC;
#endif

    [SerializeField]
    TMP_Text _infoTitleTxt;

    [SerializeField]
    TMP_Text _infoTxt;

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

        SetInputValue("cap44", "http://localhost:7200/");
    }


    private void TryCreateGraphDb()
    {
        (string repoId, string repoServerUrl) = GetInputValue();

        if(repoId.Length == 0)
        {
            _infoTitleTxt.text = "Couldn't create the repository";
            _infoTxt.text = "Please provide a repository id.";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }

        if(repoServerUrl.Length == 0)
        {
            _infoTitleTxt.text = "Couldn't create the repository";
            _infoTxt.text = "Please provide a server url.";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }

        if(  !(repoServerUrl.Contains("http://") || repoServerUrl.Contains("https://")) )
        {
            _infoTitleTxt.text = "Couldn't create the repository";
            _infoTxt.text = "The server url is wrong. Please ensure it adheres to the standard URL format";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }


        TryConnectRepo(repoId, repoServerUrl);
    }

    private async void TryConnectRepo(string repoId, string serverUrl)
    {
        DisplayPage(LoadRepoPage.LoadingDialog);
        bool couldConnect = await GraphDBAPI.DoRepositoryExist(serverUrl, repoId);

        if(!couldConnect)
        {
            _infoTitleTxt.text = "Couldn't create the repository";
            _infoTxt.text = "Unable to connect to the repository. Please ensure you have an active internet connection and that the repository details are correct.";
            _previousPage = LoadRepoPage.NewModifyRepo;
            DisplayPage(LoadRepoPage.InformationDialog, false);
            return;
        }

        GraphDbRepository dbRepository = new(serverUrl, repoId);
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

    

    private void SetInputValue(string repoId, string serverUrl)
    {
        if(_repoIdInput != null)
            _repoIdInput.Value = repoId;

        if(_serverUrlinput != null)
            _serverUrlinput.Value = serverUrl;

#if UNITY_EDITOR
        if(_repoIdPCInput != null)
            _repoIdPCInput.text = repoId;

        if(_serverUrlPCInput != null)
            _serverUrlPCInput.text = serverUrl;
#endif
    }

    private (string repoId, string serverUrl) GetInputValue()
    {
        string repoId = "";
        string serverUrl = "";

        if (_repoIdInput != null)
            repoId = _repoIdInput.Value;

        if (_serverUrlinput != null)
            serverUrl = _serverUrlinput.Value;

#if UNITY_EDITOR
        if (_repoIdPCInput != null)
            repoId = _repoIdPCInput.text;

        if (_serverUrlPCInput != null)
            serverUrl = _serverUrlPCInput.text;
#endif

        return (repoId, serverUrl);
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
