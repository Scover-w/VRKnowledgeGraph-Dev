using AIDEN.TactileUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepositoryItemUI : MonoBehaviour
{
    public List<Collider> Colliders => _interactionColliders;

    public ScrollItem ScrollItem { get; set; }

    [SerializeField]
    TMP_Text _repoNameTxt;

    [SerializeField]
    TMP_Text _serverUrlTxt;

    [SerializeField]
    Image _selectedImg;

    [SerializeField]
    List<Collider> _interactionColliders;

    [SerializeField]
    ButtonUI _modifyBtn;

    GraphDbRepository _repo;
    LoadRepoUI _loadRepoUI;


    public void Load(GraphDbRepository repo, LoadRepoUI loadRepoUI)
    {
        _repo = repo;
        _repoNameTxt.text = repo.RepositoryId;
        _serverUrlTxt.text = repo.ServerURL;
        _loadRepoUI = loadRepoUI;
        _modifyBtn.Interactable = false;
    }

    public void Select(bool select)
    {
        _selectedImg.enabled = select;
        _modifyBtn.Interactable = select;
    }

    public void SelectClick()
    {
        _loadRepoUI.SelecRepoFromItem(_repo);
    }

    public void TryDeleteClick()
    {
        _loadRepoUI.TryDeleteRepo();
    }
}
