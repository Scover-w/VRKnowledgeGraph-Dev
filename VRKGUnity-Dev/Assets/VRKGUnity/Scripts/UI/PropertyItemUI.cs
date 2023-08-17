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

    public string Value { get { return _value; } }

    [SerializeField]
    GameObject _uriTxtPf;

    [SerializeField]
    RectTransform _uriContainerRect;

    [SerializeField]
    TMP_Text _valueTxt;

    [SerializeField]
    Image _selectedImg;

    [SerializeField]
    List<Collider> _interactionColliders;

    NodeInfoUI _nodeInfoUI;

    bool _isSelected = false;

    string _value;


    public void Load(NodeInfoUI nodeInfoUI, string namespce, string value)
    {
        _nodeInfoUI = nodeInfoUI;
        _value = value;

        _valueTxt.text = _value;

        AddNamespaceText(namespce);

        _selectedImg.enabled = false;
    }

    public void AddNamespace(string txt)
    {
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
