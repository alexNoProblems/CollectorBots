using System;
using System.Collections.Generic;
using UnityEngine;

public class BrainScanner : MonoBehaviour
{
    [SerializeField] private float _radius = 20f;
    [SerializeField] private Transform _center;

    private readonly HashSet<Brain> _foundedBrain = new();

    private BrainDispatcher _brains;

    public BrainDispatcher Dispatcher => _brains;

    public event Action Scanned;

    private void Awake()
    {
        if (_center == null)
            _center = transform;
    }

    public void Init(BrainDispatcher brains)
    {
        _brains = brains;
    }

    public void Scan()
    {
        Collider[] hits = Physics.OverlapSphere(_center.position, _radius);

        float radiusSqr = _radius * _radius;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out Brain brain) && brain.isActiveAndEnabled)
                _foundedBrain.Add(brain);
        }

        _foundedBrain.RemoveWhere(brain => brain == null || (brain.transform.position - _center.position).sqrMagnitude > radiusSqr);

        Scanned?.Invoke();
    }
}
