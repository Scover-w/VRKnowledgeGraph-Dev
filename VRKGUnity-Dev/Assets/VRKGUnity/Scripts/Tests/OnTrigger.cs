using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour
{
    [SerializeField]
    UnityEvent<Collider> _onTriggerEnter;

    [SerializeField]
    UnityEvent<Collider> _onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        _onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _onTriggerExit?.Invoke(other);
    }

}
