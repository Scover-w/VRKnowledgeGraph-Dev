using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGraph : MonoBehaviour
{

    public Transform Tf { get {  return _mainGraphTf; } }

    [SerializeField]
    EasingType _easingType = EasingType.EaseInOutQuint;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    Transform _mainGraphTf;

    [SerializeField]
    Transform _deskTf;

    [SerializeField]
    GameObject _loadingGo;

    Transform _playerTf;

    EasingDel _easingFunction;

    MainGraphMode _mainGraphMode;
    MainGraphMode _nextGraphMode;

    void Start()
    {
        _mainGraphMode = (Settings.DEFAULT_GRAPH_MODE == GraphMode.Desk)? MainGraphMode.Desk : MainGraphMode.Immersion;
        _playerTf = _referenceHolderSo.HMDCamSA.Value.transform;
        _mainGraphTf.position = _deskTf.position;

        _easingFunction = Easing.GetEasing(_easingType);

        _graphManager.OnGraphUpdate += OnGraphUpdated;

    }


    #region OnGraphUpdated
    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        switch (updateType)
        {
            case GraphUpdateType.RetrievingFromDb:
                RetrieveFromDb();
                break;

            case GraphUpdateType.BeforeSimulationStart:

                break;

            case GraphUpdateType.AfterSimulationHasStopped:
                AfterSimulationHasStopped();
                break;

            case GraphUpdateType.BeforeSwitchMode:
                BeforeSwitchMode();
                break;

            case GraphUpdateType.AfterSwitchModeToDesk:
                AfterSwitchModeToDesk();
                break;

            case GraphUpdateType.AfterSwitchModeToImmersion:
                AfterSwitchModeToImmersion();
                break;
        }
    }

    private void RetrieveFromDb()
    {
        if (_mainGraphMode == MainGraphMode.Immersion)
            return;

        _loadingGo.SetActive(true);
    }

    private void AfterSimulationHasStopped()
    {
        _loadingGo.SetActive(false);
    }

    private void BeforeSwitchMode()
    {
        _loadingGo.SetActive(false);

        _nextGraphMode = (_mainGraphMode == MainGraphMode.Desk? MainGraphMode.Immersion: MainGraphMode.Desk);
        _mainGraphMode = MainGraphMode.InTransition;

        StartCoroutine(MovingMainGraph());
    }

    private void AfterSwitchModeToDesk()
    {
        _mainGraphMode = MainGraphMode.Desk;
    }

    private void AfterSwitchModeToImmersion()
    {
        _mainGraphMode = MainGraphMode.Immersion;
    }

    IEnumerator MovingMainGraph()
    {
        float speed = 1f / _graphManager.GraphConfiguration.GraphModeTransitionTime;
        float time = 0f;

        while(time < 1f)
        {
            MoveMainGraph(time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        MoveMainGraph(1f);
    }

    private void MoveMainGraph(float t)
    {
        if(_nextGraphMode == MainGraphMode.Desk)
        {
            _mainGraphTf.position = Vector3.Lerp(Vector3.zero, _deskTf.position, _easingFunction(t));
        }
        else
        {
            _mainGraphTf.position = Vector3.Lerp(_deskTf.position, Vector3.zero, _easingFunction(t));
        }
    }
    #endregion

    enum MainGraphMode
    {
        Desk,
        Immersion,
        InTransition
    }

}