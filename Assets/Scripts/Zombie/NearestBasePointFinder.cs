using UnityEngine;
using UnityEngine.AI;

public class NearestBasePointFinder
{
    private const float Radius = 2f;

    public static Vector3 GetNearestPointAroundBase(Vector3 from, Transform baseTransform)
    {
        Vector3 position = baseTransform.position;

        if (baseTransform.TryGetComponent<Collider>(out var collider))
            position = collider.ClosestPoint(from);
        
        if (NavMesh.SamplePosition(position, out var hit, Radius, NavMesh.AllAreas))
            return hit.position;

        return position;
    }
}
