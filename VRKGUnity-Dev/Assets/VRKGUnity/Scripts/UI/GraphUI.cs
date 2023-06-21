using UnityEngine;

public class GraphUI : MonoBehaviour
{
    [SerializeField]
    User _user;

    [SerializeField]
    NodgeInfoUI _nodgeInfoUI;


    public void DisplayInfoNode(Node node)
    {
        _nodgeInfoUI.DisplayInfoNode(node);
    }

    public void DisplayInfoEdge(Edge edge)
    {
        _nodgeInfoUI.DisplayInfoEdge(edge);
    }

    public void HideSelectedNode()
    {
        _user.HideSelectedNode();
    }

    public void HideSelectedEdge()
    {

    }
}
