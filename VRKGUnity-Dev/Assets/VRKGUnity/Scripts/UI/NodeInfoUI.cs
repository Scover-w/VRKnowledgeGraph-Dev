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
    ScrollUI _propertiesScrollUI;

    [SerializeField]
    ScrollUI _propertyScrollUI;

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
    GameObject _mediaGo;

    [SerializeField]
    GameObject _propertyGo;

    [SerializeField]
    GameObject _propertyItemPf;

    [SerializeField]
    TMP_Text _valueTxt;

    [SerializeField]
    List<ButtonUI> _previousBtns;

    [SerializeField]
    List<ButtonUI> _nextBtns;

    List<PropertyItemUI> _propertyItems;

    GraphDbRepositoryMedias _repoMedias;
    NodgeSelectionManager _selectionManager;

    RectTransform _infoMediaImgRect;
    RectTransform _zoomedMediaImgRect;

    Node _nodeDisplayed;
    Node _nodeToDisplayLock;
    bool _isLocked = false;
    bool _inZoom = false;

    Vector2 _maxMediaSize = new(630, 400);
    Vector2 _maxZoomedMediaSize = new(705, 930);

    List<Texture2D> _medias;
    Texture2D _displayedTexture;

    PropertyItemUI _displayedProperty;

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

        var lastSelectedNode = _selectionManager.LastSelectedNode;
        if (lastSelectedNode == null)
            return;

        DisplayInfoNode(lastSelectedNode);
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
        _displayedProperty = null;
        _selectedTextureId = -1;
        _titleNodeTxt.text = "";
        _classNodeTxt.text = "";
        _infoNavGo.SetActive(false);
        _propertyGo.SetActive(false);
        _mediaGo.SetActive(true);

        
        ClearPropertiesFromScroll();
        StopAllCoroutines();
        SetZoom(false);
        UpdateNavBtns();
        UpdateLockImg();
    }

    private void ClearPropertiesFromScroll()
    {
        if (_propertyItems == null)
        {
            _propertyItems = new();
            return;
        }

        List<ScrollItem> _scollItems = new();

        foreach(PropertyItemUI propertyItem in _propertyItems)
        {
            _scollItems.Add(propertyItem.ScrollItem);
        }

        _propertiesScrollUI.RemoveItems(_scollItems);

        _propertyItems = new();
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
    }

    public void OnNextMediaClick() 
    {
        if (_medias == null || _medias.Count == 0)
            return;

        _selectedTextureId++;

        if (_selectedTextureId > _medias.Count - 1)
            _selectedTextureId = 0;

        DisplaySelectedIdMedia();
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
        CreatePropertyItems();
        TryLoadMedias();
    }

    private void DisplayTexts()
    {
        var name = _nodeDisplayed.GetPrefName();
        _titleNodeTxt.text = name;

        Debug.Log("Test truncated : " + _titleNodeTxt.isTextTruncated);

        var ontoNode = _nodeDisplayed.OntoNode;

        _classNodeTxt.text = (ontoNode == null) ? "" : ontoNode.Value;
    }

    private void CreatePropertyItems()
    {
        var properties = _nodeDisplayed.Properties;


        Dictionary<string, PropertyItemUI> valuesDict = new();
        List<ScrollItem> scrollItems = new();


        foreach(var property in properties)
        {
            string uri = property.Key;
            string value = property.Value;

            if(valuesDict.TryGetValue(value, out PropertyItemUI itemUI))
            {
                itemUI.AddUri(uri);
                continue;
            }

            PropertyItemUI propItemUI = CreatePropertyItem(uri, value);
            scrollItems.Add(propItemUI.ScrollItem);
            valuesDict.Add(value, propItemUI);
        }

        _propertiesScrollUI.AddItems(scrollItems);
    }


    public void DisplayProperty(PropertyItemUI propertyItem)
    {
        _displayedProperty = propertyItem;
        _propertyGo.SetActive(true);
        _mediaGo.SetActive(false);
        _valueTxt.text = propertyItem.Value;
        _propertyScrollUI.UpdateContent();
    }

    public void HideProperty()
    {
        _propertyGo.SetActive(false);
        _mediaGo.SetActive(true);
        _displayedProperty = null;
    }

    public void OnHidePropertyClick()
    {
        _propertyGo.SetActive(false);
        _mediaGo.SetActive(true);

        if (_displayedProperty == null)
            return;

        _displayedProperty.Unselect();
        _displayedProperty = null;
    }

    private PropertyItemUI CreatePropertyItem(string uri, string value)
    {
        var go = Instantiate(_propertyItemPf, _propertiesScrollUI.ItemContainer);

        var propertyItemUI = go.GetComponent<PropertyItemUI>();
        propertyItemUI.Load(this, uri, value);

        var colliders = propertyItemUI.Colliders;
        var scollItem = new ScrollItem(go.GetComponent<RectTransform>(), colliders);

        propertyItemUI.ScrollItem = scollItem;
        _propertyItems.Add(propertyItemUI);

        return propertyItemUI;
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

        byte[] bytes;

        if (Path.GetExtension(savePath).ToLower() == ".png")
            bytes = texture.EncodeToPNG();
        else
            bytes = texture.EncodeToJPG();

        await File.WriteAllBytesAsync(savePath, bytes);
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
        Vector2 maxSize = _inZoom ? _maxZoomedMediaSize : _maxMediaSize;

        var ratioText = _displayedTexture.width / _displayedTexture.height;

        float scale = 1f;

        if(ratioText < 1f)
        {
            // Height Max
            scale = maxSize.y / _displayedTexture.height;
        }
        else
        {
            // Width Max
            scale = maxSize.x / _displayedTexture.width;
        }


        rectTf.sizeDelta = new Vector2(_displayedTexture.width * scale, _displayedTexture.height * scale);
    }

    private void AddMedia(Texture2D texture)
    {
        _medias.Add(texture);

        if (_medias.Count == 1)
        {
            // First Media to be added
            _infoNavGo.SetActive(true);
            _selectedTextureId = 0;
            DisplaySelectedIdMedia();
        }

        UpdateNavBtns();
    }

    private void UpdateNavBtns()
    {
        bool isBtnInteractable = false;

        if (_medias != null)
        {
            isBtnInteractable = _medias.Count > 1;
        }

        foreach(ButtonUI btn in _previousBtns)
            btn.Interactable = isBtnInteractable;

        foreach (ButtonUI btn in _nextBtns)
            btn.Interactable = isBtnInteractable;
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
