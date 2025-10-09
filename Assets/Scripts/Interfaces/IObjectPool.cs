using System;
using UnityEngine;

public interface IObjectPool<T> where T: Component, IPoolable<T>
{
    T Get();

    void Release(T item);

    int CountInactive { get; }
    Transform Container { get; }
}
