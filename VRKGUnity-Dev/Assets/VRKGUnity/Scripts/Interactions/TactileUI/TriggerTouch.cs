using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AIDEN.TactileUI
{
    public class TriggerTouch : MonoBehaviour
    {
        [SerializeField]
        bool _isProximity = false;

        [SerializeField]
        MonoBehaviour _tactileUIScript;

        ITactileUI _iTactileUI;

        private void Start()
        {
            _iTactileUI = _tactileUIScript.GetComponent<ITactileUI>();

            if (_iTactileUI == null)
                Debug.LogError("_physicalUIScript don't implement the IPhysicalUI interface.");
        }

        private void OnTriggerEnter(Collider touchCollider)
        {
            if (_isProximity && !touchCollider.CompareTag(Tags.ProximityUI))
                return;

            if (!_isProximity && !touchCollider.CompareTag(Tags.InteractionUI))
                return;

            _iTactileUI.TriggerEnter(_isProximity, touchCollider);
        }

        private void OnTriggerExit(Collider touchCollider)
        {
            if (_isProximity && !touchCollider.CompareTag(Tags.ProximityUI))
                return;

            if (!_isProximity && !touchCollider.CompareTag(Tags.InteractionUI))
                return;

            _iTactileUI.TriggerExit(_isProximity, touchCollider);
        }

        private void OnValidate()
        {
            if (_tactileUIScript == null)
                return;

            if (_tactileUIScript.GetComponent<ITactileUI>() != null)
                return;

            _tactileUIScript = null;
            Debug.LogError("_physicalUIScript don't implement the IPhysicalUI interface.");
        }
    }
}
