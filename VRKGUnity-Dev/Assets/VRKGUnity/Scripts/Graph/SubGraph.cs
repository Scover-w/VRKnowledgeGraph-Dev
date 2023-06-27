using System.Collections;
using UnityEngine;

public class SubGraph : MonoBehaviour
{

    public Transform Tf { get { return _subGraphTf; } }

    [SerializeField]
    float _transitionTime = 1f;

    [SerializeField]
    EasingType _easingType = EasingType.EaseInOutQuint;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    Transform _subGraphTf;

    [SerializeField]
    Transform _watchTf;

    [SerializeField]
    Transform _gpsPointTf;

    [SerializeField]
    Transform _graphUpDeskTf;

    SubGraphMode _subGraphMode;

    bool _displayWatch = true;
    bool _inTransition = false;

    float _deltaHeightWatch = .105f;
    float _deltaHeightDesk = .5f;

    float _sizeWatchGraph = .15f;

    private void Start()
    {
        _subGraphMode = SubGraphMode.Lens;
        _graphManager.OnGraphUpdate += OnGraphUpdated;
    }

    private void Update()
    {
        if (_subGraphMode != SubGraphMode.Watch)
            return;


        _subGraphTf.position = _watchTf.position + _watchTf.up * _deltaHeightWatch;
    }


    


    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        switch (updateType)
        {
            case GraphUpdateType.BeforeSimulationStart:
                break;
            case GraphUpdateType.AfterSimulationHasStopped:
                SimulationStopped();
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
        if (graphMode == GraphMode.Desk && _subGraphMode == SubGraphMode.Lens ||
            graphMode == GraphMode.Immersion && _subGraphMode == SubGraphMode.Watch)
            return;



    }

    private void SimulationStopped()
    {
        // TODO : SimulationStopped
        if(_subGraphMode == SubGraphMode.Lens)
        {

        }
        else // Watch
        {

        }
    }

    [ContextMenu("SwitchGPSVisibility")]
    public void SetWatchVisibility(bool isVisible)
    {
        if (_subGraphMode != SubGraphMode.Watch)
            return;

        _displayWatch = isVisible;
        _subGraphTf.gameObject.SetActive(_displayWatch);
    }

    public void UpdateGPSPoint(Vector3 normPosition)
    {
        // TODO : convert to miniGraph scale
    }

    enum SubGraphMode
    {
        Lens,
        Watch
    }

}
