using System;
using UnityEngine;

public interface IPoolable<T> where T : Component
{
    void Init(Action<T> releaseToPool);
}
