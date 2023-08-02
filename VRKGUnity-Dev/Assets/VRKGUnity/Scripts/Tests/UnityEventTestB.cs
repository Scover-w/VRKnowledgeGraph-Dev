using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEventTestB : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("OnClick");
    }


    public void OnWriteBro(char v)
    {
        Debug.Log("OnWrite " + v);
    }
}
