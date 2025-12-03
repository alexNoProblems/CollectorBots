using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ZombieMover), typeof(ZombiePickUpper), typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Zombie : MonoBehaviour, IPoolable<Zombie>
{
    private enum State { Idle, ToBrain, CarryToBase, ToBuild }

    [SerializeField] private Transform _model;
    [SerializeField] private Transform _carryAnchor;
    [SerializeField] private float _nearestDistance = 1.5f;

    private BrainStorage _storage;
    private ZombieDispatcher _zombies;
    private BrainScanner _scanner;
    private Transform _base;
    private ZombieMover _mover;
    private ZombiePickUpper _pickUpper;
    private ZombieAnimator _animator;

    private State _state = State.Idle;
    private Transform _target;
    private Brain _carriedBrain;
    private Vector3 _initialScale;

    private Action<Zombie> _onBuildArrivedCallback;

    public event Action<Zombie> Released;

    public bool HasTarget => _target != null;
    public bool IsAvailable => _state == State.Idle && _carriedBrain == null;
    public Vector3 TargetPosition => _target != null ? _target.position : transform.position;
    private bool IsPickingUp => _pickUpper != null && _pickUpper.IsPickingUp;

    private void Awake()
    {
        _mover = GetComponent<ZombieMover>();
        _pickUpper = GetComponent<ZombiePickUpper>();
        _animator = GetComponent<ZombieAnimator>();
        _initialScale = transform.localScale;

        if (_model == null) 
            _model = transform;

        if (_carryAnchor == null) 
            _carryAnchor = transform;
    }

    private void Update()
    {
        if (_animator != null && _mover != null)
            _animator.SetMoving(_mover.IsMoving);
    }

    private void OnEnable()
    {
        _mover.Arrived += HandleArrived;
        _pickUpper.PickedUpCompleted += HandlePickUpDone;
    }

    private void OnDisable()
    {
        _mover.Arrived -= HandleArrived;
        _pickUpper.PickedUpCompleted -= HandlePickUpDone;

         _carriedBrain = null;
        _target = null;
        _state = State.Idle;

        var navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent != null)
        {
            navMeshAgent.ResetPath();
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
        }
    }

    public void Init()
    {
        _pickUpper.BindCarryAnchor(_carryAnchor);
        _carriedBrain = null;
        ClearTarget();
    }

    public void MakeDependencies(ZombieDispatcher dispatcher, BrainStorage storage, Transform basePosition)
    {
        _zombies = dispatcher;
        _storage = storage;
        _base = basePosition;
    }

    public void FinalizeSetup()
    {
        if (_zombies == null)
            return;

        _zombies.Register(this);
        _zombies.MarkFreeZombie(this);
    }

    public void SpawnTo(Vector3 worldPosition)
    {
        _mover.TeleportTo(worldPosition);
    }

    public void Despawn()
    {
        Released?.Invoke(this);
    }

    public void SetScanner(BrainScanner scanner)
    {
        _scanner = scanner;
    }

    public void SetBase(Transform baseTransform)
    {
        _base = baseTransform;
    }

    public void SetTarget(Transform target)
    {
        if (_state != State.Idle)
            return;

        _target = target;
        _state = (target != null && target.GetComponent<Brain>() != null) ? State.ToBrain : State.Idle;

        if (_state == State.ToBrain)
            _mover.GoToTarget(_target);
    }

    public void OnPickUpAnimationEnd()
    {
        if (_base != null)
            GoToBase();
        else
            _mover.ResumeMovement();
    }

    public void GoToConstructionNewBase(Vector3 targetPosition, Action<Zombie> onArrived)
    {
        ClearTarget();
        _carriedBrain = null;
        SetIdle();

        _onBuildArrivedCallback = onArrived;
        _state = State.ToBuild;

        _zombies?.MarkBusyZombie(this);
        _mover.ResumeMovement();
        _mover.GoToPosition(targetPosition);
    }

    private void SetIdle()
    {
        _state = State.Idle;
        _target = null;
        _mover.ClearDestination();
    }

    private void HandleArrived()
    {
        switch (_state)
        {
            case State.ToBrain:
                TryStartPickUp();
                break;

            case State.CarryToBase:
                DeliverBrainToBase();
                break;

            case State.ToBuild:
                BuildNewBase();
                break;
        }
    }

    private void TryStartPickUp()
    {
        Brain brain = _target ? _target.GetComponent<Brain>() : null;

        if (brain == null)
        {
            ClearTarget();
            return;
        }

        _pickUpper.StartPickUp(brain);
        _mover.PauseMovement();
    }

    private void HandlePickUpDone(Brain pickedBrain)
    {
        _carriedBrain = pickedBrain;
        _pickUpper.AttachToCarry(_carriedBrain);
    }

    private void GoToBase()
    {
        if (_base == null)
            return;

        var basePoint = NearestBasePointFinder.GetNearestPointAroundBase(transform.position, _base, _nearestDistance);

        _mover.ResumeMovement();
        _mover.GoToPosition(basePoint);

        _target = _base;
        _state = State.CarryToBase;
    }

    private void DeliverBrainToBase()
    {
        if (_carriedBrain == null)
            return;

        Brain deliveredBrain = _carriedBrain;
        _carriedBrain = null;

        ClearTarget();
        _zombies?.MarkFreeZombie(this);

        BrainStorage storage = _storage;

        deliveredBrain.OnDelivered();

        if (storage != null)
            storage.AddBrain(deliveredBrain);
    }

    private void ClearTarget()
    {
        _target = null;
        _state = State.Idle;
        _mover.ClearDestination();
    }

    private void BuildNewBase()
    {
        var callback = _onBuildArrivedCallback;
        _onBuildArrivedCallback = null;

        SetIdle();
        callback?.Invoke(this);
    }
}