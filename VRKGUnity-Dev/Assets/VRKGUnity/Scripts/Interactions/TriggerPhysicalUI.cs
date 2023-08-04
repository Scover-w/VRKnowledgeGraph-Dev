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

    private void OnTriggerEnter(Collider touchCollider)
    {
        if (_isProximity && !touchCollider.CompareTag(Tags.ProximityUI))
            return;

        if (!_isProximity && !touchCollider.CompareTag(Tags.InteractionUI))
            return;

        _physicalUI.TriggerEnter(_isProximity, touchCollider);
    }

    private void OnTriggerExit(Collider touchCollider)
    {
        if (_isProximity && !touchCollider.CompareTag(Tags.ProximityUI))
            return;

        if (!_isProximity && !touchCollider.CompareTag(Tags.InteractionUI))
            return;

        _physicalUI.TriggerExit(_isProximity, touchCollider);
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