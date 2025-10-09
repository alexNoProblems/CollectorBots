using System.Collections.Generic;
using UnityEngine;

public class ZombieDispatcher : MonoBehaviour
{
    private readonly List<Zombie> _zombies = new();
    private readonly HashSet<Zombie> _busyZombie = new();

    public void Register(Zombie zombie)
    {
        if (zombie == null)
            return;
        
        if (!_zombies.Contains(zombie))
            _zombies.Add(zombie);
    }

    public void Unregister(Zombie zombie)
    {
        if (zombie == null)
            return;

        _zombies.Remove(zombie);
        _busyZombie.Remove(zombie);
    }

    public void MarkBusyZombie(Zombie zombie)
    {
        if (zombie != null)
            _busyZombie.Add(zombie);
    }

    public void MarkFreeZombie(Zombie zombie)
    {
        if (zombie != null)
            _busyZombie.Remove(zombie);
    }

    public bool IsBusy(Zombie zombie) => zombie != null && _busyZombie.Contains(zombie);

    public Zombie FindAnyFreeZombie()
    {
        foreach (var zombie in _zombies)
        {
            if (zombie != null && !IsBusy(zombie))
                return zombie;
        }

        return null;
    }
}
