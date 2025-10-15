using System;
using UnityEngine;

public abstract class OriginSpawner<T>: MonoBehaviour, ISpawner<T> where T: Component, IPoolable<T> 
{
    private const float RaycastLengthMultiplier = 2f;
    private const int AllLayersMask = ~0;

    [SerializeField] protected PrefabsHandler<T> Pool;
    [SerializeField] protected Transform Base;
    [SerializeField] protected bool IsGrounded = false;
    [SerializeField] protected float RaycastHeight = 200f;
    [SerializeField] protected int StartCount = 0;

    public event Action<T> Spawned;

    protected virtual void Start()
    {
        for (int i = 0; i < StartCount; i++)
            Spawn();
    }

    public T Spawn()
    {
        if (Pool == null || Base == null)
            return null;

        var position = GetSpawnPosition();

        if (IsGrounded && TryGetGroundPosition(ref position))
            return SpawnAt(position);

        position.y = Base.position.y;

        return SpawnAt(position);
    }

    public virtual T SpawnAt(Vector3 position)
    {
        if (Pool == null)
            return null;

        var item = Pool.Get();

        if (item == null)
            return null;

        Transform spawnPlace = item.transform;
        spawnPlace.SetParent(null, true);
        spawnPlace.position = position;

        Spawned?.Invoke(item);

        return item;
    }

    protected bool TryGetGroundPosition(ref Vector3 position)
    {
        Vector3 from = new Vector3(position.x, RaycastHeight, position.z);
        float rayLength = RaycastHeight * RaycastLengthMultiplier;

        if (Physics.Raycast(from, Vector3.down, out var hit, rayLength, AllLayersMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.GetComponent<Ground>() != null)
            {
                position = hit.point;

                return true;
            }
        }

        return false;
    }

    protected abstract Vector3 GetSpawnPosition();
}
