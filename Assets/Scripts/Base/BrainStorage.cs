using System;
using UnityEngine;

public class BrainStorage : MonoBehaviour
{
    public event Action<int> BrainsCountChanged;
    
    public int Count { get; private set; }

    public bool TryClean(int amount)
    {
        if (amount <= 0)
            return true;
        
        if (Count < amount)
            return false;

        Count -= amount;
        BrainsCountChanged?.Invoke(Count);

        return true;
    }
    
    public void AddBrain(Brain brain)
    {
        if (brain == null)
            return;

        Count++;
        BrainsCountChanged?.Invoke(Count);
    }

    public void ResetCount()
    {
        if (Count != 0)
            return;
        
        Count= 0;
        BrainsCountChanged?.Invoke(Count);
    }
}
