using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropertyItemUI : MonoBehaviour
{
    public List<Collider> Colliders => _interactionColliders;

    public ScrollItem ScrollItem { get; set; }

    public string Uri { get { return _uri; } }
    public string Value { get { return _value; } }


    [SerializeField]
    TMP_Text _uriTxt;

    [SerializeField]
    TMP_Text _valueTxt;

    [SerializeField]
    Image _selectedImg;

    [SerializeField]
    List<Collider> _interactionColliders;

    NodeInfoUI _nodeInfoUI;

    bool _isSelected = false;
    string _uri;
    string _value;


    public void Load(NodeInfoUI nodeInfoUI, KeyValuePair<string,string> uriAndValue)
    {
        _nodeInfoUI = nodeInfoUI;
        _uri = uriAndValue.Key;
        _value = uriAndValue.Key;

        _uriTxt.text = _uri;
        _valueTxt.text = _value;

        _selectedImg.enabled = false;
    }

    public void OnClick()
    {
        _isSelected = !_isSelected;

        _selectedImg.enabled = _isSelected;

        if (_isSelected) 
            _nodeInfoUI.DisplayProperty(this);
        else
            _nodeInfoUI.HideProperty();
    }

    public void Unselect()
    {
        if (!_isSelected)
            return;

        _isSelected = false;
        _selectedImg.enabled = false;
    }

}
