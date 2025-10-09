using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Brain : MonoBehaviour, IPoolable<Brain>
{
    private Collider _collider;

    public event Action<Brain> Appeared;
    public event Action<Brain> PickedUp;
    public event Action<Brain> Delivered;
    public event Action<Brain> Despawned;
    public event Action<Brain> Released;

    public void Init(Action<Brain> releaseToPool)
    {
        Released = releaseToPool;

        if (_collider == null)
            _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        Appeared?.Invoke(this);
    }

    private void OnDisable()
    {
        Despawned?.Invoke(this);
    }
    
    public void Despawn()
    {
        Released?.Invoke(this);
    }

    public void OnPickUp(Transform carryAnchor)
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

        Despawn();
    }
}
