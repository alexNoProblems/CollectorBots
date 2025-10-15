using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 0.2f;
    [SerializeField] private float _turnSpeed = 720f;
    [SerializeField] private float _stoppingDistance = 0.6f;
    [SerializeField] private float _baseAcceleration = 8f;
    [SerializeField] private float _accelerationMultiplier = 4f;
    [SerializeField] private float _arrivalBuffer = 0.05f;
    [SerializeField] private float _moveThreshold = 0.01f;

    private NavMeshAgent _navMeshAgent;
    private Transform _destination;
    private Vector3? _destinationOverride;
    private bool _isPaused;

    public event Action Arrived;

    public bool IsMoving 
    {
        get
        {
            if (_isPaused)
                return false;

            float speedSqr = _navMeshAgent.velocity.sqrMagnitude;

            return speedSqr > _moveThreshold;
        }
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        ConfigureNavMeshAgent();
    }

    private void Update()
    {
        if (_isPaused)
        {
            if (!_navMeshAgent.isStopped)
                _navMeshAgent.isStopped = true;

            _navMeshAgent.velocity = Vector3.zero;

            return;
        }

        if (_destination == null && !_destinationOverride.HasValue)
        {
            if (!_navMeshAgent.isStopped)
                _navMeshAgent.isStopped = true;

            return;
        }

        if (_navMeshAgent.isStopped)
            _navMeshAgent.isStopped = false;

        var destination = _destinationOverride ?? _destination.position;
        _navMeshAgent.SetDestination(destination);

        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + _arrivalBuffer)
            Arrived?.Invoke();
    }

    private void ConfigureNavMeshAgent()
    {
        _navMeshAgent.speed = _moveSpeed;
        _navMeshAgent.angularSpeed = _turnSpeed;
        _navMeshAgent.stoppingDistance = _stoppingDistance;
        _navMeshAgent.acceleration = Mathf.Max(_baseAcceleration, _moveSpeed * _accelerationMultiplier);
        _navMeshAgent.autoBraking = true;
        _navMeshAgent.updateRotation = true;
    }

    public void GoToTarget(Transform target)
    {
        _destination = target;
        _destinationOverride = null;
    }

    public void GoToPosition(Vector3 position)
    {
        _destination = null;
        _destinationOverride = position;
    }

    public void ClearDestination()
    {
        _destination = null;
        _destinationOverride = null;

        if (_navMeshAgent.isOnNavMesh)
            _navMeshAgent.ResetPath();
    }

    public void PauseMovement()
    {
        _isPaused = true;

        if (!_navMeshAgent.isStopped)
            _navMeshAgent.isStopped = true;

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.velocity = Vector3.zero;
    }

    public void ResumeMovement()
    {
        _isPaused = false;
        _navMeshAgent.updateRotation = true;
        _navMeshAgent.isStopped = false;
    }

    public void TeleportTo(Vector3 worldPosition)
    {
        if (_navMeshAgent.isOnNavMesh)
            _navMeshAgent.Warp(worldPosition);
        else
            transform.position = worldPosition;
    }
}
