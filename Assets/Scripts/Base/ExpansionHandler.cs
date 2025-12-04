using System;
using UnityEngine;

[RequireComponent(typeof(Base), typeof(BrainStorage), typeof(ZombieDispatcher))]
[RequireComponent(typeof(BrainScanner), typeof(ZombieSpawner))]
public class ExpansionHandler : MonoBehaviour
{
    [SerializeField] private FlagPlacer _flagPlacer;
    [SerializeField] private int _brainsRequired = 5;
    [SerializeField] private int _minZombieCount = 1;
    [SerializeField] private ZombieSpawner _zombieSpawner;

    private Base _base;
    private BrainStorage _storage;
    private ZombieDispatcher _zombieDispatcher;
    private BrainScanner _scanner;
    private BaseConstructor _baseConstructor;
    private bool _hasPendingFlag;
    private bool _isZombieDispatched;
    private Vector3 _flagPosition;

    public bool HasPendingExpansion => _hasPendingFlag && !_isZombieDispatched;
    public bool IsExpansionInProgress => _hasPendingFlag || _isZombieDispatched;


    public void Init(BaseConstructor baseConstructor, FlagPlacer flagPlacer)
    {
        _baseConstructor = baseConstructor;
        
        _base = GetComponent<Base>();
        _storage = GetComponent<BrainStorage>();
        _zombieDispatcher = GetComponent<ZombieDispatcher>();
        _scanner = GetComponent<BrainScanner>();
        _zombieSpawner = GetComponent<ZombieSpawner>();

        if (_flagPlacer != null)
            _flagPlacer.FlagPlaced -= OnFlagPlaced;

        _flagPlacer = flagPlacer;

        if (_flagPlacer != null)
            _flagPlacer.FlagPlaced += OnFlagPlaced;

        _storage.BrainsCountChanged += OnBrainChanged;
        _zombieDispatcher.ZombieFreed += OnZombieFreed;
    }

    private void OnDestroy()
    {
        if (_flagPlacer != null)
            _flagPlacer.FlagPlaced -= OnFlagPlaced;

        if (_storage != null)
            _storage.BrainsCountChanged -= OnBrainChanged;

        if (_zombieDispatcher != null)
            _zombieDispatcher.ZombieFreed -= OnZombieFreed;
    }

    private void OnFlagPlaced(Base originBase, Vector3 position)
    {
        if (originBase != _base)
            return;

        _base.SetStateSentUnit();
        _flagPosition = position;
        _hasPendingFlag = true;

        TryDispatchBuilder();
    }

    private void OnBrainChanged(int deliveredCount)
    {
        if (_hasPendingFlag && !_isZombieDispatched)
            TryDispatchBuilder();
    }

    private void OnZombieFreed(Zombie zombie)
    {
        if (_hasPendingFlag && !_isZombieDispatched)
            TryDispatchBuilder();
    }
    private void TryDispatchBuilder()
    {
        if (!_hasPendingFlag || _isZombieDispatched)
            return;
        
        if (_storage == null || _zombieDispatcher == null)
            return;

        int delivered = _storage.Count;

        if (delivered < _brainsRequired)
            return;
        
        if (_zombieDispatcher.TotalZombieCount <= _minZombieCount)
            return;

        if (!_zombieDispatcher.TryGetFreeZombie(out var builder))
            return;

        if (!_storage.TryClean(_brainsRequired))
            return;

        _zombieDispatcher.MarkBusyZombie(builder);
        builder.GoToConstructionNewBase(_flagPosition, OnBuilderArrived);

        _isZombieDispatched = true;
    }

    private void OnBuilderArrived(Zombie builder)
    {   
        if (_baseConstructor == null)
            return;

        _flagPlacer.RemoveFlag(_base);
        _baseConstructor.StartConstruction(_flagPosition, builder);

        _zombieDispatcher.MarkFreeZombie(builder);
        _hasPendingFlag = false;
        _isZombieDispatched = false;

        _zombieSpawner.NotifyExpansionFinished(_base);
    }
}
