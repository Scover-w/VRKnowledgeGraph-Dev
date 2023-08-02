using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PhysicalTabControllerUI;

public class PhysicalDropdownControllerUI : MonoBehaviour, IPhysicalUI
{
    [SerializeField]
    List<ColorTab> _colorDropdown;

    [SerializeField]
    ColorTab _colorItem;

    [SerializeField]
    List<Image> _imgs;

    [SerializeField]
    GameObject _optionsContainerGo;

    [SerializeField]
    List<PhysicalDropdownUI> _dropdowns;

    [SerializeField]
    TMP_Text _label;

    [SerializeField]
    string _selectedValue;

    Transform _touchTf;
    TouchInteraction _touchInter;

    bool _isOpen = false;
    bool _canClick = true;


    private void OnEnable()
    {
        _isOpen = false;
        _optionsContainerGo.SetActive(_isOpen);
        UpdateColor(InteractionStateUI.Normal);
    }

    public void TriggerEnter(bool isProximity, Collider touchCollider)
    {
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
        if (!_canClick)
            return;

        _canClick = false;
        _isOpen = !_isOpen;
        _optionsContainerGo.SetActive(_isOpen);

        if (_isOpen)
            RefreshValues();

        UpdateColor(InteractionStateUI.Active);

        if (_touchInter != null)
            _touchInter.ActiveBtn(true, this);
    }

    public void TriggerExit(bool isProximity, Collider touchCollider)
    {
        if (isProximity && touchCollider.CompareTag(Tags.ProximityUI))
        {
            _canClick = true;
            UpdateColor(InteractionStateUI.Normal);
        }
        else if (!isProximity && touchCollider.CompareTag(Tags.ActiveUI))
        {
            if (_touchInter != null && !_canClick)
                _touchInter.ActiveBtn(false, this);

            UpdateColor(InteractionStateUI.Normal);
        }
    }

    public void CloseFromDropdown(string value)
    {
        _label.text = value;
        _selectedValue = value;

        RefreshValues();

        _isOpen = false;
        _optionsContainerGo.SetActive(false);


        // TODO : Link the the true datas
        Debug.Log("Select " + value);
    }

    private void UpdateColor(InteractionStateUI interactionState)
    {
        int nbImg = _imgs.Count;

        for (int i = 0; i < nbImg; i++)
        {
            Image img = _imgs[i];
            ColorTab colorTab = _colorDropdown[i];

            switch (interactionState)
            {
                case InteractionStateUI.Normal:
                    img.color = colorTab.NormalColor;
                    break;
                case InteractionStateUI.InProximity:
                    img.color = colorTab.ProximityColor;
                    break;
                case InteractionStateUI.Active:
                    img.color = colorTab.ActivatedColor;
                    break;
            }
        }
    }

    private void RefreshValues()
    {
        foreach(var dropdown in _dropdowns)
        {
            dropdown.ResfreshValue(_selectedValue, _colorItem);
        }
    }
}
