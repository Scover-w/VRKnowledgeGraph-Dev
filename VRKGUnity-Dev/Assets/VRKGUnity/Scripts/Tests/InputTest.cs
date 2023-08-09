using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    [SerializeField]
    GameObject _go;


    [SerializeField]
    InputActionReference _actionRef;

    InputAction _action;

    private void Awake()
    {
        _action = _actionRef.action;
    }

    private void OnEnable()
    {
        _action.performed += Switch;
    }

    private void OnDisable()
    {
        _action.performed -= Switch;
    }

    public void Switch(InputAction.CallbackContext context)
    {
        _go.SetActive(!_go.activeSelf);
    }
}
