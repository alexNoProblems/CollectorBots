using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class FlagHandler : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private FlagPlacer _flagPlacer;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private float _maxRayDistance = 2000f;
    [SerializeField] private string _actionMap = "Camera";
    [SerializeField] private string _point = "MousePosition";
    [SerializeField] private string _click = "Click";
    [SerializeField] private string _cancel = "Cancel";
    [SerializeField] private string _rotateButton = "RotateButton";

    private InputAction _pointAction;
    private InputAction _clickAction;
    private InputAction _cancelAction;
    private InputAction _rotateAction;

    private void Reset()
    {
        if (_camera == null)
            _camera = Camera.main;
        
        if (_playerInput == null)
            _playerInput = GetComponent<PlayerInput>();
    }

    private void Awake()
    {
        if (_playerInput == null)
            _playerInput = GetComponent<PlayerInput>();

        var map = _playerInput != null ? _playerInput.actions.FindActionMap(_actionMap, true) : null;

        _pointAction = map?.FindAction(_point, true);
        _rotateAction = map?.FindAction(_rotateButton, true);
        _clickAction = map?.FindAction(_click, false);
        _cancelAction = map?.FindAction(_cancel, false);
    }

    private void OnEnable()
    {
        if (_clickAction != null)
            _clickAction.performed += OnClickPerformed;
        
        if (_cancelAction != null)
            _cancelAction.performed += OnCancelPerformed;
    }

    private void OnDisable()
    {
        if (_clickAction != null)
            _clickAction.performed -= OnClickPerformed;
        
        if (_cancelAction != null)
            _cancelAction.performed -= OnCancelPerformed;
    }

    private void Update()
    {
        if (_flagPlacer.IsPlacing && !IsRotating())
        {
            if (TryRaycastComponent<Ground>(ScreenPointToRay(GetPointer()), out var hit, out _))
                _flagPlacer.UpdatePreview(hit.point, hit.normal);
        }
    }

    private static bool IsPoinerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void OnClickPerformed(InputAction.CallbackContext _)
    {
        if (IsPoinerOverUI() || IsRotating())
            return;

        HandleClick();
    }

    private void OnCancelPerformed(InputAction.CallbackContext _)
    {
        if (_flagPlacer.IsPlacing)
            _flagPlacer.Cancel();
    }

    private void HandleClick()
    {
        var ray = ScreenPointToRay(GetPointer());

        if (TryRaycastComponent<Base>(ray, out var baseHit, out var selectedBase))
        {
            _flagPlacer.BeginPlacement(selectedBase);

            return;
        }

        if (_flagPlacer.IsPlacing && TryRaycastComponent<Ground>(ray, out var groundHit, out _))
            _flagPlacer.PlaceAt(groundHit.point, groundHit.normal);
    }

    private Vector2 GetPointer()
    {
        return _pointAction.ReadValue<Vector2>();
    }

    private Ray ScreenPointToRay(Vector2 screenPosition)
    {
        return _camera.ScreenPointToRay(screenPosition);
    }

    private bool IsRotating()
    {
        return _rotateAction != null && _rotateAction.IsPressed();
    }

    private bool TryRaycastComponent<T>(Ray ray, out RaycastHit hit, out T component) where T : Component
    {
        hit = default;
        component = null;

        var hits = Physics.RaycastAll(ray, _maxRayDistance);

        if (hits == null || hits.Length == 0)
            return false;

        float bestDistance = float.MaxValue;
        RaycastHit bestHit = default;
        T bestComponent = null;

        for (int i = 0; i < hits.Length; i++)
        {
            var item = hits[i];

            if (!item.collider.TryGetComponent<T>(out var found))
                found = item.collider.GetComponentInParent<T>();
            
            if (found == null)
                continue;
            
            if (item.distance < bestDistance)
            {
                bestDistance = item.distance;
                bestHit = item;
                bestComponent = found;
            }
        }

        if (bestComponent == null)
            return false;

        hit = bestHit;
        component = bestComponent;

        return true;
    }
}
