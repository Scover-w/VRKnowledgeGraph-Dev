using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class LoadRepoUI : MonoBehaviour
{
    [SerializeField]
    MainMenuUI _mainMenuUI;

    [SerializeField]
    ScrollUI _scrollUI;

    [SerializeField]
    GameObject _scrollItemPf;

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

        if (repositories.Count == 0)
            return;

        foreach (GraphDbRepository repo in repositories)
        {
            AddRepository(repo);
        }
    }

    private RepositoryItemUI CreateItem(GraphDbRepository repository)
    {
        var go = Instantiate(_scrollItemPf, _scrollUI.ItemContainer);

        var repoItemUI = go.GetComponent<RepositoryItemUI>();
        repoItemUI.Load(repository, this);

        var colliders = repoItemUI.Colliders;

        var scollItem = new ScrollItem(go.GetComponent<RectTransform>(), colliders);
        repoItemUI.ScrollItem = scollItem;
        _scrollUI.AddItem(scollItem);

        return repoItemUI;
    }


    public void SelecRepoFromItem(GraphDbRepository repositoryClicked)
    {
        SelectRepo(repositoryClicked);
    }

    public void NewRepositoryClick()
    {
        DisplayPage(LoadRepoPage.NewModifyRepo);

        _repoIdInput.Value = "cap44";
        _serverUrlinput.Value = "http://localhost:7200/";
    }


    private void TryCreateGraphDb()
    {
        string repoId = _repoIdInput.Value;
        string repoServerUrl = _serverUrlinput.Value;

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
        _selectedRepository = graphDbRepository;
        foreach (var kvp in _repositoriesDict)
        {
            bool isSelected = kvp.Key == _selectedRepository;
            kvp.Value.Select(isSelected);

            if (!isSelected)
                continue;

            _mainMenuUI.RepoSelected(_selectedRepository);
        }
    }

    #region Click
    public void ConfirmCreateRepoClick()
    {
        TryCreateGraphDb();
    }
    
    public void CancelConfirmationClick()
    {
        DisplayPage(LoadRepoPage.RepoList);
    }

    public void ConfirmDeletionClick()
    {
        if(!_repositoriesDict.TryGetValue(_selectedRepository, out RepositoryItemUI itemUI))
        {
            Debug.LogWarning("Unexpected to not find the associated itemUI.");
        }
        else
        {
            _repositoriesDict.Remove(_selectedRepository);
            _scrollUI.RemoveItem(itemUI.ScrollItem);
        }


        _graphDbRepositories.Remove(_selectedRepository); 

        _selectedRepository = null;
        SelectRepo(null);

        _mainMenuUI.RepoSelected(null);

        DisplayPage(LoadRepoPage.RepoList);
    }

    public void CancelCreationClick()
    {
        DisplayPage(LoadRepoPage.NewModifyRepo);
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

    private void AddRepository(GraphDbRepository repo)
    {
        RepositoryItemUI itemUI = CreateItem(repo);
        _repositoriesDict.Add(repo, itemUI);
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
