using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationPC : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    public float CameraSpeed = 5f;
    public float Sensitivity = 1f;
    public float ZoomSpeed = 1f;

    Transform _camTf;
    Camera _cam;

    private void Start()
    {
        _cam = _referenceHolderSO.HMDCamSA.Value;
        _camTf = _cam.transform;
    }

    void Update()
    {
        UpdateLook();
        UpdateMove();
        
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
            _camTf.RotateAround(_camTf.position, Vector3.up, movement.x * CameraSpeed * Time.deltaTime);
            _camTf.RotateAround(_camTf.position, _camTf.right, -movement.y * CameraSpeed * Time.deltaTime);
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

        if (Input.GetKey(KeyCode.A))
            _direction += _camTf.up;

        if (Input.GetKey(KeyCode.E))
            _direction -= _camTf.up;

        float speed = (Input.GetKey(KeyCode.LeftShift) ? 10f : 1f);
        speed *= (Input.GetKey(KeyCode.Space) ? .10f : 1f);
        _camTf.Translate(_direction.normalized * (Time.deltaTime * CameraSpeed * speed), Space.World);
    }
}
