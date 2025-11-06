using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClicker : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxRayDistance = 2000f;

    private void Reset()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    public bool TryRaycastComponent<T>(Vector2 screenPosition, out RaycastHit hit, out T component) where T : Component
    {
        hit = default;
        component = null;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return false;

        var ray = _camera.ScreenPointToRay(screenPosition);
        var hits = Physics.RaycastAll(ray, _maxRayDistance);

        if (hits == null || hits.Length == 0)
            return false;

        float bestDistance = float.MaxValue;
        RaycastHit bestHit = default;
        T bestComponent = null;

        for (int i = 0; i < hits.Length; i++)
        {
            var item = hits[i];

            if (!item.collider.TryGetComponent<T>(out var found))
                found = item.collider.GetComponentInParent<T>();
            
            if (found == null)
                continue;
            
            if (item.distance < bestDistance)
            {
                bestDistance = item.distance;
                bestHit = item;
                bestComponent = found;
            }
        }

        if (bestComponent == null)
            return false;

        hit = bestHit;
        component = bestComponent;

        return true;
    }
}
