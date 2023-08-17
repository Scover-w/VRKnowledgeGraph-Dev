using AIDEN.TactileUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepositoryItemUI : MonoBehaviour
{
    public ScrollItemUI ScrollItemUI {  get { return _scrollItemUI; } }

    [SerializeField]
    ScrollItemUI _scrollItemUI;

    [SerializeField]
    TMP_Text _repoNameTxt;

    [SerializeField]
    TMP_Text _serverUrlTxt;

    GraphDbRepository _repo;
    LoadRepoUI _loadRepoUI;


    public void Load(GraphDbRepository repo, LoadRepoUI loadRepoUI)
    {
        _repo = repo;
        _repoNameTxt.text = repo.RepositoryId;
        _serverUrlTxt.text = repo.ServerURL;
        _loadRepoUI = loadRepoUI;
    }


    public void SelectClick()
    {
        _loadRepoUI.SelectRepoFromItem(_repo);
    }
}
