using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;
    
    [SerializeField]
    NodgeSelectionManager _nodeSelectionManager;


    GraphMode _graphMode = GraphMode.Desk;


    [ContextMenu("Switch Mode")]
    public void SwitchMode()
    {
        if (_graphManager.IsRunningSimulation)
        {
            Debug.Log("Can't switch Mode when running simulation");
            // TODO : Notification can't switch mode in simulation
            return;
        }

        if(_graphMode == GraphMode.Desk)
        {
            Debug.Log("Switch to Immersion Mode");
            _graphMode = GraphMode.Immersion;
            _graphManager.TrySwitchModeToImmersion();
        }
        else
        {
            Debug.Log("Switch to Desk Mode");
            _graphMode = GraphMode.Desk;
            _graphManager.TrySwitchModeToDesk();
        }
    }


    //public void HideSelectedNode()
    //{
    //    var graph = _graphManager.Graph;

    //    // TODO :  HideSelectedNode

    //    Node selectedNode = null;// graph.SelectedNode;

    //    if (selectedNode == null)
    //        return;

    //    var sparqlQuery = new SPARQLQuery(selectedNode);
    //    _graphManager.Add(sparqlQuery);
    //}
    
}
