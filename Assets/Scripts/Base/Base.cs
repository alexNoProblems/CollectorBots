using System;
using UnityEngine;

[RequireComponent(typeof(BrainStorage))]
public class Base : MonoBehaviour, IBrainCollector
{
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private BrainStorage _storage;

    public Transform BasePoint => _baseTransform;
    public BrainStorage Storage => _storage;

    public BaseState State { get; private set; } = BaseState.Idle;

    private void Awake()
    {
        if (_storage == null)
            _storage = GetComponent<BrainStorage>();
    }

    public void SetCollectingMode()
    {
        if (State == BaseState.Idle)
            State = BaseState.CollectingBrains;
    }

    public void SetStateSentUnit()
    {
        State = BaseState.Expanding;
    }

    public void SetStateExpansionComplete()
    {
        State = BaseState.Idle;
    }

    public void Collect(Zombie zombie, Brain brain)
    {
        if (brain == null || _storage == null)
            return;
        
        _storage.AddBrain(brain);
    }
}
