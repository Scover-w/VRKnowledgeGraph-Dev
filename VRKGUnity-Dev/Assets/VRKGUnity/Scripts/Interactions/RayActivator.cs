using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RayActivator : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GameObject _lenRayInteractorGo;

    bool _isFirstEndSimu = true;

    bool _canInteractorBeEnabled = false;
    bool _doesDeskInteractorWantToHideIt = false;


    private void Start()
    {
        Invoke(nameof(OnDelayedStart), .5f);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDelayedStart()
    {
        _lenRayInteractorGo.SetActive(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != Scenes.KG)
            return;

        Invoke(nameof(DelayedRegisterGraphUpdate), 1f);
    }

    void DelayedRegisterGraphUpdate()
    {
        _referenceHolderSo.GraphManager.OnGraphUpdate += OnGraphUpdated;
    }


    private void OnGraphUpdated(GraphUpdateType updateType)
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
