using System;
using UnityEngine;

public interface IPoolable<T> where T : Component
{
    event Action<T> Released;
    void Init();
}
