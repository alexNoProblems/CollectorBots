using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Fog : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fogEffect;

    private void Awake()
    {
        if (_fogEffect == null)
            _fogEffect = GetComponent<ParticleSystem>();
    }

    public void Play(float radius, float seconds)
    {
        _fogEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var shape = _fogEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius;

        _fogEffect.Clear(true);
        _fogEffect.Play(true);

        Destroy(gameObject, seconds);
    }
}
