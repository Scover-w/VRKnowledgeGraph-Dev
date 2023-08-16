using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScollAddTest : MonoBehaviour
{
    [SerializeField]
    ScrollUI _scrollUI;

    [SerializeField]
    GameObject _itemPf;

    List<ItemTests> _itemsTests;

    private void Awake()
    {
        _itemsTests = new();
    }

    [ContextMenu("ForceUpdate")]
    public void ForceUpdate()
    {
        var rect = _scrollUI.GetComponent<RectTransform>();
        //rect.ForceUpdateRectTransforms();
        LayoutRebuilder.MarkLayoutForRebuild(rect);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    [ContextMenu("AddItem")]
    public void AddItem()
    {
        CreateItem();
        _scrollUI.AddItem(_itemsTests.Last().ScrollItem);
    }

    [ContextMenu("AddItems")]
    public void AddItems()
    {
        for(int i = 0; i < 100; i++)
        {
            CreateItem();
        }

        List<ScrollItem> items = new List<ScrollItem>();

        foreach (var item in _itemsTests)
            items.Add(item.ScrollItem);

        
        _scrollUI.AddItems(items);
    }

    private void CreateItem()
    {
        var go = Instantiate(_itemPf, _scrollUI.ItemContainer);

        var itemTestUI = go.GetComponent<ItemTests>();
        itemTestUI.Load(GenerateRandomString(Random.Range(0, 200)));

        var colliders = itemTestUI.Colliders;

        var scollItem = new ScrollItem(go.GetComponent<RectTransform>(), colliders);
        itemTestUI.ScrollItem = scollItem;
      
        _itemsTests.Add(itemTestUI);

    }

    static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
        StringBuilder result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[Random.Range(0, chars.Length)]);
        }

        return result.ToString();
    }




}
