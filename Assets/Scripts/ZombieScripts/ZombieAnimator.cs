using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class ZombieAnimator : MonoBehaviour
{
    private const int BaseLayerIndex = 0;
    [SerializeField] private float _moveThresholdSqr = 0.0004f;
    [SerializeField] private string _pickUpStateName = "PickUp";

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    private Animator _animator;
    private int _pickUpStateHash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _pickUpStateHash = Animator.StringToHash(_pickUpStateName);
    }

    public void OverridePickUpStateName(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
            return;

        _pickUpStateName = stateName;
        _pickUpStateHash = Animator.StringToHash(_pickUpStateName);
    }

    public void PlayPickUp(float crossFadeDuration = 0.05f, int layer = BaseLayerIndex)
    {
        _animator.CrossFade(_pickUpStateHash, crossFadeDuration, layer);
        _animator.SetBool(IsMoving, false);
    }

    public void SetMoving(bool isMoving)
    {
        _animator.SetBool(IsMoving, isMoving);
    }

    public bool IsInPickUp(out float normalizedTime)
    {
        var state = _animator.GetCurrentAnimatorStateInfo(BaseLayerIndex);
        normalizedTime = state.normalizedTime;

        return state.shortNameHash == _pickUpStateHash;
    }
}
