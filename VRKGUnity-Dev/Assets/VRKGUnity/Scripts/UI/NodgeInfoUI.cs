using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class NodgeInfoUI : MonoBehaviour
{
    //[SerializeField]
    //ReferenceHolderSO _referenceHolderSo;

    //[SerializeField]
    //GameObject _canvasGo;

    //[SerializeField]
    //TMP_Text _typeTxt;

    //[SerializeField]
    //TMP_Text _valueTxt;

    //[SerializeField]
    //TMP_Text _nbEdgeOrNameNodesTxt;

    //GraphDbRepositoryMedias _repoMedias;

    //Node _nodeDisplayed;

    //private void Start()
    //{
    //    _repoMedias = _referenceHolderSo.SelectedGraphDbRepository.GraphDbRepositoryMedias;
    //    _canvasGo.SetActive(false);
    //}

    //public void DisplayInfoNode(Node node)
    //{
    //    _canvasGo.SetActive(node != null);

    //    _nodeDisplayed = node;

    //    if (node == null)
    //        return;

    //    DisplayTexts();
    //    TryLoadMedias();
    //}

    //private void DisplayTexts()
    //{
    //    var name = _nodeDisplayed.GetPrefName();

    //    name ??= _nodeDisplayed.PrefixValue;

    //    if (name.Length > 30)
    //        name = name[..30] + "...";

    //    _typeTxt.text = _nodeDisplayed.Type.ToString();
    //    _valueTxt.text = name;
    //    _nbEdgeOrNameNodesTxt.text = (_nodeDisplayed.EdgeSource.Count + _nodeDisplayed.EdgeTarget.Count).ToString();
    //}

    //private void TryLoadMedias()
    //{
    //    List<string> mediaToRetrieve = new();

    //    foreach (string urlMedia in _nodeDisplayed.Medias)
    //    {
    //        MediaState state = _repoMedias.TryGetMediaState(urlMedia);


    //        if (state == MediaState.Unloadable)
    //            continue;

    //        if(state == MediaState.None)
    //        {
    //            mediaToRetrieve.Add(urlMedia);
    //            continue;
    //        }

    //        LoadMedia(urlMedia);
    //    }

    //    RetrieveMedias(mediaToRetrieve);
    //}

    //private void LoadMedia(string mediaUrl)
    //{
    //    string filePath = _repoMedias.GetPath(mediaUrl);

    //    if(!File.Exists(filePath)) 
    //    {
    //        Debug.LogError("File don't exist");
    //        return;
    //    }

    //    StartCoroutine(DownloadLocalTexture(filePath, _nodeDisplayed));      
    //}

    //private async void RetrieveMedias(List<string> mediaToRetrieve) 
    //{
    //    var node = _nodeDisplayed;


    //    foreach(string urlMedia in mediaToRetrieve) 
    //    {

    //        string extension = Path.GetExtension(urlMedia).ToLower();

    //        if (!(extension.Contains(".jpg") || extension.Contains(".png")))
    //        {
    //            await _repoMedias.AddMedia(urlMedia, MediaState.Unloadable);
    //            continue;
    //        }

    //        StartCoroutine(DownloadTexture(urlMedia, node));
    //    }
    //}

    //IEnumerator DownloadLocalTexture(string filePath, Node node)
    //{
    //    using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath);

    //    yield return uwr.SendWebRequest();

    //    if (uwr.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(uwr.error);
    //        yield break;
    //    }

    //    Stopwatch stopwatch = new();
    //    stopwatch.Start();
    //    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);


    //    //var texture.EncodeToJPG();

    //    stopwatch.Stop();

    //    TimeSpan ts = stopwatch.Elapsed;
    //    var msSpan = ts.TotalMilliseconds;
    //    Debug.Log(" has lasted " + msSpan + " ms.");

    //    DisplayTexture(texture, node);
    //}

    //IEnumerator DownloadTexture(string mediaUrl, Node node)
    //{
    //    using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(mediaUrl);
    //    yield return uwr.SendWebRequest();

    //    if (uwr.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(uwr.error);
    //        Debug.Log(mediaUrl);
    //        _ = _repoMedias.AddMedia(mediaUrl, MediaState.Unloadable);
    //        yield break;
    //    }

    //    _ = _repoMedias.AddMedia(mediaUrl, MediaState.Loadable);

    //    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

    //    string savePath = _repoMedias.GetPath(mediaUrl);
    //    byte[] bytes;

    //    if (Path.GetExtension(savePath).ToLower() == ".png")
    //        bytes = texture.EncodeToPNG();
    //    else
    //        bytes = texture.EncodeToJPG();

    //    SaveMedia(savePath, bytes);

    //    DisplayTexture(texture, node);
    //}

    //private async void SaveMedia(string savePath, byte[] byteMedia)
    //{
    //    await File.WriteAllBytesAsync(savePath, byteMedia);
    //}

    //private void DisplayTexture(Texture2D texture, Node node)
    //{
    //    if (node != _nodeDisplayed)
    //        return;

    //    // TODO : Display texture
    //}
}
