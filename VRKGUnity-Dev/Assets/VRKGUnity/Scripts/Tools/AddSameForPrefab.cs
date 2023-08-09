using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class AddSameForPrefab : MonoBehaviour
{
    [SerializeField]
    List<GameObject> _gos;

    [SerializeField]
    GameObject _prefab;

    [ContextMenu("Add")]
    private void Add()
    {
        foreach (GameObject go in _gos) 
        { 
            var parent = go.transform.parent;

            var rectTf = go.GetComponent<RectTransform>();

            var addedGo = PrefabUtility.InstantiatePrefab(_prefab, parent) as GameObject;
            addedGo.name = go.name;
            
            var addedRectTf = addedGo.GetComponent<RectTransform>();

            CopyRectTransform(rectTf, addedRectTf);

        }
    }

    private void CopyRectTransform(RectTransform source, RectTransform destination)
    {
        destination.anchorMin = source.anchorMin;
        destination.anchorMax = source.anchorMax;
        destination.anchoredPosition = source.anchoredPosition;
        destination.anchoredPosition3D = source.anchoredPosition3D;
        destination.sizeDelta = source.sizeDelta;
        destination.pivot = source.pivot;
        destination.localRotation = source.localRotation;
        destination.localScale = source.localScale;
    }
}
#endif