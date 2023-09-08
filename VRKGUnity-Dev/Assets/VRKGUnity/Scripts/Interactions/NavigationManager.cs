using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    EasingType _easingType = EasingType.EaseInOutQuint;

    Transform _xrOriginTf;
    EasingDel _easingFunction;

    GraphMode _graphMode = Settings.DEFAULT_GRAPH_MODE;

    private void Start()
    {
        _graphManager.OnGraphUpdate += OnGraphUpdated;
        _easingFunction = Easing.GetEasing(_easingType);

        _xrOriginTf = _referenceHolderSO.XrOriginTf.Value;
    }

    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        switch (updateType)
        {
            case GraphUpdateType.RetrievingFromDb:
                break;

            case GraphUpdateType.BeforeSimulationStart:

                break;

            case GraphUpdateType.AfterSimulationHasStopped:
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

    private void BeforeSwitchMode()
    {
        if(_graphMode== GraphMode.Immersion)
            StartCoroutine(MovingPlayerToDesk());
    }

    private void AfterSwitchModeToDesk()
    {
        _graphMode = GraphMode.Desk;
    }

    private void AfterSwitchModeToImmersion()
    {
        _graphMode = GraphMode.Immersion;
    }

    IEnumerator MovingPlayerToDesk()
    {
        float speed = 1f / _graphManager.GraphConfiguration.GraphModeTransitionTime;
        float time = 0f;

        Vector3 startPos = _xrOriginTf.position;
        Vector3 deskPos = Vector3.zero;

        while (time < 1f)
        {
            MovePlayer();
            yield return null;
            time += Time.deltaTime * speed;
        }

        time = 1;
        MovePlayer();

        void MovePlayer()
        {
            _xrOriginTf.position = Vector3.Lerp(startPos, deskPos, _easingFunction(time));
        }
    }
}
