using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTouchTest : MonoBehaviour
{
    [SerializeField]
    GameObject _pageA;

    [SerializeField]
    GameObject _pageB;

    public void DisplayPageA()
    {
        Debug.Log("DisplayPageA");

        _pageA.SetActive(true);
        _pageB.SetActive(false);
    }

    public void DisplayPageB()
    {
        Debug.Log("DisplayPageB");

        _pageA.SetActive(false);
        _pageB.SetActive(true);
    }
}
