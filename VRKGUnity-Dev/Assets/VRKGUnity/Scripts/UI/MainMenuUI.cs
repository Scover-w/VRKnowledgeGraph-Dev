using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
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
        _repoStatusTxt.text = "Not Loaded";
        _launchGraphBtn.Interactable = false;
        _iconRepoStatusImg.sprite = _unconnectedIcon;

        DisplayMainMenu();
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

