using System.Collections.Generic;
using UnityEngine;

public class BrainDispatcher : MonoBehaviour
{
    private readonly List<Brain> _brains = new();
    private readonly Dictionary<Brain, Zombie> _brainToZombie = new();

    public void Register(Brain brain)
    {
        if (brain == null)
            return;
        
        if (!_brains.Contains(brain))
            _brains.Add(brain);

        brain.Despawned += OnBrainGone;
        brain.Delivered += OnBrainGone;
    }

    public void Unregister(Brain brain)
    {
        if (brain == null)
            return;

        _brains.Remove(brain);
        _brainToZombie.Remove(brain);

        brain.Despawned -= OnBrainGone;
        brain.Delivered -= OnBrainGone;
    }

    public bool IsBrainAssigned(Brain brain) => brain != null && _brainToZombie.ContainsKey(brain);

    public bool TryClaim(Brain brain, Zombie zombie)
    {
        if (brain == null || zombie == null)
            return false;
        
        if (IsBrainAssigned(brain))
            return false;

        _brainToZombie[brain] = zombie;

        return true;
    }

    public void UnclaimBrainByZombie(Zombie zombie)
    {
        if (zombie == null)
            return;

        Brain assignedBrain = null;

        foreach (var item in _brainToZombie)
        {
            if (item.Value == zombie)
            {
                assignedBrain = item.Key;

                break;
            }
        }

        if (assignedBrain != null)
            _brainToZombie.Remove(assignedBrain);
    }

    public Brain FindFirstFreeBrain()
    {
        foreach (var brain in _brains)
        {
            if (brain != null && !IsBrainAssigned(brain))
                return brain;
        }

        return null;
    }

    private void OnBrainGone(Brain brain)
    {
        Unregister(brain);
    }
}
