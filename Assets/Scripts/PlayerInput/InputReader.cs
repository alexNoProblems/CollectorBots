using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    [SerializeField] private string _actionMap = "Camera";
    [SerializeField] private string _point = "MousePosition";
    [SerializeField] private string _click = "Click";
    [SerializeField] private string _cancel = "Cancel";
    [SerializeField] private string _rotateButton = "RotateButton";

    private PlayerInput _playerInput;
    private InputAction _pointAction, _clickAction, _cancelAction, _rotateAction;

    public Vector2 Pointer()
    {
        return _pointAction.ReadValue<Vector2>();
    }

    public bool IsRotating()
    {
        return _rotateAction != null && _rotateAction.IsPressed();
    }

    public event Action Clicked;
    public event Action Canceled;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        var actionMap = _playerInput.actions.FindActionMap(_actionMap, true);

        _pointAction = actionMap?.FindAction(_point, true);
        _rotateAction = actionMap?.FindAction(_rotateButton, true);
        _clickAction = actionMap?.FindAction(_click, false);
        _cancelAction = actionMap?.FindAction(_cancel, false);
    }

    private void OnEnable()
    {
        if (_clickAction != null)
            _clickAction.performed  += _ => Clicked?.Invoke();
        
        if (_cancelAction != null)
            _cancelAction.performed += _ => Canceled?.Invoke();
    }

    private void OnDisable()
    {
        if (_clickAction != null)
            _clickAction.performed -= _ => Clicked?.Invoke();

        if (_cancelAction != null)
            _cancelAction.performed -= _ => Canceled?.Invoke();
    }
}
