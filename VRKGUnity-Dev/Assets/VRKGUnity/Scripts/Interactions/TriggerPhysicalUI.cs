using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TriggerPhysicalUI : MonoBehaviour
{
    [SerializeField]
    bool _isProximity = false;

    [SerializeField]
    MonoBehaviour _physicalUIScript;

    IPhysicalUI _physicalUI;

    private void Start()
    {
        _physicalUI = _physicalUIScript.GetComponent<IPhysicalUI>();

        if (_physicalUI == null)
            Debug.LogError("_physicalUIScript don't implement the IPhysicalUI interface.");
    }

    private void OnTriggerEnter(Collider other)
    {
        _physicalUI.TriggerEnter(_isProximity, other);
    }

    private void OnTriggerExit(Collider other)
    {
        _physicalUI.TriggerExit(_isProximity, other);
    }

    private void OnValidate()
    {
        if (_physicalUIScript == null)
            return;

        if (_physicalUIScript.GetComponent<IPhysicalUI>() != null)
            return;

        _physicalUIScript = null;
        Debug.LogError("_physicalUIScript don't implement the IPhysicalUI interface.");
    }
}

public interface IPhysicalUI
{
    public void TriggerEnter(bool isProximity, Collider collider);
    public void TriggerExit(bool isProximity, Collider collider);
}
