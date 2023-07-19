using System.Text;
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
        //TryLoadMedia();
    }

    private void DisplayTexts()
    {
        var name = _nodeDisplayed.GetName();

        name ??= _nodeDisplayed.PrefixValue;

        if (name.Length > 30)
            name = name[..30] + "...";

        _typeTxt.text = _nodeDisplayed.Type.ToString();
        _valueTxt.text = name;
        _nbEdgeOrNameNodesTxt.text = (_nodeDisplayed.EdgeSource.Count + _nodeDisplayed.EdgeTarget.Count).ToString();
    }

    //private void TryLoadMedia()
    //{
    //    foreach (string urlMedia in _nodeDisplayed.Medias)
    //    {

    //        MediaState state = _repoMedias.TryGetMediaState

    //    }
    //}

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

        var nameSourceNode = nodeSource.GetName();
        var nameTargetNode = nodeTarget.GetName();

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
