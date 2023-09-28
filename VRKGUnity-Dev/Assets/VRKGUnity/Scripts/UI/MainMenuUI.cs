using AIDEN.TactileUI;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    MainMenuManager _mainMenuManager;

    [SerializeField]
    GameObject _menuPageGo;

    [SerializeField]
    GameObject _loadRepoPageGo;

    [SerializeField]
    GameObject _settingsPageGo;

    [SerializeField]
    TMP_Text _repoStatusTxt;

    [SerializeField]
    Image _iconRepoStatusImg;

    [SerializeField]
    Sprite _unconnectedIcon;

    [SerializeField]
    Sprite _connectedIcon;

    [SerializeField]
    GameObject _loadingIconGo;

    [Header("VR")]
    [SerializeField]
    ButtonUI _launchGraphBtn;

#if UNITY_EDITOR
    [Header("PC")]
    [SerializeField]
    Button _launchGraphPcBtn;
#endif

    private void Awake()
    {
        _loadingIconGo.SetActive(false);
        SetConnectionStatus(false);

        DisplayMainMenu();
    }

    public void RepoSelected(GraphDbRepository selectedRepo)
    {
        if (selectedRepo == null)
        {
            SetConnectionStatus(false);
            _referenceHolderSO.SelectedGraphDbRepository = null;
            return;
        }

        DebugDev.Log("selectedRepo.GraphDbRepositoryId : " + selectedRepo.GraphDbRepositoryId);

        _repoStatusTxt.text = selectedRepo.GraphDbRepositoryId;
        SetConnectionStatus(true);
        _referenceHolderSO.SelectedGraphDbRepository = selectedRepo;
    }


    private void SetConnectionStatus(bool isLoaded)
    {
        if(isLoaded) 
        {
            _iconRepoStatusImg.sprite = _connectedIcon;

            SetLaunchGraphBtnState(true);
            
            return;
        }

        _repoStatusTxt.text = "Non chargé";
        _iconRepoStatusImg.sprite = _unconnectedIcon;
        SetLaunchGraphBtnState(false);
    }

    public void DisplayMainMenu()
    {
        _menuPageGo.SetActive(true);
        _loadRepoPageGo.SetActive(false);
        _settingsPageGo.SetActive(false);
    }

    public void DisplayRepositoryClick()
    {
        _menuPageGo.SetActive(false);
        _loadRepoPageGo.SetActive(true);
        _settingsPageGo.SetActive(false);
    }

    public void DisplaySettings()
    {
        _menuPageGo.SetActive(false);
        _loadRepoPageGo.SetActive(false);
        _settingsPageGo.SetActive(true);
    }

    public void LoadTutorialClick()
    {
        _mainMenuManager.LoadTutorialScene();
    }

    public async void LaunchGraphClick()
    {
        var repo = _referenceHolderSO.SelectedGraphDbRepository;

        if (repo == null) return;

        SetLaunchGraphBtnState(false);
        _loadingIconGo.SetActive(true);

        await repo.SetGraphDbCredentials();

        _mainMenuManager.LoadGraphScene();
    }

    public void DeleteAllClick()
    {
        var folderPath = Path.Combine(Application.persistentDataPath, "Data");

        Directory.Delete(folderPath, true);
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = true;
#endif
    }

    private void SetLaunchGraphBtnState(bool isInteractable)
    {
        if (_launchGraphBtn != null)
            _launchGraphBtn.Interactable = isInteractable;

#if UNITY_EDITOR
        if (_launchGraphPcBtn != null)
            _launchGraphPcBtn.interactable = isInteractable;
#endif
    }
}

