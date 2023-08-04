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

        ITouchUI _iTouchUI;

        private void Start()
        {
            _iTouchUI = _tactileUIScript.GetComponent<ITouchUI>();

            if (_iTouchUI == null)
                Debug.LogError("_physicalUIScript don't implement the IPhysicalUI interface.");
        }

        private void OnTriggerEnter(Collider touchCollider)
        {
            if (_isProximity && !touchCollider.CompareTag(Tags.ProximityUI))
                return;

            if (!_isProximity && !touchCollider.CompareTag(Tags.InteractionUI))
                return;

            _iTouchUI.TriggerEnter(_isProximity, touchCollider.transform.parent);
        }

        private void OnTriggerExit(Collider touchCollider)
        {
            if (_isProximity && !touchCollider.CompareTag(Tags.ProximityUI))
                return;

            if (!_isProximity && !touchCollider.CompareTag(Tags.InteractionUI))
                return;

            _iTouchUI.TriggerExit(_isProximity, touchCollider.transform.parent);
        }

        private void OnValidate()
        {
            if (_tactileUIScript == null)
                return;

            if (_tactileUIScript.GetComponent<ITouchUI>() != null)
                return;

            _tactileUIScript = null;
            Debug.LogError("_physicalUIScript don't implement the IPhysicalUI interface.");
        }
    }
}
