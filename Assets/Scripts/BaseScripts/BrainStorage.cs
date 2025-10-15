using System;
using UnityEngine;

public class BrainStorage : MonoBehaviour
{
    private int _delivered;

    public int DeliveredCount => _delivered;

    public event Action<int> DeliveredChanged;

    public void AddBrain(Brain brain)
    {
        if (brain == null)
            return;

        _delivered++;
        DeliveredChanged?.Invoke(_delivered);
    }
}
