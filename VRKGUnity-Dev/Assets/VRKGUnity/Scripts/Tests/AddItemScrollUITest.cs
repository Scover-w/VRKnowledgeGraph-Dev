using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddItemScrollUITest : MonoBehaviour
{
    [SerializeField]
    ScrollUI _scrollUI;

    [SerializeField]
    GameObject _pf;

    List<ScrollItem> scrollItems = new List<ScrollItem>();


    [ContextMenu("AddItem")]
    public void AddItem()
    {
        var go = Instantiate(_pf, _scrollUI.ItemContainer);
        var rectTf = go.GetComponent<RectTransform>();
        var collider = go.GetComponent<BoxCollider>();

        List<Collider> colliders = new List<Collider>
        {
            collider
        };

        ScrollItem item = new ScrollItem(rectTf, colliders);
        scrollItems.Add(item);

        _scrollUI.AddItem(item);
    }

    [ContextMenu("RemoveItem")]
    public void RemoveItem()
    {
        var itemToRemove = scrollItems.First();

        if (itemToRemove == null)
            return;

        scrollItems.Remove(itemToRemove);
        _scrollUI.RemoveItem(itemToRemove);


    }
}
