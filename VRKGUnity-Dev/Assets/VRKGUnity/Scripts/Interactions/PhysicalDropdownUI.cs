using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PhysicalTabControllerUI;

public class PhysicalDropdownUI : MonoBehaviour, IPhysicalUI
{
    [SerializeField]
    PhysicalDropdownControllerUI _controller;

    [SerializeField]
    Image _checkMarkImg;

    [SerializeField]
    TMP_Text _label;

    [SerializeField]
    string _value;

    GameObject _checkMarkGo;
    Transform _touchTf;
    TouchInteraction _touchInter;
    ColorTab _colorTab;

    bool _isSelected = false;


    public void TriggerEnter(bool isProximity, Collider touchCollider)
    {
        Debug.Log("TriggerEnter");
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _touchTf = touchCollider.transform.parent;
            _touchInter = _touchTf.GetComponent<TouchInteraction>();
            UpdateColor(InteractionStateUI.InProximity);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.ActiveUI))
        {
            TryClick();
        }
    }

    private void TryClick()
    {
        UpdateColor(InteractionStateUI.Active);

        if (_touchInter != null)
            _touchInter.ActiveBtn(true, this);

        _controller.CloseFromDropdown(_value);
    }

    public void TriggerExit(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            UpdateColor(InteractionStateUI.Normal);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.ActiveUI))
        {
            if (_touchInter != null)
                _touchInter.ActiveBtn(false, this);

            UpdateColor(InteractionStateUI.Normal);
        }
    }

    private void UpdateColor(InteractionStateUI interactionState)
    {
        switch (interactionState)
        {
            case InteractionStateUI.Normal:
                _label.color = _colorTab.NormalColor;
                _checkMarkImg.color = _colorTab.NormalColor;
                break;
            case InteractionStateUI.InProximity:
                _label.color = _colorTab.ProximityColor;
                _checkMarkImg.color = _colorTab.ProximityColor;
                break;
            case InteractionStateUI.Active:
                _label.color = _colorTab.ActivatedColor;
                _checkMarkImg.color = _colorTab.ActivatedColor;
                break;
        }
    }

    public void ResfreshValue(string value, ColorTab colorTab)
    {
        _colorTab = colorTab;
        UpdateColor(InteractionStateUI.Normal);

        _isSelected = (_value == value);

        if(_checkMarkGo == null)
            _checkMarkGo = _checkMarkImg.gameObject;
        _checkMarkGo.SetActive(_isSelected);
    }

    private void OnValidate()
    {
        if (_label == null)
            return;

        _label.text = _value;
    }
}
