using System.Collections;
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
    GameManager _gameManager;

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

    GraphDbRepositories _graphDbRespositories;
    GraphDbRepository _selectedRepository;


    List<Button> _repoBtns;


    // Start is called before the first frame update
    async void Start()
    {
        _repoCanvasGo.SetActive(true);

        _graphDbRespositories = await GraphDbRepositories.Load();

        if(_graphDbRespositories.Count > 0)
        {
            _selectedRepository = _graphDbRespositories.AutoSelect();
            _referenceHolderSO.SelectedGraphDbRepository = _selectedRepository; 
        }

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
        _selectedRepository = _graphDbRespositories.Select(id);
        _referenceHolderSO.SelectedGraphDbRepository = _selectedRepository;

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
        _referenceHolderSO.SelectedGraphDbRepository = _selectedRepository;

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
}



public static class ColorHelperExtension
{
    public static Color Lighten(this Color color,float amount)
    {
        Color lightenedColor = new Color(
                                Mathf.Clamp(color.r + amount, 0f, 1f),
                                Mathf.Clamp(color.g + amount, 0f, 1f),
                                Mathf.Clamp(color.b + amount, 0f, 1f),
                                color.a);
        return lightenedColor;
    }
}
