using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGraph : MonoBehaviour
{

    public Transform Tf { get {  return _tf; } }

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    Transform _tf;

    Transform _playerTf;

    MainGraphMode _mainGraphMode;

    void Start()
    {
        _mainGraphMode = MainGraphMode.Desk;
        _playerTf = _referenceHolderSo.HMDCamSA.Value.transform;
        _graphManager.OnGraphUpdate += OnGraphUpdated;
    }


    private void Update()
    {
        
    }


    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        switch (updateType)
        {
            case GraphUpdateType.BeforeSimulationStart:

                break;

            case GraphUpdateType.AfterSimulationHasStopped:

                break;

            case GraphUpdateType.BeforeSwitchMode:

                break;
            case GraphUpdateType.AfterSwitchModeToDesk:

                break;

            case GraphUpdateType.AfterSwitchModeToImmersion:

                break;
        }
    }

    public void SwitchMode(GraphMode graphMode)
    {
        if (graphMode == GraphMode.Desk && _mainGraphMode == MainGraphMode.Desk ||
            graphMode == GraphMode.Immersion && _mainGraphMode == MainGraphMode.Desk)
            return;



    }


    enum MainGraphMode
    {
        Desk,
        Immersion
    }

}