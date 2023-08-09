using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KGUIController : MonoBehaviour
{
    [SerializeField]
    GameObject _kguiPf;

    [SerializeField]
    Transform _uiPosition;

    [SerializeField]
    InputActionReference _displayUIActionRef;

    InputAction _displayUIAction;

    KGUI _currentKGUI;
    GameObject _currentUIGo;

    bool _isUIDisplayed = false;

    private void Awake()
    {
        _displayUIAction = _displayUIActionRef.action;
    }

    private void Start()
    {
        CreateNewUI();
    }

    private void OnEnable()
    {
        _displayUIAction.performed += SwitchDisplayUI;
        //_displayUIAction.Enable();
    }

    private void OnDisable()
    {
        _displayUIAction.performed -= SwitchDisplayUI;
        //_displayUIAction.Disable();
    }

    public void UIHasBeenDetached()
    {
        CreateNewUI();
        _isUIDisplayed = false;
    }

    private void CreateNewUI()
    {
        _currentUIGo = Instantiate(_kguiPf);
        _currentKGUI = _currentUIGo.GetComponent<KGUI>();
        _currentUIGo.SetActive(false);
    }

    private void SwitchDisplayUI(InputAction.CallbackContext context)
    {
        _isUIDisplayed = !_isUIDisplayed;
        _currentUIGo.SetActive(_isUIDisplayed);
    }
}
