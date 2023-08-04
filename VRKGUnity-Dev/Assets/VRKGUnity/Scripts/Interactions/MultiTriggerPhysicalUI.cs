using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTriggerPhysicalUI : MonoBehaviour
{
    [SerializeField]
    bool _isProximity = false;

    [SerializeField]
    MonoBehaviour _physicalUIScript;

    IPhysicalUI _physicalUI;

    int _nbIn = 0;

    private void Start()
    {
        _physicalUI = _physicalUIScript.GetComponent<IPhysicalUI>();

        if (_physicalUI == null)
            Debug.LogError("_physicalUIScript don't implement the IPhysicalUI interface.");
    }


    public void OnChildTriggerEnter(Collider touchCollider)
    {
        if (_isProximity && !touchCollider.CompareTag(Tags.ProximityUI))
            return;

        if (!_isProximity && !touchCollider.CompareTag(Tags.InteractionUI))
            return;

        _nbIn++;

        if (_nbIn > 1)
            return;

        Debug.Log("OnTrigger TriggerEnter");
        _physicalUI.TriggerEnter(_isProximity, touchCollider);
    }

    public void OnChildTriggerExit(Collider touchCollider)
    {
        if (_isProximity && !touchCollider.CompareTag(Tags.ProximityUI))
            return;

        if (!_isProximity && !touchCollider.CompareTag(Tags.InteractionUI))
            return;

        _nbIn--;

        if (_nbIn > 0)
            return;

        Debug.Log("OnTrigger TriggerExit");
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
