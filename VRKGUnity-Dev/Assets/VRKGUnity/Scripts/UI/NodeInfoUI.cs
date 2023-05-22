using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NodeInfoUI : MonoBehaviour
{
    [SerializeField]
    GameObject _canvasGo;

    [SerializeField]
    TMP_Text _typeTxt;

    [SerializeField]
    TMP_Text _valueTxt;

    [SerializeField]
    TMP_Text _nbEdgeTxt;

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

        _typeTxt.text = node.Type;
        _valueTxt.text = (name != null)? name : node.Value;
        _nbEdgeTxt.text = (node.EdgeSource.Count + node.EdgeTarget.Count).ToString();

        foreach (var prop in node.Properties)
        {
            Debug.Log(prop.Key + " : " + prop.Value);
        }
    }
}
