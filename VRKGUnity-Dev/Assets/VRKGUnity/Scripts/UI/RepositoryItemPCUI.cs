using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepositoryItemPCUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text _repoNameTxt;

    [SerializeField]
    TMP_Text _serverUrlTxt;

    [SerializeField]
    Image _selectedImg;

    [SerializeField]
    Button _modifyBtn;

    GraphDbRepository _repo;
    LoadRepoUI _loadRepoUI;


    public void Load(GraphDbRepository repo, LoadRepoUI loadRepoUI)
    {
        _repo = repo;
        _repoNameTxt.text = repo.RepositoryId;
        _serverUrlTxt.text = repo.ServerURL;
        _loadRepoUI = loadRepoUI;
        _modifyBtn.interactable = false;
    }

    public void Select(bool select)
    {
        _selectedImg.enabled = select;
        _modifyBtn.interactable = select;
    }

    public void SelectClick()
    {
        Debug.Log("SelectClick");
        _loadRepoUI.SelectRepoFromItem(_repo);
    }

    public void TryDeleteClick()
    {
        Debug.Log("TryDeleteClick");
        _loadRepoUI.TryDeleteRepo();
    }
}
