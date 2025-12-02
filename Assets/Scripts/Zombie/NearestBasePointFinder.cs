using UnityEngine;
using UnityEngine.AI;

public class NearestBasePointFinder
{
    private const float Radius = 2f;

    public static Vector3 GetNearestPointAroundBase(Vector3 from, Transform baseTransform, float standoffDistance)
    {
        Vector3 position = baseTransform.position;

        if (baseTransform.TryGetComponent<Collider>(out var collider))
            position = collider.ClosestPoint(from);

        Vector3 direction = (from - position).normalized;
        Vector3 targetPoint = position + direction * standoffDistance;

        if (NavMesh.SamplePosition(targetPoint, out var hit, Radius, NavMesh.AllAreas))
            return hit.position;

        return targetPoint;
    }
}
