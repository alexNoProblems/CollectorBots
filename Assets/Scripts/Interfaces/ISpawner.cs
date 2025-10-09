using UnityEngine;

public interface ISpawner<T> where T : Component, IPoolable<T>
{
    T Spawn();

    T SpawnAt(Vector3 position);
}
