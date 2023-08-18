using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTest : MonoBehaviour
{

    private delegate void TestDelegate();
    TestDelegate testDelegate;


    Dictionary<string, TestDelegate> testDelegates;

    [ContextMenu("ResetDict")]
    private void ResetDict()
    {
        testDelegates = new();
    }

    [ContextMenu("TryAddToDict")]
    private void TryAddToDict()
    {
        if (!testDelegates.TryGetValue("test", out TestDelegate testDelegate))
        {
            testDelegates.Add("test", testDelegate);
        }

        testDelegate += Test;
        testDelegates["test"] = testDelegate;

        if (!testDelegates.TryGetValue("test", out TestDelegate testDelegateB))
        {
            Debug.Log("Weird");
            return;
        }

        Debug.Log(testDelegateB);
    }

    [ContextMenu("TryAddToDictB")]
    private void TryAddToDictB()
    {
        if(!testDelegates.TryGetValue("test", out TestDelegate testDelegate))
        {
            testDelegates.Add("test", testDelegate);
        }

        testDelegate += TestB;
        testDelegates["test"] = testDelegate;
    }



    [ContextMenu("TryRemoveToDict")]
    private void TryRemoveToDict()
    {
        if (!testDelegates.TryGetValue("test", out TestDelegate testDelegate))
        {
            Debug.Log("No test in dict");
            return;
        }

        testDelegate -= Test;
        testDelegates["test"] = testDelegate;
        Debug.Log("testDelegate " + testDelegate);
    }

    [ContextMenu("TryRemoveToDictB")]
    private void TryRemoveToDictB()
    {
        if (!testDelegates.TryGetValue("test", out TestDelegate testDelegate))
        {
            Debug.Log("No test in dict");
            return;
        }

        testDelegate -= TestB;
        testDelegates["test"] = testDelegate;

        Debug.Log("testDelegate " + testDelegate);
    }


    [ContextMenu("CallDict")]
    private void CallDict()
    {
        if (!testDelegates.TryGetValue("test", out TestDelegate testDelegate))
        {
            Debug.Log("No test in dict");
            return;
        }

        testDelegate?.Invoke();
    }




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

    public void TestB()
    {
        Debug.Log("TestB");
    }

}
