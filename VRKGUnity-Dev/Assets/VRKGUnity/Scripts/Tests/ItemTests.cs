using AIDEN.TactileUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTests : MonoBehaviour
{
    public List<Collider> Colliders => _interactionColliders;

    public ScrollItem ScrollItem { get; set; }


    [SerializeField]
    TMP_Text _textTxt;


    [SerializeField]
    List<Collider> _interactionColliders;

    NodeInfoUI _nodeInfoUI;

    bool _isSelected = false;
    string _uri;
    string _value;


    public void Load(string txt)
    {
        _textTxt.text = txt;

        Invoke(nameof(UpdateColliders), 1f);
    }


    private void UpdateColliders()
    {
        Vector2 size = GetComponent<RectTransform>().sizeDelta;

        foreach(var collider in _interactionColliders)
        {
            BoxCollider box = collider as BoxCollider;
            var boxSize = box.size;
            boxSize.y = size.y;
            box.size = boxSize;
        }
    }
}
