using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ZombieAnimator))]
[DisallowMultipleComponent]
public class ZombiePickUpper : MonoBehaviour
{
    [SerializeField] private string _pickUpStateName = "PickUp";
    [SerializeField] private float _enterStateTimeOut = 0.35f;
    [SerializeField] private float _maxPickUpWait = 5f;

    private ZombieAnimator _animator;
    private Transform _carryAnchor;
    private bool _isPickingUp;
    private Brain _pendingBrain;

    public event Action<Brain> PickedUpCompleted;

    public bool IsPickingUp => _isPickingUp;

    private void Awake()
    {
        _animator = GetComponent<ZombieAnimator>();
        _animator.OverridePickUpStateName(_pickUpStateName);
    }

    public void BindCarryAnchor(Transform anchor) => _carryAnchor = anchor != null ? anchor : transform;

    public void StartPickUp(Brain brain)
    {
        if (_isPickingUp || brain == null)
            return;

        _pendingBrain = brain;

        StartCoroutine(PickUpRoutine());
    }

    public void AttachToCarry(Brain brain)
    {
        if (brain == null || _carryAnchor == null)
            return;

        brain.AttachToZombie(_carryAnchor);
    }

    public void OnPickUpAnimationEnd()
    {
        if (!_isPickingUp)
            return;
        
        Complete();
    }

    private IEnumerator PickUpRoutine()
    {
        float waitingTime = 0f;
        float elapsedTime = 0f;

        _isPickingUp = true;
        _animator.PlayPickUp();

        while (waitingTime < _enterStateTimeOut)
        {
            waitingTime += Time.deltaTime;

            if (_animator.IsInPickUp(out _))
                break;

            yield return null;
        }

        while (_isPickingUp && elapsedTime < _maxPickUpWait)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (_isPickingUp)
            Complete();
    }

    private void Complete()
    {
        _isPickingUp = false;
        Brain brain = _pendingBrain;
        _pendingBrain = null;

        PickedUpCompleted?.Invoke(brain);
    }
}
