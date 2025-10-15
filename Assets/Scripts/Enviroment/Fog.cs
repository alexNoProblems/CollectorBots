using UnityEngine;

public class Fog : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fogEffect;

    public void Play(float radius, float seconds)
    {
        if (_fogEffect == null)
            _fogEffect = GetComponent<ParticleSystem>();

        _fogEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var shape = _fogEffect.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius;

        _fogEffect.Clear(true);
        _fogEffect.Play(true);

        Destroy(gameObject, seconds);
    }
}
