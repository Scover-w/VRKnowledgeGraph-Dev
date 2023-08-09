using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KGUIController : MonoBehaviour
{
    [SerializeField]
    GameObject _kguiPf;

    [SerializeField]
    Transform _uiPositionTf;

    [SerializeField]
    InputActionReference _displayUIActionRef;

    InputAction _displayUIAction;

    KGUI _currentKGUI;
    GameObject _currentUIGo;

    Transform _tf;
    bool _isUIDisplayed = false;

    private void Awake()
    {
        _tf = transform;
        _displayUIAction = _displayUIActionRef.action;
    }

    private void Start()
    {
        CreateNewUI();
    }

    private void OnEnable()
    {
        _displayUIAction.performed += SwitchDisplayUI;
    }

    private void OnDisable()
    {
        _displayUIAction.performed -= SwitchDisplayUI;
    }

    public void UIHasBeenDetached()
    {
        CreateNewUI();
        _isUIDisplayed = false;
    }

    private void CreateNewUI()
    {
        _currentUIGo = Instantiate(_kguiPf, _tf);
        _currentKGUI = _currentUIGo.GetComponent<KGUI>();
        _currentKGUI.KGUIController = this;
        _currentUIGo.SetActive(false);
        
        var tf = _currentUIGo.transform;
        tf.position = _uiPositionTf.position;
        tf.localRotation = _uiPositionTf.localRotation;
    }

    private void SwitchDisplayUI(InputAction.CallbackContext context)
    {
        _isUIDisplayed = !_isUIDisplayed;
        _currentUIGo.SetActive(_isUIDisplayed);
    }
}
