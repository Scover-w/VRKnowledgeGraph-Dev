using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;


    public void HideSelectedNode()
    {
        var graph = _graphManager.Graph;

        var selectedNode = graph.SelectedNode;

        if (selectedNode == null)
            return;

        var sparqlQuery = new SPARQLQuery(selectedNode);
        _graphManager.Add(sparqlQuery);
    }
    
}
