using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngineInternal;

public class TeleportationToNode : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    InputActionReference _teleportationActionRef;

    GraphMode _graphMode = Settings.DEFAULT_GRAPH_MODE;

    Transform _xrOriginTf;

    InputAction _teleportationAction;

    Transform _tf;
    int _layer;
    bool _canTeleport = false;

    private void Awake()
    {
        _teleportationAction = _teleportationActionRef.action;
    }

    private void Start()
    {
        _tf = transform;
        _layer = 1 << Layers.Node;
        _xrOriginTf = _referenceHolderSO.XrOriginTf.Value;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != Scenes.KG)
            return;

        Invoke(nameof(DelayedRegisterGraphUpdate), 1f);
    }

    void DelayedRegisterGraphUpdate()
    {
        _referenceHolderSO.GraphManager.OnGraphUpdate += OnGraphUpdated;
    }

    private void OnEnable()
    {
        _teleportationAction.performed += TryTeleport;
    }

    private void OnDisable()
    {
        _teleportationAction.performed -= TryTeleport;
    }

    private void TryTeleport(InputAction.CallbackContext context)
    {
        Debug.Log("Try teleport : " + _canTeleport);

        if (!_canTeleport)
            return;

        RaycastHit hit;

        if (!Physics.Raycast(_tf.position, _tf.forward, out hit, 1000f, _layer))
        {
            return;
        }

        Vector3 hitPos = hit.point;
        Vector3 nodePos = hit.transform.position;
        float radiusNode = (hitPos - nodePos).magnitude;

        Vector3 newPlayerPos = nodePos + Vector3.up * radiusNode;
        _xrOriginTf.position = newPlayerPos;
    }

    private void OnGraphUpdated(GraphUpdateType updateType)
    {

        switch (updateType)
        {
            case GraphUpdateType.RetrievingFromDb:
                _canTeleport = false;
                break;
            case GraphUpdateType.BeforeSimulationStart:
                _canTeleport = false;
                break;
            case GraphUpdateType.AfterSimulationHasStopped:
                _canTeleport = _graphMode == GraphMode.Immersion;
                break;
            case GraphUpdateType.BeforeSwitchMode:
                _canTeleport = false;
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                _graphMode = GraphMode.Desk;
                _canTeleport = false;
                break;
            case GraphUpdateType.AfterSwitchModeToImmersion:
                _graphMode = GraphMode.Immersion;
                _canTeleport = true;
                break;
        }

    }

}
