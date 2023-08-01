using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    UnityEvent _onTriggerEnter;

    [SerializeField]
    UnityEvent _onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        _onTriggerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        _onTriggerExit?.Invoke();
    }
}
