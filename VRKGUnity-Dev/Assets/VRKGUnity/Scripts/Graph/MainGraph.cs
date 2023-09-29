using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manager the Desk and Immersion Graph
/// </summary>
public class MainGraph : MonoBehaviour
{

    public Transform Tf { get {  return _mainGraphTf; } }

    [SerializeField]
    EasingType _easingType = EasingType.EaseInOutQuint;

    [SerializeField]
    EasingType _easingTypeDesk = EasingType.EaseInOutQuint;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    Transform _mainGraphTf;

    [SerializeField]
    Transform _upDeskTf;

    [SerializeField]
    GameObject _loadingGo;

    [SerializeField]
    Transform _mobiusDeskTf;

    [SerializeField]
    GameObject _deskGraphTriggerGo;

    [SerializeField]
    Transform _deskFloor;

    [SerializeField]
    GameObject _deskCollidersGo;

    [SerializeField]
    Transform _deskGraphTriggerTf;

    Transform _playerTf;

    EasingDel _easingFunction;
    GraphConfiguration _graphConfig;

    bool _isSimulatingGraph = false;
    bool _isMovingMainGRaph = false;

    MainGraphMode _mainGraphMode;
    MainGraphMode _nextGraphMode;

    void Start()
    {
        Debug.Log("MainGraph Start");

        _mainGraphMode = (Settings.DEFAULT_GRAPH_MODE == GraphMode.Desk)? MainGraphMode.Desk : MainGraphMode.Immersion;
        _playerTf = _referenceHolderSo.HMDCamSA.Value.transform;
        _mainGraphTf.position = _upDeskTf.position;

        _referenceHolderSo.OnNewMaxRadius += OnNewMaxRadius;

        _easingFunction = Easing.GetEasing(_easingType);

        _graphManager.OnGraphUpdate += OnGraphUpdated;

        _graphConfig = GraphConfiguration.Instance;
    }

    private void OnDestroy()
    {
        _graphManager.OnGraphUpdate -= OnGraphUpdated;
        _referenceHolderSo.OnNewMaxRadius -= OnNewMaxRadius;

        StopAllCoroutines();
    }


    #region OnGraphUpdated
    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        DebugDev.Log("MainGraph : OnGraphUpdated " + updateType.ToString());

        switch (updateType)
        {
            case GraphUpdateType.RetrievingFromDb:
                RetrieveFromDb();
                break;

            case GraphUpdateType.BeforeSimulationStart:
                BeforeSimulationStart();
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
        _mainGraphTf.gameObject.SetActive(false);

        if (_mainGraphMode == MainGraphMode.Immersion)
            return;

        _loadingGo.SetActive(true);
        _deskCollidersGo.SetActive(false);
        _isSimulatingGraph = true;
        StartCoroutine(RotatingDesk());
    }

    private void BeforeSimulationStart()
    {
        _mainGraphTf.gameObject.SetActive(true);
    }


    private void AfterSimulationHasStopped()
    {
        _loadingGo.SetActive(false);
        _isSimulatingGraph = false;

        if (_mainGraphMode == MainGraphMode.Desk)
            _deskCollidersGo.SetActive(true);
    }

    private void BeforeSwitchMode()
    {
        _loadingGo.SetActive(false);
        _deskGraphTriggerGo.SetActive(false);

        _nextGraphMode = (_mainGraphMode == MainGraphMode.Desk? MainGraphMode.Immersion: MainGraphMode.Desk);
        _mainGraphMode = MainGraphMode.InTransition;

        StartCoroutine(MovingMainGraph());
        StartCoroutine(TransitioningDesk(_nextGraphMode == MainGraphMode.Desk));
    }

    private void AfterSwitchModeToDesk()
    {
        _mainGraphMode = MainGraphMode.Desk;

        _deskGraphTriggerGo.SetActive(true);
    }

    private void AfterSwitchModeToImmersion()
    {
        _mainGraphMode = MainGraphMode.Immersion;
    }

    IEnumerator MovingMainGraph()
    {
        float speed = 1f / _graphManager.GraphConfiguration.GraphModeTransitionTime;
        float time = 0f;
        _isMovingMainGRaph = true;
        while (time < 1f)
        {
            MoveMainGraph(time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        MoveMainGraph(1f);

        _isMovingMainGRaph = false;
    }

    private void MoveMainGraph(float t)
    {
        if(_nextGraphMode == MainGraphMode.Desk)
        {
            _mainGraphTf.position = Vector3.Lerp(Vector3.zero, _upDeskTf.position, _easingFunction(t));
            _mainGraphTf.rotation = Quaternion.Slerp(Quaternion.identity, _upDeskTf.rotation, _easingFunction(t));
        }
        else
        {
            _mainGraphTf.position = Vector3.Lerp(_upDeskTf.position, Vector3.zero, _easingFunction(t));
            _mainGraphTf.rotation = Quaternion.Slerp(_upDeskTf.rotation, Quaternion.identity, _easingFunction(t));
        }
    }

    IEnumerator TransitioningDesk(bool switchToDesk)
    {
        float speed = 1f / _graphManager.GraphConfiguration.GraphModeTransitionTime;
        float time = 0f;

        Vector3 startPos = (switchToDesk) ? Vector3.down * 3f : Vector3.zero;
        Vector3 endPos = (switchToDesk) ? Vector3.zero : Vector3.down * 3f;

        if(switchToDesk)
        {
            _mobiusDeskTf.gameObject.SetActive(true);
            _deskFloor.gameObject.SetActive(true);
        }

        while (time < 1f)
        {
            MoveMobiusDesk();
            yield return null;
            time += Time.deltaTime * speed;
        }

        time = 1f;
        MoveMobiusDesk();

        if (!switchToDesk)
        {
            _mobiusDeskTf.gameObject.SetActive(false);
            _deskFloor.gameObject.SetActive(false);
        }

        void MoveMobiusDesk()
        {
            _mobiusDeskTf.localPosition = Vector3.Lerp(startPos, endPos, _easingFunction(time));
            _deskFloor.localPosition = Vector3.Lerp(startPos, endPos, _easingFunction(time));
            
        }
    }

    IEnumerator RotatingDesk()
    {
        float start = Time.time; 

        while(_isSimulatingGraph)
        {
            yield return null;
            float rot = _easingFunction((Time.time - start) * .5f % 1f);

            _mobiusDeskTf.localRotation = Quaternion.Euler(new Vector3(0f, rot * 360f, 0f));
        }

        float velocity = 0;
        float currentYRotation = _mobiusDeskTf.eulerAngles.y;
        float newYRotation = Mathf.SmoothDampAngle(currentYRotation, 0, ref velocity, .5f);

        while (Mathf.Abs(newYRotation) > 0.1f)
        {
            yield return null;
            currentYRotation = _mobiusDeskTf.eulerAngles.y;
            newYRotation = Mathf.SmoothDampAngle(currentYRotation, 0, ref velocity, .5f);
            _mobiusDeskTf.eulerAngles = new Vector3(_mobiusDeskTf.eulerAngles.x, newYRotation, _mobiusDeskTf.eulerAngles.z);
        }

        _mobiusDeskTf.rotation = Quaternion.identity;
    }
    #endregion


    public void DeskGraphChanged()
    {
        OnNewMaxRadius(_referenceHolderSo.MaxRadiusGraph);
    }

    private void OnNewMaxRadius(float maxRadius)
    {
        if (maxRadius == -1f)
            return;

        float maxRadiusScaled = maxRadius * _graphConfig.EffectiveDeskGraphSize;
        _upDeskTf.localPosition = new Vector3(0f, maxRadiusScaled, 0f);

        float maxRadiusScaledScaledUp = maxRadiusScaled + .025f;
        _deskGraphTriggerTf.localScale = new Vector3(maxRadiusScaledScaledUp, maxRadiusScaledScaledUp, maxRadiusScaledScaledUp);

        if (_mainGraphMode == MainGraphMode.Desk)
            _mainGraphTf.position = _upDeskTf.position;
    }


    enum MainGraphMode
    {
        Desk,
        Immersion,
        InTransition
    }

}