using UnityEngine;

public abstract class OriginSpawner<T>: MonoBehaviour, ISpawner<T> where T: Component, IPoolable<T> 
{
    private const float RaycastLengthMultiplier = 2f;
    private const int AllLayersMask = ~0;

    [SerializeField] protected ResourcesPool<T> _pool;
    [SerializeField] protected Transform _base;
    [SerializeField] protected bool _isGrounded = false;
    [SerializeField] protected float _raycastHeight = 200f;
    [SerializeField] protected int _startCount = 0;

    protected virtual void Start()
    {
        for (int i = 0; i < _startCount; i++)
            Spawn();
    }

    public T Spawn()
    {
        if (_pool == null || _base == null)
            return null;

        var position = GetSpawnPosition();

        if (_isGrounded && TryGround(ref position))
            return SpawnAt(position);

        position.y = _base.position.y;

        return SpawnAt(position);
    }

    public virtual T SpawnAt(Vector3 position)
    {
        if (_pool == null)
            return null;

        var item = _pool.Get();

        if (item == null)
            return null;

        Transform spawnPlace = item.transform;
        spawnPlace.SetParent(null, true);
        spawnPlace.position = position;

        return item;
    }

    protected bool TryGround(ref Vector3 position)
    {
        Vector3 from = new Vector3(position.x, _raycastHeight, position.z);
        float rayLength = _raycastHeight * RaycastLengthMultiplier;

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
