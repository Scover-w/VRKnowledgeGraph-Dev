using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameWindowNavigation : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    Transform _camTf;


    public float CameraSpeed = 5f;
    public float Sensitivity = 1f;
    public float ZoomSpeed = 1f;


    private Camera _cam;

    private void Start()
    {
        _cam = _camTf.gameObject.GetComponent<Camera>();
    }

    void Update()
    {
        UpdateLook();
        UpdateMove();
        TryRaycastNode();

    }

    private void UpdateLook()
    {
        if (Input.GetMouseButtonDown(1))
            Cursor.lockState = CursorLockMode.Locked;
        else if (Input.GetMouseButtonUp(1))
            Cursor.lockState = CursorLockMode.None;

        else if (Input.GetMouseButton(1))
        {
            // Calculate new mouse position and movement vector
            Vector3 movement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Sensitivity;

            // Rotate camera based on mouse movement
            transform.RotateAround(_camTf.position, Vector3.up, movement.x * CameraSpeed * Time.deltaTime);
            transform.RotateAround(_camTf.position, _camTf.right, -movement.y * CameraSpeed * Time.deltaTime);
        }
    }

    private void UpdateMove()
    {

        Vector3 _direction = Vector3.zero;

        if (Input.GetKey(KeyCode.Z))
            _direction += _camTf.forward;

        if (Input.GetKey(KeyCode.S))
            _direction -= _camTf.forward;

        if (Input.GetKey(KeyCode.Q))
            _direction -= _camTf.right;

        if (Input.GetKey(KeyCode.D))
            _direction += _camTf.right;

        _camTf.Translate(_direction.normalized * (Time.deltaTime * CameraSpeed * (Input.GetKey(KeyCode.LeftShift)? 2f : 1f)), Space.World);
    }

    private void TryRaycastNode()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        var graph = _graphManager.Graph;
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 100f, ~Layers.Node))
        {
            graph.SelectNode(null);
            return;
        }

        graph.SelectNode(hit.collider.transform);
    }
}
