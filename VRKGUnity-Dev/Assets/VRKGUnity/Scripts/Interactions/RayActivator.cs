using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayActivator : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GameObject _lenRayInteractorGo;

    [SerializeField]
    bool _isLeft;

    bool _isFirstEndSimu = true;

    bool _canInteractorBeEnabled = false;
    bool _doesDeskInteractorWantToHideIt = false;


    private void Start()
    {
        if (_isLeft)
            _referenceHolderSo.LeftLensRayActivator = this;
        else
            _referenceHolderSo.RightLensRayActivator = this;

        Invoke(nameof(OnDelayedStart), 1f);
    }

    private void OnDelayedStart()
    {
        _lenRayInteractorGo.SetActive(false);
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

        _lenRayInteractorGo.SetActive(_canInteractorBeEnabled && !_doesDeskInteractorWantToHideIt);
    }

    public void SetDeskInteractorWantToHideIt(bool hideIt)
    {
        _doesDeskInteractorWantToHideIt = hideIt;
        _lenRayInteractorGo.SetActive(_canInteractorBeEnabled && !hideIt);
    }
}
