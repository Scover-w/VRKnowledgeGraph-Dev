using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphDbRepositoriesUI : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    GameObject _repoCanvasGo;

    [SerializeField]
    RectTransform _parentRepositoriesRect;

    [SerializeField]
    GameObject _repositoryPf;

    [SerializeField]
    RectTransform _createRepoRect;

    [SerializeField]
    TMP_InputField _serverURLInput;

    [SerializeField]
    TMP_InputField _repositoryIdInput;

    [SerializeField]
    Button _startBtn;

    GraphDbRepositories _graphDbRespositories;
    GraphDbRepository _selectedRepository;


    List<Button> _repoBtns;


    AppManager _appManager;

    // Start is called before the first frame update
    async void Start()
    {

        _appManager = _referenceHolderSO.AppManagerSA.Value;

        _repoCanvasGo.SetActive(true);

        _graphDbRespositories = await GraphDbRepositories.Load();

        Debug.Log("Startrepo");

        if(_graphDbRespositories.Count > 0)
        {
            Debug.Log("Count " + _graphDbRespositories.Count);
            SelectRepo(_graphDbRespositories.AutoSelect());
        }
        else
            _startBtn.interactable = false;

        LoadRepositories();
        int lastSelectedId = _graphDbRespositories.LastSelectedId;
        StyleRepoBtn(lastSelectedId == -1 ? 0 : lastSelectedId);
    }

    private void LoadRepositories()
    {
        _repoBtns = new();
        var repos = _graphDbRespositories.Repositories;

        foreach (var repo in repos) 
        {
            CreateRepoBtn(repo);
        }

        StyleRepoBtn(-1);
    }

    public void SelectRepoClick(int id)
    {
        Debug.Log("SelectRepoClick " + id);

        SelectRepo(_graphDbRespositories.Select(id));

        StyleRepoBtn(id);
    }

    public void NewRepositoryClick()
    {
        _serverURLInput.text = "";
        _repositoryIdInput.text = "";
        _createRepoRect.gameObject.SetActive(true);
    }

    public void CreateRepositoryClick()
    {
        var serverUrl = _serverURLInput.text;
        var repositoryId = _repositoryIdInput.text;

        GraphDbRepository newRepo = new(serverUrl, repositoryId);

        CreateRepoBtn(newRepo);

        _createRepoRect.gameObject.SetActive(false);

        _graphDbRespositories.Add(newRepo);
        _graphDbRespositories.Select(newRepo);

        SelectRepo(newRepo);

        StyleRepoBtn(_repoBtns.Count - 1);

    }

    private void CreateRepoBtn(GraphDbRepository repo)
    {
        int id = _parentRepositoriesRect.childCount;

        var cn = Instantiate(_repositoryPf, _parentRepositoriesRect);

        var btn = cn.GetComponent<Button>();
        _repoBtns.Add(btn);
        btn.onClick.AddListener(() => SelectRepoClick(id));

        var cnTf = cn.transform;
        cnTf.GetChild(0).GetComponent<TMP_Text>().text = repo.ServerURL;
        cnTf.GetChild(1).GetComponent<TMP_Text>().text = repo.RepositoryId;
    }

    private void StyleRepoBtn(int id)
    {
        int nbBtn = _repoBtns.Count;

        for (int i = 0; i < nbBtn; i++)
        {
            var btn = _repoBtns[i];
            btn.OnDeselect(null);
            ColorBlock colors = btn.colors;
            colors.normalColor = (i == id)? Color.green : Color.white;
            colors.selectedColor = colors.normalColor;
            colors.highlightedColor = colors.normalColor.Lighten(.1f);
            btn.colors = colors;
        }
    }

    private void SelectRepo(GraphDbRepository repo)
    {
        _selectedRepository = repo;
        _referenceHolderSO.SelectedGraphDbRepository = repo;
        _startBtn.interactable = true;
    }
}
