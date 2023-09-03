using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDENUI : MonoBehaviour
{
    [SerializeField]
    GameObject _micGo;

    [SerializeField]
    GameObject _waitGo;


    public void DisplayMic()
    {
        _micGo.SetActive(true);
        _waitGo.SetActive(false);
    }

    public void DisplayWait()
    {
        _micGo.SetActive(false);
        _waitGo.SetActive(true);
    }

    public void DisplayNone()
    {
        _micGo.SetActive(false);
        _waitGo.SetActive(false);
    }

}
