using UnityEngine;
using UnityEngine.Pool;

public class PrefabPool<T>: MonoBehaviour, IObjectPool<T> where T :Component, IPoolable<T> 
{
    private const int defaultInactiveCount = 0;

    [SerializeField] private T _prefab;
    [SerializeField] private int _initializeSize = 20;
    [SerializeField] private int _maxSize = 100;
    [SerializeField] private Transform _poolContainer;
    [SerializeField] private bool _collectionCheck = false;

    private ObjectPool<T> _pool;

    public int CountInactive => _pool?.CountInactive ?? defaultInactiveCount;

    public Transform Container => _poolContainer != null ? _poolContainer : transform;

    private void Awake()
    {
        TryInitializePool();
    }

    public T Get()
    {
        if (_pool == null && !TryInitializePool())
            return null;

        return _pool.Get();
    }

    public void Release(T item)
    {
        if (_pool == null)
            return;
        
        if(item == null)
            return;
        
        _pool.Release(item);
    }  

    private bool TryInitializePool()
    {
        if (_prefab == null)
            return false;
        
        if (_poolContainer == null)
            _poolContainer = transform;
        
        if (_pool == null)
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
        
        return true;
    }

    private T Create()
    {
        var item = Instantiate(_prefab, Container);
        item.gameObject.SetActive(false);
        item.Released += Release;
        item.Init();

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
        if (item == null)
            return;
        
        item.Released -= Release;
        Destroy(item.gameObject);
    }
}
