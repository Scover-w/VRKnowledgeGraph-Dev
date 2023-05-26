using QuikGraph.Algorithms.Observers;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class NodgeInfoUI : MonoBehaviour
{
    [SerializeField]
    GameObject _canvasGo;

    [SerializeField]
    TMP_Text _typeTxt;

    [SerializeField]
    TMP_Text _valueTxt;

    [SerializeField]
    TMP_Text _nbEdgeOrNameNodesTxt;

    private void Start()
    {
        _canvasGo.SetActive(false);
    }

    public void DisplayInfoNode(Node node)
    {
        _canvasGo.SetActive(node != null);

        if (node == null)
            return;

        var name = node.GetName();

        if (name == null)
            name = node.Value;

        if (name.Length > 30)
            name = name.Substring(0, 30) + "...";

        _typeTxt.text = node.Type;
        _valueTxt.text = name;
        _nbEdgeOrNameNodesTxt.text = (node.EdgeSource.Count + node.EdgeTarget.Count).ToString();
    }

    public void DisplayInfoEdge(Edge edge)
    {
        _canvasGo.SetActive(edge != null);

        if (edge == null)
            return;

        _typeTxt.text = edge.Type;
        _valueTxt.text = edge.Value;

        StringBuilder sb = new();

        var nodeSource = edge.Source;
        var nodeTarget = edge.Target;

        var nameSourceNode = nodeSource.GetName();
        var nameTargetNode = nodeTarget.GetName();

        if (nameSourceNode == null)
            nameSourceNode = nodeSource.Value;

        if(nameTargetNode == null)
            nameTargetNode = nodeTarget.Value;


        if(nameSourceNode.Length > 30)
            nameSourceNode = nameSourceNode.Substring(0, 30) + "...";
        
        if(nameTargetNode.Length > 30)
            nameTargetNode = nameTargetNode.Substring(0, 30) + "...";

        sb.Append(nameSourceNode);
        sb.Append("\n |\n\\/\n");
        sb.Append(nameTargetNode);


        _nbEdgeOrNameNodesTxt.text = sb.ToString();

    }
}
