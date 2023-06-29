using UnityEngine;

public class GraphUI : MonoBehaviour
{
    [SerializeField]
    User _user;

    [SerializeField]
    NodgeInfoUI _nodgeInfoUI;

    [SerializeField]
    NodgeSelectionManager _selectionManager;


    private void Start()
    {
        _selectionManager.OnNodeSelected += DisplayInfoNode;
    }

    public void DisplayInfoNode(Node node)
    {
        _nodgeInfoUI.DisplayInfoNode(node);
    }

    public void DisplayInfoEdge(Edge edge)
    {
        _nodgeInfoUI.DisplayInfoEdge(edge);
    }

    public void HideSelectedEdge()
    {

    }
}
