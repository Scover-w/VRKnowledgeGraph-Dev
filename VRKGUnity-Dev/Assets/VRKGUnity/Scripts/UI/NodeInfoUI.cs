using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
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
    ScrollViewUI _propertiesScrollUI;

    [SerializeField]
    ScrollViewUI _propertyScrollUI;

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
    GameObject _mediaIconGo;

    [SerializeField]
    GameObject _loadingMediaIconGo;

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
    GraphDbRepositoryNamespaces _repoNamespaces;
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
        _repoNamespaces = _referenceHolderSo.SelectedGraphDbRepository.GraphDbRepositoryNamespaces;
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

        List<ScrollItemUI> _scollItems = new();

        foreach(PropertyItemUI propertyItem in _propertyItems)
        {
            _scollItems.Add(propertyItem.ScrollItemUI);
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

        if (name == null || name.Length == 0)
            name = _nodeDisplayed.PrefixValue;

        _titleNodeTxt.text = name;

        Debug.Log("Test truncated : " + _titleNodeTxt.isTextTruncated);

        var ontoNode = _nodeDisplayed.OntoNode;

        string className = (ontoNode == null) ? "" : ontoNode.Value;

        if (className.Length == 0)
        {
            _classNodeTxt.text = "";
            return;
        }

        _classNodeTxt.text = _repoNamespaces.GetUriPrefixed(className);
    }

    private void CreatePropertyItems()
    {
        var properties = _nodeDisplayed.Properties;


        Dictionary<string, PropertyItemUI> valuesDict = new();
        List<ScrollItemUI> scrollItems = new();


        foreach(var property in properties)
        {

            string uriPrefixed = _repoNamespaces.GetUriPrefixed(property.Key);
            string value = property.Value;

            if(valuesDict.TryGetValue(value, out PropertyItemUI itemUI))
            {
                itemUI.AddNamespace(uriPrefixed);
                continue;
            }

            PropertyItemUI propItemUI = CreatePropertyItem(uriPrefixed, value);
            scrollItems.Add(propItemUI.ScrollItemUI);
            valuesDict.Add(value, propItemUI);
        }

        _propertiesScrollUI.AddItems(scrollItems);
    }


    public void DisplayProperty(PropertyItemUI propertyItem)
    {
        if(_displayedProperty == propertyItem)
        {
            HideProperty();
            return;
        }

        _displayedProperty = propertyItem;
        _propertyGo.SetActive(true);
        _mediaGo.SetActive(false);


        var namespaces = propertyItem.Namespaces;
        StringBuilder sb = new(namespaces.Count > 1? "Uris :" : "Uri :");

        foreach(var namespce in namespaces)
        {
            sb.AppendLine(namespce);
        }

        sb.AppendLine("\nValue :");
        sb.AppendLine(propertyItem.Value);

        _valueTxt.text = sb.ToString();
        _propertyScrollUI.UpdateContent();
    }

    public void HideProperty()
    {
        _propertyGo.SetActive(false);
        _mediaGo.SetActive(true);

        if (_displayedProperty == null)
            return;

        _displayedProperty = null;
    }

    public void OnHidePropertyClick()
    {
        HideProperty();
    }

    private PropertyItemUI CreatePropertyItem(string namespce, string value)
    {
        var go = Instantiate(_propertyItemPf, _propertiesScrollUI.ItemContainer);

        var propertyItemUI = go.GetComponent<PropertyItemUI>();
        propertyItemUI.Load(this, namespce, value);

        _propertyItems.Add(propertyItemUI);

        return propertyItemUI;
    }

    #region LoadMedia
    private void TryLoadMedias()
    {
        _medias = new();
        List<string> localMediaToRetrieve = new();
        List<string> internetMediaToRetrieve = new();


        bool doContainMedias = _nodeDisplayed.Medias.Count > 0;

        _mediaIconGo.SetActive(!doContainMedias);
        _loadingMediaIconGo.SetActive(doContainMedias);

        if (!doContainMedias)
            return;

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

        float scale;
        if (ratioText < 1f)
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
