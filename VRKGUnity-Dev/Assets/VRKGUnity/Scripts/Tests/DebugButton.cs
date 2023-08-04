using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Click");
    }

    public void OnEnter()
    {
        Debug.Log("OnEnter");
    }

    public void OnExit()
    {
        Debug.Log("OnExit");
    }
}
