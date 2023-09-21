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
    InputUI _usernameInput;

    [SerializeField]
    InputUI _passwordInput;


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
    TMP_InputField _usernamePCInput;

    [SerializeField]
    TMP_InputField _passwordPCInput;

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

        ResetSetInputValue();
    }


    private void TryCreateGraphDb()
    {
        (string graphDbServerUrl, string graphDbRepoId, string username, string password) = GetInputValue();

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

        if (username.Length == 0)
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "Veuillez fournir un nom d'utilisateur.";
            DisplayPage(LoadRepoPage.InformationDialog);
            return;
        }

        if (password.Length == 0)
        {
            _infoTitleTxt.text = "Impossible de créer le dépôt";
            _infoTxt.text = "Veuillez fournir un mot de passe";
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


        TryConnectRepo(graphDbRepoId, graphDbServerUrl, username, password);
    }

    private async void TryConnectRepo(string graphDbRepoId, string graphDbServerUrl, string username, string password)
    {
        DisplayPage(LoadRepoPage.LoadingDialog);
        RepositoryStatus repoStatus = await GraphDBAPI.DoRepositoryExist(graphDbServerUrl, graphDbRepoId, username, password);


        switch (repoStatus)
        {
            case RepositoryStatus.ExistButUnreadable:
                _infoTitleTxt.text = "Impossible de lire le dépôt";
                _infoTxt.text = "Le dépôt ne possède pas les autorisations de lecture requises pour communiquer avec celui-ci.";
                _previousPage = LoadRepoPage.NewModifyRepo;
                DisplayPage(LoadRepoPage.InformationDialog, false);
                return;
            case RepositoryStatus.Nonexistent:
                _infoTitleTxt.text = "Dépôt inexistant";
                _infoTxt.text = "Le dépôt n'existe pas dans la base de données.";
                _previousPage = LoadRepoPage.NewModifyRepo;
                DisplayPage(LoadRepoPage.InformationDialog, false);
                return;
            case RepositoryStatus.Unauthorized:
                _infoTitleTxt.text = "Connection au dépôt non autorisé";

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    _infoTxt.text = "Veuillez renseigner des identifiants de connections.";
                else
                    _infoTxt.text = "Vous n'avez pas les droits requis pour accéder à ce dépôt.";

                _previousPage = LoadRepoPage.NewModifyRepo;
                DisplayPage(LoadRepoPage.InformationDialog, false);
                return;
            case RepositoryStatus.CouldntConnect:
                _infoTitleTxt.text = "Impossible de se connecter à la base de données";
                _infoTxt.text = "Impossible de se connecter à la base de donnée. Veuillez vous assurer que vous disposez d'une connexion Internet ouque le serveur est en ligne.";
                _previousPage = LoadRepoPage.NewModifyRepo;
                DisplayPage(LoadRepoPage.InformationDialog, false);
                return;
            case RepositoryStatus.Failed:
                _infoTitleTxt.text = "Impossible de vérifier le status du dépôt";
                _infoTxt.text = "Le serveur a retourné une erreur.";
                _previousPage = LoadRepoPage.NewModifyRepo;
                DisplayPage(LoadRepoPage.InformationDialog, false);
                return;
        }


        string encryptedUsername = null;
        string encryptedPassword = null;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            string key = SystemInfo.deviceUniqueIdentifier + SystemInfo.graphicsDeviceID.ToString();
            encryptedUsername = CryptographyHelper.Encrypt(username, key);
            encryptedPassword = CryptographyHelper.Encrypt(password, key);
        }
        
        GraphDbRepository dbRepository = new(graphDbServerUrl, graphDbRepoId, encryptedUsername, encryptedPassword);

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
        if(_selectedRepository == graphDbRepository) // Recliking on the same repo display the delete confirmation
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

    

    private void ResetSetInputValue()
    {
        if(_graphDbServerUrlInput != null)
            _graphDbServerUrlInput.Value = "";

        if(_graphDbRepoIdInput != null)
            _graphDbRepoIdInput.Value = "";

        if(_usernameInput != null)
            _usernameInput.Value = "";

        if (_passwordInput != null)
            _passwordInput.Value = "";

#if UNITY_EDITOR
        if(_serverUrlPCInput != null)
            _serverUrlPCInput.text = "";

        if (_repoIdPCInput != null)
            _repoIdPCInput.text = "";

        if (_usernamePCInput != null)
            _usernamePCInput.text = "";

        if (_passwordPCInput != null)
            _passwordPCInput.text = "";
#endif
    }

    private (string serverUrl, string repoId, string username, string password) GetInputValue()
    {
        string repoId = "";
        string serverUrl = "";
        string username = "";
        string password = "";

        if (_graphDbServerUrlInput != null)
            serverUrl = _graphDbServerUrlInput.Value;

        if (serverUrl[serverUrl.Length - 1] != '/')
        {
            serverUrl += "/";
            _graphDbServerUrlInput.Value += "/";
        }

        if (_graphDbRepoIdInput != null)
            repoId = _graphDbRepoIdInput.Value;

        if (_usernameInput != null)
            username = _usernameInput.Value;

        if (_passwordInput != null)
            password = _passwordInput.Value;

#if UNITY_EDITOR
        if (_serverUrlPCInput != null)
            serverUrl = _serverUrlPCInput.text;

        if (_repoIdPCInput != null)
            repoId = _repoIdPCInput.text;

        if (_usernamePCInput != null)
            username = _usernamePCInput.text;

        if (_passwordPCInput != null)
            password = _passwordPCInput.text;
#endif

        return (serverUrl, repoId, username, password);
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
