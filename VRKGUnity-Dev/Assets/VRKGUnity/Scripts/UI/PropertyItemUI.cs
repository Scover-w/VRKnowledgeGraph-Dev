using AIDEN.TactileUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropertyItemUI : MonoBehaviour
{
    public ScrollItemUI ScrollItemUI {  get { return _scrollItemUI; } }

    public List<string> Namespaces { get { return _namespaces; } }
    public string Value { get { return _value; } }
    
    [SerializeField]
    ScrollItemUI _scrollItemUI;

    [SerializeField]
    GameObject _uriTxtPf;

    [SerializeField]
    RectTransform _uriContainerRect;

    [SerializeField]
    TMP_Text _valueTxt;

    List<string> _namespaces;

    NodeInfoUI _nodeInfoUI;

    bool _isSelected = false;

    string _value;


    public void Load(NodeInfoUI nodeInfoUI, string namespce, string value)
    {
        _namespaces = new();
        _nodeInfoUI = nodeInfoUI;
        _value = value;
        _namespaces.Add(namespce);

        _valueTxt.text = _value;

        AddNamespaceText(namespce);
    }

    public void AddNamespace(string txt)
    {
        _namespaces.Add(txt);
        AddNamespaceText(txt);
    }

    private void AddNamespaceText(string txt)
    {
        var go = Instantiate(_uriTxtPf, _uriContainerRect);
        TMP_Text tmp = go.GetComponent<TMP_Text>();
        tmp.text = txt;
    }

    public void OnClick()
    {
        _nodeInfoUI.DisplayProperty(this);
    }
}
