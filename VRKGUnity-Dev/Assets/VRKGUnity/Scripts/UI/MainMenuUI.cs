using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    ButtonUI _launchGraphBtn;

    private void Awake()
    {
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

        _repoStatusTxt.text = selectedRepo.RepositoryId;
        SetConnectionStatus(true);
        _referenceHolderSO.SelectedGraphDbRepository = selectedRepo;
    }


    private void SetConnectionStatus(bool isLoaded)
    {
        if(isLoaded) 
        {
            _iconRepoStatusImg.sprite = _connectedIcon;
            _launchGraphBtn.Interactable = true;
            return;
        }

        _repoStatusTxt.text = "Not Loaded";
        _iconRepoStatusImg.sprite = _unconnectedIcon;
        _launchGraphBtn.Interactable = false;
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

    public void LaunchGraphClick()
    {
        _mainMenuManager.LoadGraphScene();
    }
}

