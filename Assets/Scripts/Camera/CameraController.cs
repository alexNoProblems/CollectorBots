using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class CameraController : MonoBehaviour
{
    private const string Camera = "Camera";
    private const string MousePosition = "MousePosition";
    private const string ZoomMouse = "Zoom";
    private const string LookDelta = "LookDelta";
    private const string RotateButton = "RotateButton";

    [SerializeField] private Camera _camera;
    [SerializeField] private float _moveSpeed = 15f;
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 0.2f;
    [SerializeField] private float _edge = 20f;
    [SerializeField] private float _zoomThreshold = 0.01f;
    [SerializeField] private float _minZoomY = 2f;
    [SerializeField] private float _maxZoomY = 10f;

    private InputAction _mousePosition;
    private InputAction _zoom;
    private InputAction _lookDelta;
    private InputAction _rotateButton;

    private void Awake()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        InputActionMap inputActionMap = playerInput.actions.FindActionMap(Camera, true);

        _mousePosition = inputActionMap.FindAction(MousePosition, true);
        _zoom = inputActionMap.FindAction(ZoomMouse, true);
        _lookDelta = inputActionMap.FindAction(LookDelta, true);
        _rotateButton = inputActionMap.FindAction(RotateButton, true);
    }

    private void Update()
    {
        MoveCameraByEdges();
        Zoom();
        Rotate();
    }

    private void MoveCameraByEdges()
    {
        Vector2 mousePosition = _mousePosition.ReadValue<Vector2>();
        Vector3 direction = Vector3.zero;

        if (mousePosition.x <= _edge) 
            direction += Vector3.right;
        if (mousePosition.x >= Screen.width - _edge) 
            direction += Vector3.left;
        if (mousePosition.y <= _edge) 
            direction += Vector3.forward;
        if (mousePosition.y >= Screen.height - _edge) 
            direction += Vector3.back;

        if (direction.sqrMagnitude > 0f)
            transform.Translate(new Vector3(direction.x, 0, direction.z) * _moveSpeed * Time.deltaTime, Space.World);
    }

    private void Zoom()
    {
        float scroll = _zoom.ReadValue<float>();

        if (Mathf.Abs(scroll) < _zoomThreshold)
            return;

        Vector3 cameraLocalPosition = _camera.transform.localPosition;
        cameraLocalPosition.y = Mathf.Clamp(cameraLocalPosition.y - scroll * _zoomSpeed * Time.deltaTime, _minZoomY, _maxZoomY);

        _camera.transform.localPosition = cameraLocalPosition;
    }

    private void Rotate()
    {
        if (_rotateButton.IsPressed())
        {
            Vector2 displacement = _lookDelta.ReadValue<Vector2>();
            transform.Rotate(Vector3.up, displacement.x * _rotateSpeed, Space.World);
        }
    }
}
