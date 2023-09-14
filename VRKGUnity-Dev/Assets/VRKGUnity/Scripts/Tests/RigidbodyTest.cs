using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RigidbodyTest : MonoBehaviour
{
    [SerializeField]
    UnityEvent _triggerEnterEvent;

    [SerializeField]
    UnityEvent _triggerExitEvent;

    [SerializeField]
    UnityEvent _collisionEnterEvent;

    [SerializeField]
    UnityEvent _collisionExitEvent;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        _triggerEnterEvent?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        _triggerExitEvent?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        _collisionEnterEvent?.Invoke();
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("OnCollisionExit");
        _collisionExitEvent?.Invoke();
    }
}
