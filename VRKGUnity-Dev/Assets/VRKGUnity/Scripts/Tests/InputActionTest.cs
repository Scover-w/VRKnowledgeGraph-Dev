using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Wave.Native;
using Wave.Essence;

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

    private void Update()
    {
        
    }






    private void UpdateTouchs()
    {
        // /!\ Don't work if controllers not in the fov during the launch of the app


        //Debug.Log("-------");

        if (WXRDevice.ButtonTouch(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Thumbstick))
            Debug.Log("Hope 1");

        if (WXRDevice.ButtonTouch(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_2))
            Debug.Log("Hope 2");

        if (WXRDevice.ButtonTouch(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Grip))
            Debug.Log("Hope 3");

        if (WXRDevice.ButtonTouching(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Grip))
            Debug.Log("Hope 4");

        if (WXRDevice.ButtonTouch(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Trigger))
            Debug.Log("Hope 5");

        if (WXRDevice.ButtonTouching(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Trigger))
            Debug.Log("Hope 6");

        //if (WXRDevice.(WVR_DeviceType.WVR_DeviceType_Controller_Right, WVR_InputId.WVR_InputId_Alias1_Grip))
        //    Debug.Log("Hope 3");
    }


}
