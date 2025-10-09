using System;
using UnityEngine;

[RequireComponent(typeof(ZombieMover), typeof(ZombiePickUpper))] 
public class Zombie : MonoBehaviour, IPoolable<Zombie>
{
    private enum State { Idle, ToBrain, CarryToBase}

    [SerializeField] private BrainStorage _storage;
    [SerializeField] private Transform _model;
    [SerializeField] private Transform _carryAnchor;

    private BrainScanner _scanner;
    private Transform _base;
    private ZombieMover _mover;
    private ZombiePickUpper _pickUpper;
    private State _state = State.Idle;
    private Transform _target;
    private Brain _carriedBrain;
    private ZombieAnimator _animator;

    public bool HasTarget => _target != null;
    public bool IsAvailable => _state == State.Idle && _carriedBrain == null;
    public Vector3 TargetPosition => _target != null ? _target.position : transform.position;
    private bool IsPickingUp => _pickUpper != null && _pickUpper.IsPickingUp;

    public event Action<Zombie> Released;

    private void Awake()
    {
        _mover = GetComponent<ZombieMover>();
        _pickUpper = GetComponent<ZombiePickUpper>();
        _animator = GetComponent<ZombieAnimator>();

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
        _mover.OnArrived += HandleArrived;
        _pickUpper.OnPickedUpCompleted += HandlePickUpDone;

        _scanner?.RegisterZombie(this);
    }

    private void OnDisable()
    {
        _scanner?.UnRegisterZombie(this);

        _mover.OnArrived -= HandleArrived;
        _pickUpper.OnPickedUpCompleted -= HandlePickUpDone;
    }

    public void Init(Action<Zombie> releaseToPool)
    {
        Released += releaseToPool;
        _pickUpper.BindCarryAnchor(_carryAnchor);
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

        if (isActiveAndEnabled)
            _scanner.RegisterZombie(this);
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
            default:
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
        
        var basePoint = NearestBasePointFinder.GetNearestPointAroundBase(transform.position, _base);
        _mover.ResumeMovement();
        _mover.GoToPosition(basePoint);

        _target = _base;
        _state = State.CarryToBase;
    }
    
    private void DeliverBrainToBase()
    {
        if (_carriedBrain != null)
        {
            _carriedBrain.OnDelivered();

            if (_storage != null)
                _storage.AddBrain(_carriedBrain);
            
            _carriedBrain = null;

            ClearTarget();
            _scanner?.NotifyZombieAvailable(this);
        }
    }

    private void ClearTarget()
    {
        _target = null;
        _state = State.Idle;
        _mover.ClearDestination();
    }
}
