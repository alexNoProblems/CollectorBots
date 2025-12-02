using System;
using UnityEngine;

public class BrainStorage : MonoBehaviour
{
    private int _delivered;

    public event Action<int> BrainsDeliveredChanged;

    public int DeliveredCount => _delivered;

    public bool TryClean(int amount)
    {
        if (amount <= 0)
            return true;
        
        if (_delivered < amount)
            return false;

        _delivered -= amount;
        BrainsDeliveredChanged?.Invoke(_delivered);

        return true;
    }
    
    public void AddBrain(Brain brain)
    {
        if (brain == null)
            return;

        _delivered++;
        BrainsDeliveredChanged?.Invoke(_delivered);
    }

    public void ResetCount()
    {
        if (_delivered != 0)
        {
            _delivered = 0;
            BrainsDeliveredChanged?.Invoke(_delivered);
        }
    }
}
