using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensRayActivator : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GameObject _lenRayInteractorGo;

    [SerializeField]
    bool _isLeft;

    Transform _tf;
    RaycastHit hit;
    int _lensGraphLayer;

    bool _isFirstEndSimu = true;

    bool _canInteractorBeEnabled = false;
    bool _isInteractorEnabled = false;


    private void Awake()
    {
        _lensGraphLayer = 1 << Layers.LensGraph;
        _tf = transform;
    }

    private void Start()
    {
        if (_isLeft)
            _referenceHolderSo.LeftLensRayActivator.Value = this;
        else
            _referenceHolderSo.RightLensRayActivator.Value = this;

        Invoke(nameof(OnDelayedStart), 1f);
    }

    private void OnDelayedStart()
    {
        _lenRayInteractorGo.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (!_canInteractorBeEnabled)
            return;

        if (!Physics.Raycast(_tf.position, _tf.forward, out hit, 50f, _lensGraphLayer))
        {

            if (!_isInteractorEnabled)
                return;

            _lenRayInteractorGo.SetActive(false);
            _isInteractorEnabled = false;
            return;
        }

        if (_isInteractorEnabled)
            return;

        _lenRayInteractorGo.SetActive(true);
        _isInteractorEnabled = true;
    }


    public void OnGraphUpdated(GraphUpdateType updateType)
    {

        switch (updateType)
        {
            case GraphUpdateType.RetrievingFromDb:
                SetCanInteractorState(false);
                break;
            case GraphUpdateType.BeforeSimulationStart:
                SetCanInteractorState(false);
                break;
            case GraphUpdateType.AfterSimulationHasStopped:

                if(_isFirstEndSimu)
                {
                    SetCanInteractorState(true);
                    _isFirstEndSimu = false;
                }
                else
                    SetCanInteractorState(false);
                break;
            case GraphUpdateType.BeforeSwitchMode:
                SetCanInteractorState(false);
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                SetCanInteractorState(true);
                break;
            case GraphUpdateType.AfterSwitchModeToImmersion:
                SetCanInteractorState(false);
                break;
        }

    }

    private void SetCanInteractorState(bool canInteractorBeEnabled)
    {
        _canInteractorBeEnabled = canInteractorBeEnabled;

        if(!_canInteractorBeEnabled && _isInteractorEnabled)
        {
            _isInteractorEnabled = true;
            _lenRayInteractorGo.SetActive(false);
        }

    }
}
