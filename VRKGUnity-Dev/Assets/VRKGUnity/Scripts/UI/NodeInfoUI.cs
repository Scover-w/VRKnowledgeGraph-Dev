using AIDEN.TactileUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class NodeInfoUI : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GameObject _infoNodeGo;

    [SerializeField]
    GameObject _mediaZoomGo;

    [SerializeField]
    Image _lockImg;

    [SerializeField]
    Sprite _lockedSprt;

    [SerializeField]
    Sprite _unlockedSprt;

    [SerializeField]
    TMP_Text _titleNodeTxt;

    [SerializeField]
    TMP_Text _classNodeTxt;

    [SerializeField]
    RawImage _infoMediaImg;

    [SerializeField]
    RawImage _zoomedMediaImg;

    [SerializeField]
    GameObject _infoNavGo;

    [SerializeField]
    List<ButtonUI> _previousBtns;

    [SerializeField]
    List<ButtonUI> _nextBtns;

    GraphDbRepositoryMedias _repoMedias;
    NodgeSelectionManager _selectionManager;

    RectTransform _infoMediaImgRect;
    RectTransform _zoomedMediaImgRect;

    Node _nodeDisplayed;
    Node _nodeToDisplayLock;
    bool _isLocked = false;
    bool _inZoom = false;

    Vector2 _maxMediaSize = new Vector2(569, 382);
    Vector2 _maxZoomedMediaSize = new Vector2(705, 930);

    List<Texture2D> _medias;
    Texture2D _displayedTexture;

    int _selectedTextureId = -1;

    private void Awake()
    {
        _repoMedias = _referenceHolderSo.SelectedGraphDbRepository.GraphDbRepositoryMedias;
        _selectionManager = _referenceHolderSo.NodgeSelectionManager;

        _infoMediaImgRect = _infoMediaImg.GetComponent<RectTransform>();
        _zoomedMediaImgRect = _zoomedMediaImg.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _selectionManager.OnNodeSelected += DisplayInfoNode;
        ResetParameters();
    }

    private void OnDisable()
    {
        _selectionManager.OnNodeSelected -= DisplayInfoNode;
    }


    private void ResetParameters()
    {
        _isLocked = false;
        _infoMediaImg.enabled = false;
        _displayedTexture = null;
        _selectedTextureId = -1;
        _infoNavGo.SetActive(false);

        StopAllCoroutines();
        SetZoom(false);
        UpdateNavBtns();
        UpdateLockImg();
    }


    #region UIEvent
    public void OnSwitchLockClick()
    {
        _isLocked = !_isLocked;

        UpdateLockImg();

        if (_isLocked)
            return;

        DisplayInfoNode(_nodeToDisplayLock);
        _nodeToDisplayLock = null;
    }

    public void OnPreviousMediaClick()
    {
        if(_medias == null || _medias.Count == 0) 
            return;

        _selectedTextureId--;

        if(_selectedTextureId < 0 )
            _selectedTextureId = _medias.Count - 1;

        DisplaySelectedIdMedia();
        UpdateNavBtns();
    }

    public void OnNextMediaClick() 
    {
        if (_medias == null || _medias.Count == 0)
            return;

        _selectedTextureId++;

        if (_selectedTextureId > _medias.Count - 1)
            _selectedTextureId = 0;

        DisplaySelectedIdMedia();
        UpdateNavBtns();
    }

    public void OnZoomInClick()
    {
        if(_inZoom) 
            return;

        SetZoom(true);
    }

    public void OnZoomOutClick()
    {
        if (!_inZoom) 
            return;

        SetZoom(false);
    }
    #endregion


    public void DisplayInfoNode(Node node)
    {
        if (node == null)
        {
            return;
        }

        if (_nodeDisplayed == node)
            return;

        if(_isLocked)
        {
            _nodeToDisplayLock = node;
            return;
        }

        ResetParameters(); 
        _nodeDisplayed = node;
        

        DisplayTexts();
        TryLoadMedias();
    }

    private void DisplayTexts()
    {
        var name = _nodeDisplayed.GetPrefName();
        _titleNodeTxt.text = name;

        var ontoNode = _nodeDisplayed.OntoNode;

        _classNodeTxt.text = (ontoNode == null) ? "" : ontoNode.Value;
    }


    #region LoadMedia
    private void TryLoadMedias()
    {
        _medias = new();
        List<string> localMediaToRetrieve = new();
        List<string> internetMediaToRetrieve = new();

        foreach (string urlMedia in _nodeDisplayed.Medias)
        {
            if (!_repoMedias.TryGetMediaData(urlMedia, out MediaData mediaData))
            {
                internetMediaToRetrieve.Add(urlMedia);
                continue;
            }

            if (mediaData.State == MediaState.Unloadable)
                continue;

            localMediaToRetrieve.Add(urlMedia);
        }

        LoadLocalMedias(localMediaToRetrieve);
        LoadInternetMedias(internetMediaToRetrieve);
    }

    private void LoadLocalMedias(List<string> mediaUrls)
    {
        Dictionary<string, MediaData> mediaDatas = new();

        foreach (string mediaUrl in mediaUrls) 
        {
            if(_repoMedias.TryGetMediaData(mediaUrl, out MediaData mediaData))
                mediaDatas.Add(mediaUrl, mediaData);
        }

        StartCoroutine(DownloadLocalMedias(mediaDatas));
    }

    private async void LoadInternetMedias(List<string> mediaToRetrieve)
    {
        List<string> mediasUrls = new();

        foreach (string urlMedia in mediaToRetrieve)
        {
            string extension = Path.GetExtension(urlMedia).ToLower();

            if (!(extension.Contains(".jpg") || extension.Contains(".png")))
            {
                await _repoMedias.AddMedia(urlMedia, new MediaData(MediaState.Unloadable));
                continue;
            }

            mediasUrls.Add(urlMedia);
        }

        StartCoroutine(DownloadTextures(mediasUrls));
    }

    IEnumerator DownloadLocalMedias(Dictionary<string, MediaData> mediaDatas)
    {

        foreach(var kvp in mediaDatas) 
        {
            string mediaUrl = kvp.Key;
            string filePath = _repoMedias.GetPath(mediaUrl);

            if (!File.Exists(filePath))
            {
                Debug.LogError("File don't exist");
                // TODO : Update _repomedias
                continue; 
            }

            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath);

            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
                continue;
            }

            Texture2D texture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
            AddMedia(texture);
        }
    }

    IEnumerator DownloadTextures(List<string> mediaUrls)
    {

        foreach (string mediaUrl in mediaUrls)
        {
            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(mediaUrl);
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
                Debug.Log(mediaUrl);
                _ = _repoMedias.AddMedia(mediaUrl, new MediaData(MediaState.Unloadable));
                continue;
            }


            Texture2D texture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
            AddMedia(texture);
            SaveTexture(texture, mediaUrl);
        }
    }

    private async void SaveTexture(Texture2D texture, string mediaUrl)
    {
        string savePath = _repoMedias.GetPath(mediaUrl);
        Vector2 size = new(texture.width, texture.height);
        TextureFormat format = texture.format;

        MediaData mediaData = new(MediaState.Loadable, size, format);

        byte[] rawData = texture.GetRawTextureData();
        await File.WriteAllBytesAsync(savePath, rawData);
        await _repoMedias.AddMedia(mediaUrl, mediaData);
    }
    #endregion


    private void DisplaySelectedIdMedia()
    {
        _displayedTexture = _medias[_selectedTextureId];

        if(_inZoom)
        {
            _zoomedMediaImg.texture = _displayedTexture;
            ResizeRawImage();
        }
        else
        {
            _infoMediaImg.texture = _displayedTexture;
            _infoMediaImg.enabled = true;
            ResizeRawImage();
        }

    }

    private void ResizeRawImage()
    {
        RectTransform rectTf = _inZoom? _zoomedMediaImgRect: _infoMediaImgRect;
        rectTf.sizeDelta = new Vector2(_displayedTexture.width, _displayedTexture.height);
    }

    private void AddMedia(Texture2D texture)
    {
        _medias.Add(texture);

        if (_medias.Count > 1)
            return;

        // First Media to be added
        _infoNavGo.SetActive(true);
        _selectedTextureId = 0;
        DisplaySelectedIdMedia();
        UpdateNavBtns();
    }

    private void UpdateNavBtns()
    {
        bool isPreviousBtnInteractable = false;
        bool isNextBtnInteractable = false;

        if (_medias != null)
        {
            isPreviousBtnInteractable = (_selectedTextureId > 0);
            isNextBtnInteractable = (_selectedTextureId < _medias.Count - 1);
        }

        foreach(ButtonUI btn in _previousBtns)
            btn.Interactable = isPreviousBtnInteractable;

        foreach (ButtonUI btn in _nextBtns)
            btn.Interactable = isNextBtnInteractable;
    }

    private void UpdateLockImg()
    {
        _lockImg.sprite = _isLocked ? _lockedSprt : _unlockedSprt;
    }

    private void SetZoom(bool inZoom)
    {
        _inZoom = inZoom;
        _infoNodeGo.SetActive(!inZoom);
        _mediaZoomGo.SetActive(inZoom);

        if (_displayedTexture == null)
            return;

        if (_selectedTextureId == -1)
            return;

        if (_selectedTextureId > _medias.Count - 1)
            _selectedTextureId = 0;

        DisplaySelectedIdMedia();
    }
}
