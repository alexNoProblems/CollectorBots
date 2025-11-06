using UnityEngine;

public class MapBounds : MonoBehaviour
{
    [SerializeField] private Collider _area;

    public bool Contains(Vector3 position) => _area != null && _area.bounds.Contains(position);

    public Vector3 ClampToBounds(Vector3 position)
    {
        if (_area == null)
            return position;

        var bounds = _area.bounds;

        return new Vector3(Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
            Mathf.Clamp(position.y, bounds.min.y, bounds.max.y),
            Mathf.Clamp(position.z, bounds.min.z, bounds.max.z)
        );
    }
}
