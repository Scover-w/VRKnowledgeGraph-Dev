using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace AIDEN.TactileUI
{
    public class TactileUITools : MonoBehaviour
    {
        [MenuItem("AIDEN Tools/TactileUI/Add Basic Interaction UI GOs")]
        private static void AddBasicInteractionGOs()
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                Debug.LogWarning("No gameobject selected.");
                return;
            }

            MonoBehaviour physicalUIScript = GetPhysicalUIScript();

            if (physicalUIScript == null)
            {
                Debug.LogWarning("No Monobehavior that implement IPhysicalUI founded.");
                return;
            }

            Transform selectedTf = selectedObject.transform;
            RectTransform selectedRect = selectedObject.GetComponent<RectTransform>();

            Transform proximityTf = new GameObject("Proximity").transform;
            ResetTf(proximityTf);



            proximityTf.tag = Tags.ProximityUI;
            var sphereCol = proximityTf.AddComponent<SphereCollider>();
            sphereCol.isTrigger = true;
            var triggerA = proximityTf.AddComponent<TriggerTouch>();
            SerializedObject serializedTriggerA = new SerializedObject(triggerA);
            SerializedProperty isProximityProp = serializedTriggerA.FindProperty("_isProximity");
            SerializedProperty physicalUIScriptPropA = serializedTriggerA.FindProperty("_tactileUIScript");
            isProximityProp.boolValue = true;
            physicalUIScriptPropA.objectReferenceValue = physicalUIScript;

            serializedTriggerA.ApplyModifiedProperties();

            float x = selectedRect.rect.width;
            float y = selectedRect.rect.height;
            float maxXY = (x > y) ? x : y;
            sphereCol.radius = maxXY * .5f;


            Transform activeTf = new GameObject("Interaction").transform;
            ResetTf(activeTf);

            activeTf.tag = Tags.InteractionUI;
            var boxCol = activeTf.AddComponent<BoxCollider>();
            boxCol.isTrigger = true;
            var triggerB = activeTf.AddComponent<TriggerTouch>();
            SerializedObject serializedTriggerB = new SerializedObject(triggerB);
            SerializedProperty physicalUIScriptPropB = serializedTriggerB.FindProperty("_tactileUIScript");
            physicalUIScriptPropB.objectReferenceValue = physicalUIScript;

            serializedTriggerB.ApplyModifiedProperties();

            boxCol.size = new Vector3(x, y, 80f);
            boxCol.center = new Vector3(0, 0, 80f / 2);

            Debug.Log("Basic Interaction UI GOs added !");


            void ResetTf(Transform tf)
            {
                tf.parent = selectedTf;
                tf.localPosition = Vector3.zero;
                tf.localRotation = Quaternion.identity;
                tf.localScale = new Vector3(1f, 1f, 1f);

                tf.gameObject.layer = Layers.UI;
            }

            MonoBehaviour GetPhysicalUIScript()
            {
                var monos = selectedObject.GetComponents<MonoBehaviour>();

                foreach (MonoBehaviour script in monos)
                {
                    if (script is ITouchUI)
                    {
                        return script;
                    }
                }

                return null;
            }
        }
    }
}