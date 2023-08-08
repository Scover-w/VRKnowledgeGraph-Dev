using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIDEN.TactileUI
{
    public class TouchActivator : MonoBehaviour
    {
        [SerializeField]
        GameObject _touchInteractorGo;

        [SerializeField]
        Transform _tf;

        [SerializeField]
        float _presenceDetectionDistance = 0.2f;

        [SerializeField]
        float _touchDistance = 0.08f;


        Transform _touchInteractorTf;

        RaycastHit hit;
        int _presenceUILayer;

        bool _isActive;
        float _currentDistance;

        private void Awake()
        {
            _presenceUILayer = 1 << Layers.PresenceUI;
            _tf = transform;
            _touchInteractorTf = _touchInteractorGo.transform;

            _isActive = false;
            _touchInteractorGo.SetActive(false);
        }




        void Update()
        {
            if(!Physics.Raycast(_tf.position, _tf.forward, out hit, _presenceDetectionDistance, _presenceUILayer))
            {
                if (!_isActive)
                    return;

                _isActive = false;
                _touchInteractorGo.SetActive(false);
                return;
            }


            float hitDistance = hit.distance;
            float newDistance = (hitDistance < _touchDistance ? hitDistance : _touchDistance);

            if (!_isActive)
            {
                _isActive = true;
                _touchInteractorGo.SetActive(true);

                
                _currentDistance = newDistance;
                _touchInteractorTf.localPosition = Vector3.forward * _currentDistance;
                return;
            }


            if (newDistance == _currentDistance)
                return;

            _currentDistance = newDistance;
            _touchInteractorTf.localPosition = Vector3.forward * _currentDistance;
        }
    }
}