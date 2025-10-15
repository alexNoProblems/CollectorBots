using UnityEngine;
using UnityEngine.Pool;

public class PrefabPool<T>: MonoBehaviour, IObjectPool<T> where T :Component, IPoolable<T> 
{
    [SerializeField] private T _prefab;
    [SerializeField] private int _initializeSize = 20;
    [SerializeField] private int _maxSize = 100;
    [SerializeField] private Transform _poolContainer;
    [SerializeField] private bool _collectionCheck = false;

    private ObjectPool<T> _pool;

    public int CountInactive => _pool?.CountInactive ?? 0;

    public Transform Container => _poolContainer != null ? _poolContainer : transform;

    private void Awake()
    {
        if (_prefab == null)
            return;
        
        if (_poolContainer == null)
            _poolContainer = transform;

        InitializePool(_initializeSize);
    }

    public T Get() => _pool.Get();

    public void Release(T item) => _pool.Release(item);

    private void InitializePool(int poolSize)
    {
        _pool = new ObjectPool<T>(
            createFunc: Create,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: _collectionCheck,
            defaultCapacity: _initializeSize,
            maxSize: _maxSize
        );
    }

    private T Create()
    {
        var item = Instantiate(_prefab, Container);
        item.gameObject.SetActive(false);
        item.Init();
        item.Released += Release;

        return item;
    }

    private void OnGet(T item)
    {
        item.transform.SetParent(null, true);
        item.gameObject.SetActive(true);
    }

    private void OnRelease(T item)
    {
        item.gameObject.SetActive(false);
        item.transform.SetParent(Container, false);
    }

    private void OnDestroyItem(T item)
    {
        if (item != null)
        {
            item.Released -= Release;
            Destroy(item.gameObject);
        }
    }
}
