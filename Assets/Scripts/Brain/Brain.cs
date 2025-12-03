using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Brain : MonoBehaviour, IPoolable<Brain>
{
    private Collider _collider;
    private bool _isReserved;

    public event Action<Brain> Appeared;
    public event Action<Brain> PickedUp;
    public event Action<Brain> Delivered;
    public event Action<Brain> Despawned;
    public event Action<Brain> Released;
    
    public bool IsReserved => _isReserved;

    public bool TryReserve()
    {
        if (_isReserved)
            return false;
        
        _isReserved = true;

        return true;
    }

    public void CleanReservation()
    {
        _isReserved = false;
    }

    public void Init()
    {
        if (_collider == null)
            _collider = GetComponent<Collider>();
        
        if (_collider != null)
            _collider.enabled = true;

        _isReserved = false;
        transform.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        _isReserved = false;

        if (_collider != null)
            _collider.enabled = true;
        
        transform.localScale = Vector3.one;
        Appeared?.Invoke(this);
    }

    private void OnDisable()
    {
        Despawned?.Invoke(this);
        _isReserved = false;
    }
    
    public void Despawn()
    {
        _isReserved = false;
        Released?.Invoke(this);
    }

    public void AttachToZombie(Transform carryAnchor)
    {
        if (_collider != null)
            _collider.enabled = false;

        transform.SetParent(carryAnchor, true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        PickedUp?.Invoke(this);
    }

    public void OnDelivered()
    {
        Delivered?.Invoke(this);
        
        _isReserved = false;
        Despawn();
    }
}
