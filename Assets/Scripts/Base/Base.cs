using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private BrainStorage _storage;

    public Transform BasePoint => _baseTransform;
    public BrainStorage Storage => _storage;

    public BaseState State { get; private set; } = BaseState.Idle;

    public void SetCollectingMode()
    {
        if (State == BaseState.Idle)
            State = BaseState.CollectingBrains;
    }

    public void SetStateSentUnit()
    {
        State = BaseState.Expanding;
    }

    public void SetStateExpansionComplete()
    {
        State = BaseState.Idle;
    }
}
