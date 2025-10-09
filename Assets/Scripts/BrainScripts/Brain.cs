using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Brain : MonoBehaviour, IPoolable<Brain>
{
    private Collider _collider;
    private BrainScanner _scanner;

    private Action<Brain> _release;

    public void Init(Action<Brain> releaseToPool)
    {
        _release = releaseToPool;

        if (_collider == null)
            _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        _scanner?.RegisterBrain(this);
    }

    private void OnDisable()
    {
        _scanner?.UnRegisterBrain(this);
    }
    
    public void Despawn()
    {
        _release?.Invoke(this);
    }

    public void SetScanner(BrainScanner scanner)
    {
        _scanner = scanner;

        if (isActiveAndEnabled)
            _scanner.RegisterBrain(this);
    }

    public void OnPickUp(Transform carryAnchor)
    {
        if (_collider != null)
            _collider.enabled = false;

        transform.SetParent(carryAnchor, true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDelivered()
    {
        Despawn();
    }
}
