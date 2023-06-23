using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NodeSelector : MonoBehaviour
{
    [SerializeField]
    NodgeSelectionManager _selectionManager;

    public void OnEnterHover(HoverEnterEventArgs args) 
    {
        
    }

    public void OnExitHover(HoverExitEventArgs args)
    {
        Debug.Log("OnExitHover Node");
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        var interactableTf = args.interactableObject.transform;

        Debug.Log("OnEnterHover Node");
        _selectionManager.SelectNode(interactableTf);
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        Debug.Log("OnExitHover Node");
    }
}
