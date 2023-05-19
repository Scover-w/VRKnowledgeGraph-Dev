using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NodeMaterial : MonoBehaviour
{

    [SerializeField]
    MeshRenderer _renderer;

    [SerializeField]
    Material _defaultMat;

    [SerializeField]
    Material _hoveredMat;

    [SerializeField]
    Material _selectedMat;

    bool _isHovered = false;
    bool _isSelected = false;

    private void OnEnable()
    {
        _isHovered = false;
        _isSelected = false;

        _renderer.sharedMaterial = _defaultMat;
    }

    public void OnEnterHover(HoverEnterEventArgs args)
    {
        _isHovered = true;
        UpdateMaterial();
    }

    public void OnExitHover(HoverExitEventArgs args)
    {
        _isHovered = false;
        UpdateMaterial();
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        _isHovered = true;
        UpdateMaterial();
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        _isHovered = false;
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if(_isSelected)
            _renderer.sharedMaterial = _selectedMat;
        else if(_isHovered)
            _renderer.sharedMaterial = _hoveredMat;
        else
            _renderer.sharedMaterial = _defaultMat;
    }
}
