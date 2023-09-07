using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskDirectActivator : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GameObject _deskDirectInteractorGo;

    [SerializeField]
    bool _isLeft;

    bool _isFirstEndSimu = true;
    bool _canInteractorBeEnabled = false;

    bool _isInteractorInGraph = false;

    private void OnEnable()
    {
        _deskDirectInteractorGo.SetActive(false);   
    }

    private void Start()
    {
        if (_isLeft)
            _referenceHolderSo.LeftDeskDirectActivator = this;
        else
            _referenceHolderSo.RightDeskDirectActivator = this;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Tags.DeskGraph))
            return;

        _isInteractorInGraph = true;
        UpdateInteractor();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tags.DeskGraph))
            return;

        _isInteractorInGraph = false;
        UpdateInteractor();
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

                if (_isFirstEndSimu)
                {
                    SetCanInteractorState(true);
                    _isFirstEndSimu = false;
                }
                else
                    SetCanInteractorState(true);
                break;
            case GraphUpdateType.BeforeSwitchMode:
                SetCanInteractorState(false);
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                SetCanInteractorState(true);
                break;
            case GraphUpdateType.AfterSwitchModeToImmersion:
                SetCanInteractorState(true);
                break;
        }

    }

    private void SetCanInteractorState(bool canInteractorBeEnabled)
    {
        _canInteractorBeEnabled = canInteractorBeEnabled;
        UpdateInteractor();
    }

    private void UpdateInteractor()
    {
        _deskDirectInteractorGo.SetActive(_canInteractorBeEnabled && _isInteractorInGraph);
    }
}
