using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest2 : MonoBehaviour
{
    [SerializeField]
    bool _isProximity;

    [SerializeField]
    PhysicalButtonUI _physicalBtnTest;


    private void OnTriggerEnter(Collider other)
    {
        _physicalBtnTest.TriggerEnter(_isProximity, other);
    }

    private void OnTriggerExit(Collider other)
    {
        _physicalBtnTest.TriggerExit(_isProximity, other);
    }
}
