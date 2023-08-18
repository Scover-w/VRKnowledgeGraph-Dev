using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTest : MonoBehaviour
{

    private delegate void TestDelegate();
    TestDelegate testDelegate;


    [ContextMenu("ResetDelegate")]
    private void ResetDelegate()
    {
        testDelegate = null;
    }

    [ContextMenu("CallDelegate")]
    private void CallDelegate()
    {
        testDelegate?.Invoke();
    }

    [ContextMenu("AddToDelegate")]
    private void AddToDelegate()
    {
        testDelegate += Test;
    }

    [ContextMenu("RemoveToDelegate")]
    private void RemoveToDelegate()
    {
        testDelegate -= Test;
    }

    [ContextMenu("CheckDelegate")]
    private void CheckDelegate()
    {
        Debug.Log(testDelegate == null);
    }

    public void Test()
    {
        Debug.Log("Test");
    }

}
