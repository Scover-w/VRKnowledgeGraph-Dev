using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class NodgeInfoUI : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GameObject _canvasGo;

    [SerializeField]
    TMP_Text _typeTxt;

    [SerializeField]
    TMP_Text _valueTxt;

    [SerializeField]
    TMP_Text _nbEdgeOrNameNodesTxt;

    GraphDbRepositoryMedias _repoMedias;

    Node _nodeDisplayed;

    private void Start()
    {
        _repoMedias = _referenceHolderSo.SelectedGraphDbRepository.GraphDbRepositoryMedias;
        _canvasGo.SetActive(false);
    }

    public void DisplayInfoNode(Node node)
    {
        _canvasGo.SetActive(node != null);

        _nodeDisplayed = node;

        if (node == null)
            return;

        DisplayTexts();
        TryLoadMedias();
    }

    private void DisplayTexts()
    {
        var name = _nodeDisplayed.GetPrefName();

        name ??= _nodeDisplayed.PrefixValue;

        if (name.Length > 30)
            name = name[..30] + "...";

        _typeTxt.text = _nodeDisplayed.Type.ToString();
        _valueTxt.text = name;
        _nbEdgeOrNameNodesTxt.text = (_nodeDisplayed.EdgeSource.Count + _nodeDisplayed.EdgeTarget.Count).ToString();
    }

    private void TryLoadMedias()
    {
        List<string> mediaToRetrieve = new();

        foreach (string urlMedia in _nodeDisplayed.Medias)
        {
            MediaState state = _repoMedias.TryGetMediaState(urlMedia);


            if (state == MediaState.Unloadable)
                continue;

            if(state == MediaState.None)
            {
                mediaToRetrieve.Add(urlMedia);
                continue;
            }

            LoadMedia(urlMedia);
        }

        RetrieveMedias(mediaToRetrieve);
    }

    private async void LoadMedia(string mediaUrl)
    {
        string filePath = _repoMedias.GetPath(mediaUrl);

        if(!File.Exists(filePath)) 
        {
            Debug.LogError("File don't exist");
            return;
        }

        var imageBytes = await File.ReadAllBytesAsync(filePath);

        Texture2D texture = new(2, 2);
        texture.LoadImage(imageBytes);

        // TODO : Load image somewhere
        // _image.texture = texture;
        // Need to set the width, height of the texture to the image
    }

    private void RetrieveMedias(List<string> mediaToRetrieve) 
    {
        var node = _nodeDisplayed;

        Debug.Log("RetrieveMedias thread : " + Thread.CurrentThread.ManagedThreadId);

        foreach(string urlMedia in mediaToRetrieve) 
        {
            RetrieveMedia(node, urlMedia);
        }
    }


    private async void RetrieveMedia(Node node, string urlMedia)
    {
        Debug.Log("Start RetrieveMedia");
        string savePath = _repoMedias.GetPath(urlMedia);

        Texture2D text = await MediaAPI.DownloadAndSaveImage(urlMedia, savePath);

        await _repoMedias.AddMedia(urlMedia, (text == null) ? MediaState.Unloadable : MediaState.Loadable);

        Debug.Log("RetrieveMedia thread : " + Thread.CurrentThread.ManagedThreadId);

        if (node != _nodeDisplayed)
            return;

        // TODO : Load Image somewhere
        // _image.texture = texture;
    }

    public void DisplayInfoEdge(Edge edge)
    {
        _canvasGo.SetActive(edge != null);

        if (edge == null)
            return;

        _typeTxt.text = edge.Type.ToString();
        _valueTxt.text = edge.PrefixValue;

        StringBuilder sb = new();

        var nodeSource = edge.Source;
        var nodeTarget = edge.Target;

        var nameSourceNode = nodeSource.GetPrefName();
        var nameTargetNode = nodeTarget.GetPrefName();

        nameSourceNode ??= nodeSource.PrefixValue;
        nameTargetNode ??= nodeTarget.PrefixValue;


        if(nameSourceNode.Length > 30)
            nameSourceNode = nameSourceNode[..30] + "...";
        
        if(nameTargetNode.Length > 30)
            nameTargetNode = nameTargetNode[..30] + "...";

        sb.Append(nameSourceNode);
        sb.Append("\n |\n\\/\n");
        sb.Append(nameTargetNode);


        _nbEdgeOrNameNodesTxt.text = sb.ToString();
    }
}
