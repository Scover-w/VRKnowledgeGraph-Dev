using UnityEngine;

namespace AIDEN.TactileUI
{
    public class MultiTriggerTouch : MonoBehaviour
    {
        [SerializeField]
        bool _isProximity = false;

        [SerializeField]
        MonoBehaviour _physicalUIScript;

        ITouchUI _iTouchUI;

        int _nbIn = 0;

        private void Start()
        {
            _iTouchUI = _physicalUIScript.GetComponent<ITouchUI>();

            if (_iTouchUI == null)
                Debug.LogError("_physicalUIScript don't implement the ITactileUI interface.");
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
            _iTouchUI.TriggerEnter(_isProximity, touchCollider.transform.parent);
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
            _iTouchUI.TriggerExit(_isProximity, touchCollider.transform.parent);
        }

        private void OnValidate()
        {
            if (_physicalUIScript == null)
                return;

            if (_physicalUIScript.GetComponent<ITouchUI>() != null)
                return;

            _physicalUIScript = null;
            Debug.LogError("_physicalUIScript don't implement the IPhysicalUI interface.");
        }
    }
}