using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;
    
    [SerializeField]
    NodgeSelectionManager _nodeSelectionManager;


    public void HideSelectedNode()
    {
        var graph = _graphManager.Graph;

        // TODO :  HideSelectedNode

        Node selectedNode = null;// graph.SelectedNode;

        if (selectedNode == null)
            return;

        var sparqlQuery = new SPARQLQuery(selectedNode);
        _graphManager.Add(sparqlQuery);
    }
    
}
