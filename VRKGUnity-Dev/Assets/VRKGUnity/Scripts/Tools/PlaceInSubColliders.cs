using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
public class PlaceInSubColliders : MonoBehaviour
{
    [ContextMenu("Organize Selected Objects")]
    public void Organize()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            Transform selectedTf = selectedObject.transform;
            Transform interactionCollidersTf = selectedTf.Find("InteractionColliders");

            if (interactionCollidersTf != null)
                continue;

            GameObject newObject = new("InteractionColliders");
            interactionCollidersTf = newObject.transform;
            interactionCollidersTf.parent = selectedTf;
            ResetTf(interactionCollidersTf);

            MoveChildToNewParent(selectedTf, interactionCollidersTf, "Proximity");
            MoveChildToNewParent(selectedTf, interactionCollidersTf, "Interaction");
        }
    }

    private void MoveChildToNewParent(Transform parent, Transform newParent, string childName)
    {
        Transform child = parent.Find(childName);
        if (child != null)
        {
            child.parent = newParent;
            ResetTf(child);
        }
    }

    void ResetTf(Transform tf)
    {
        tf.localPosition = Vector3.zero;
        tf.localRotation = Quaternion.identity;
        tf.localScale = new Vector3(1f, 1f, 1f);

        tf.gameObject.layer = Layers.UI;
    }
}
#endif