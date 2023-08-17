using AIDEN.TactileUI;
using AngleSharp.Dom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAddTest : MonoBehaviour
{
    [SerializeField]
    ScrollViewUI _scrollUI;

    [SerializeField]
    GameObject _itemPf;

    List<ScrollItemUI> _itemsTests;

    public int NbItemToAdd = 100;

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
        _scrollUI.AddItem(_itemsTests.Last());
    }

    [ContextMenu("AddItems")]
    public void AddItems()
    {
        for (int i = 0; i < NbItemToAdd; i++)
        {
            CreateItem();
        }

        List<ScrollItemUI> items = new List<ScrollItemUI>();

        foreach (var item in _itemsTests)
            items.Add(item);


        _scrollUI.AddItems(items);
    }

    [ContextMenu("AddRemoveAdd")]
    public void AddRemoveAdd()
    {
        for (int i = 0; i < 25; i++)
        {
            CreateItem();
        }

        List<ScrollItemUI> items = new List<ScrollItemUI>();

        foreach (var item in _itemsTests)
            items.Add(item);


        _scrollUI.AddItems(items);

        RemoveAll();


        for (int i = 0; i < 15; i++)
        {
            CreateItem();
        }

        items = new List<ScrollItemUI>();

        foreach (var item in _itemsTests)
            items.Add(item);


        _scrollUI.AddItems(items);
    }

    private void CreateItem()
    {
        var go = Instantiate(_itemPf, _scrollUI.ItemContainer);

        var itemTestUI = go.GetComponent<ScrollItemUI>();
        var tmp = go.transform.GetChild(0).GetComponent<TMP_Text>();
        tmp.text = GenerateRandomString(Random.Range(0, 200));

        _itemsTests.Add(itemTestUI);
    }

    [ContextMenu("RemoveAll")]
    public void RemoveAll()
    {
        _scrollUI.RemoveItems(_itemsTests);
        _itemsTests = new();

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
