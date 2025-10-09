using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesPool<T>: MonoBehaviour, IObjectPool<T> where T :Component, IPoolable<T> 
{
    [SerializeField] private T _prefab;
    [SerializeField] private int _initializeSize = 20;
    [SerializeField] Transform _poolContainer;
    [SerializeField] private bool _expendable = true;

    private readonly Queue<T> _pool = new();

    public int CountInactive => _pool.Count;

    public Transform Container => _poolContainer != null ? _poolContainer : transform;

    private void Awake()
    {
        if (_prefab == null)
            return;
        
        if (_poolContainer == null)
            _poolContainer = transform;

        InitializePool(_initializeSize);

    }
 
    public T Get()
    {
        if (_pool.Count == 0)
        {
            if ((!_expendable))
                return null;

            _pool.Enqueue(CreatePoolObject());
        }

        var item = _pool.Dequeue();
        item.gameObject.SetActive(true);

        return item;
    }

    public void Release(T item)
    {
        if (item == null)
            return;
        
        item.gameObject.SetActive(false);
        item.transform.SetParent(_poolContainer, false);
        _pool.Enqueue(item);
    }

    private void InitializePool(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
            _pool.Enqueue(CreatePoolObject());
    }

    private T CreatePoolObject()
    {
        var item = Instantiate(_prefab, Container);
        item.gameObject.SetActive(false);
        item.Init(Release);

        return item;
    }
}
