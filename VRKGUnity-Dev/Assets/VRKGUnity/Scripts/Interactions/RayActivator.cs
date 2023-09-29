using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager the ray that allowto select/activate nodes' graph
/// </summary>
public class RayActivator : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GameObject _lenRayInteractorGo;

    Transform _tf;

    bool _isFirstEndSimu = true;

    bool _canInteractorBeEnabled = false;
    bool _doesDeskInteractorWantToHideIt = false;

    bool _isRaycastingAgainstUI = false;

    RaycastHit hit;
    int _layerUI;


    private void Start()
    {
        Invoke(nameof(OnDelayedStart), .5f);
        SceneManager.sceneLoaded += OnSceneLoaded;

        _tf = transform;
        _layerUI = 1 << AIDEN.TactileUI.Layers.PresenceUI;
    }


    private void Update()
    {
        if (!_canInteractorBeEnabled || _doesDeskInteractorWantToHideIt)
            return;

        var isRaycastingAgainstUI = Physics.Raycast(_tf.position, _tf.forward, out hit, 100f, _layerUI);

        if (isRaycastingAgainstUI == _isRaycastingAgainstUI)
            return;

        _isRaycastingAgainstUI = isRaycastingAgainstUI;
        _lenRayInteractorGo.SetActive(!_isRaycastingAgainstUI);
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

        bool isActive = _canInteractorBeEnabled && !_doesDeskInteractorWantToHideIt;

        _lenRayInteractorGo.SetActive(isActive);

        if (isActive)
            _isRaycastingAgainstUI = false;
    }

    public void SetDeskInteractorWantToHideIt(bool hideIt)
    {
        _doesDeskInteractorWantToHideIt = hideIt;

        bool isActive = _canInteractorBeEnabled && !_doesDeskInteractorWantToHideIt;

        _lenRayInteractorGo.SetActive(isActive);

        if (isActive)
            _isRaycastingAgainstUI = false;
    }
}
