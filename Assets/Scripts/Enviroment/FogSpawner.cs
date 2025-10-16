using UnityEngine;

public class FogSpawner : MonoBehaviour
{
    [SerializeField] private BrainScanner _scanner;
    [SerializeField] private Fog _fogPrefab;
    [SerializeField] private float _fogDuration = 2f;

    private void OnEnable()
    {
        if (_scanner != null)
            _scanner.Scanned += HandleScanned;
    }

    private void OnDisable()
    {
        if (_scanner != null)
            _scanner.Scanned -= HandleScanned;
    }

    private void HandleScanned(float radius, Vector3 position)
    {
        if (_fogPrefab == null)
            return;
        
        var fog = Instantiate(_fogPrefab, position, Quaternion.identity);
        fog.Play(radius, _fogDuration);
    }
}
