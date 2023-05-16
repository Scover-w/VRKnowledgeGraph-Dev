using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphUI : MonoBehaviour
{
    [SerializeField]
    User _user;

    [SerializeField]
    NodeInfoUI _nodeInfoUI;



    public void DisplayInfoNode(Node node)
    {
        _nodeInfoUI.DisplayInfoNode(node);
    }

    public void HideSelectedNode()
    {
        _user.HideSelectedNode();
    }

    public void HideSelectedEdge()
    {

    }
}
