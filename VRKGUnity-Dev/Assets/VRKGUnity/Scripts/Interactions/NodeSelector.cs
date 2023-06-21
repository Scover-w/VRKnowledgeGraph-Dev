using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NodeSelector : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    public void OnEnterHover(HoverEnterEventArgs args) 
    {
        
    }

    public void OnExitHover(HoverExitEventArgs args)
    {
        Debug.Log("OnExitHover Node");
    }


    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        var graph = _graphManager.Graph;
        var interactableTf = args.interactableObject.transform;

        if (!graph.IsInGraph(interactableTf))
            return;


        Debug.Log("OnEnterHover Node");
        graph.SelectNode(interactableTf);
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log("OnExitHover Node");
    }
}
