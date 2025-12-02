using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDispatcher : MonoBehaviour
{
    private readonly List<Zombie> _zombies = new();
    private readonly HashSet<Zombie> _busyZombies = new();

    public event Action<Zombie> ZombieFreed;

    public int TotalZombieCount
    {
        get
        {
            int count = 0;

            foreach (var zombie in _zombies)
            {
                if (zombie != null)
                    count++;
            }

            return count;
        }
    }

    public int FreeZombieCount
    {
        get
        {
            int count = 0;

            foreach (var zombie in _zombies)
            {
                if (zombie == null || !zombie.isActiveAndEnabled)
                    continue;
                
                if (!IsBusy(zombie) && zombie.IsAvailable)
                    count++;
            }

            return count;
        }
    }

    public void Register(Zombie zombie)
    {
        if (zombie == null)
            return;
        
        if (!_zombies.Contains(zombie))
        {
            _zombies.Add(zombie);

            if (zombie.isActiveAndEnabled && zombie.IsAvailable)
                ZombieFreed?.Invoke(zombie);
        }
    }

    public void Unregister(Zombie zombie)
    {
        if (zombie == null)
            return;

        _zombies.Remove(zombie);
        _busyZombies.Remove(zombie);
    }

    public void MarkBusyZombie(Zombie zombie)
    {
        if (zombie == null)
            return;

        _busyZombies.Add(zombie);
    }

    public void MarkFreeZombie(Zombie zombie)
    {
        if (zombie == null)
            return;

        _busyZombies.Remove(zombie);

        if (zombie.isActiveAndEnabled && zombie.IsAvailable)
            ZombieFreed?.Invoke(zombie);
    }

    public bool IsBusy(Zombie zombie) => zombie != null && _busyZombies.Contains(zombie);

    public bool TryGetFreeZombie(out Zombie freeZombie)
    {
        foreach (var zombie in _zombies)
        {
            if (zombie == null || !zombie.isActiveAndEnabled)
                continue;

            bool busy = IsBusy(zombie);
            bool available = zombie.IsAvailable;

            if (busy || !available)
                continue;

            freeZombie = zombie;

            return true;
        }

        freeZombie = null;

        return false;
    }

    public Zombie FindAnyFreeZombie()
    {
        return TryGetFreeZombie(out var zombie) ? zombie : null;
    }
}
