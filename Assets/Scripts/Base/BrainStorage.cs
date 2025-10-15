using System;
using UnityEngine;

public class BrainStorage : MonoBehaviour
{
    private int _delivered;

    public event Action<int> BrainsDeliveredChanged;

    public int DeliveredCount => _delivered;

    public void AddBrain(Brain brain)
    {
        if (brain == null)
            return;

        _delivered++;
        BrainsDeliveredChanged?.Invoke(_delivered);
    }
}
