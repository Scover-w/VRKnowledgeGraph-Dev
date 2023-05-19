using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionTest : MonoBehaviour
{
    [SerializeField]
    InputActionProperty _inputActionBtn;
    [SerializeField]
    InputActionProperty _inputActionVector2;

    private void Awake()
    {
        _inputActionBtn.action.started += ctx => Test();

        _inputActionVector2.action.performed += ctx => Test2(ctx.ReadValue<Vector2>());
    }

    private void Test()
    {
        Debug.Log("Test Hope");
    }

    private void Test2(Vector2 v2)
    {
        Debug.Log("Test Hope " + v2.ToString());
    }


}
