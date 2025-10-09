using System;
using UnityEngine;

public class BrainScanner : MonoBehaviour
{
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private BrainDispatcher _brains;
    [SerializeField] private ZombieDispatcher _zombies;

    private int _deliverCount;

    public int DeliveredCount => _deliverCount;
   
    public event Action<int> OnBrainDelivered;

    public void RegisterBrain(Brain brain)
    {
        if (brain == null)
            return;
            
        _brains.Register(brain);

        brain.PickedUp += HandleBrainPickedUp;
        brain.Despawned += HandleBrainGone;
        brain.Delivered += HandleBrainDelivered;

        Zombie freeZombie = _zombies.FindAnyFreeZombie();

        if (freeZombie != null)
            Assign(freeZombie, brain);
    }

    public void UnRegisterBrain(Brain brain)
    {
       if (brain == null)
            return;
        
        brain.PickedUp -= HandleBrainPickedUp;
        brain.Despawned -= HandleBrainGone;
        brain.Delivered -= HandleBrainDelivered;

        _brains.Unregister(brain);
    } 

    public void RegisterZombie(Zombie zombie)
    {
        _zombies.Register(zombie);

        if (_zombies.IsBusy(zombie))
            return;

        Brain freeBrain = _brains.FindFirstFreeBrain();

        if (freeBrain != null)
            Assign(zombie, freeBrain);
    }

    public void UnRegisterZombie(Zombie zombie)
    {
        _brains.UnclaimBrainByZombie(zombie);
        _zombies.Unregister(zombie);
    }

    public void NotifyZombieAvailable(Zombie zombie)
    {
        if (zombie == null || !zombie.IsAvailable)
            return;

        _zombies.MarkFreeZombie(zombie);

        Brain nextBrain = _brains.FindFirstFreeBrain();

        if (nextBrain != null)
            Assign(zombie, nextBrain);
    }

    private void Assign(Zombie zombie, Brain brain)
    {
       if (zombie == null || brain == null)
            return;
        
        if (!_brains.TryClaim(brain, zombie))
            return;

        zombie.SetBase(_baseTransform);
        zombie.SetTarget(brain.transform);
        _zombies.MarkBusyZombie(zombie);
    }

    private void HandleBrainPickedUp(Brain brain)
    {
        _brains.Unregister(brain);
    }

    private void HandleBrainGone(Brain brain)
    {
        _brains.Unregister(brain);
    }

    private void HandleBrainDelivered(Brain brain)
    {
        _deliverCount++;
        OnBrainDelivered?.Invoke(_deliverCount);
    }
}
