using System;
using System.Collections.Generic;
using UnityEngine;

public class FlagPlacer : MonoBehaviour
{
    private const string ColorProperty = "_Color";

    [SerializeField] private GameObject _flagPrefab;
    [SerializeField, Range(0f, 1f)] private float _flagAlpha = 0.5f;
    [SerializeField] private MapBounds _bounds;

    private readonly Dictionary<Base, GameObject> _placedFlags = new();
    private GameObject _flagPreview;
    private Renderer _previewRenderer;
    private Base _originBase;

    public bool IsPlacing { get; private set; }

    public event Action<Base, Vector3> FlagPlaced;

    public void BeginPlacement(Base originBase)
    {
        _originBase = originBase;

        if (_flagPreview == null)
        {
            _flagPreview = Instantiate(_flagPrefab);
            var view = _flagPreview.GetComponent<FlagView>();
            _previewRenderer = view != null ? view.Renderer : null;
            
            SetTransparent(_previewRenderer, _flagAlpha);
        }

        Vector3 startPosition = originBase != null ? originBase.BasePoint.position : Vector3.zero;
        _flagPreview.transform.SetPositionAndRotation(startPosition, Quaternion.identity);

        IsPlacing = true;
    }

    public void UpdatePreview(Vector3 worldPosition, Vector3 surfaceNormal)
    {
        if (!IsPlacing || _flagPreview == null)
            return;

        var boundedPosition = _bounds != null ? _bounds.ClampToBounds(worldPosition) : worldPosition;

        _flagPreview.transform.position = boundedPosition;
        _flagPreview.transform.up = surfaceNormal;
    }

    public void PlaceAt(Vector3 worldPosition, Vector3 surfaceNormal)
    {
        if (!IsPlacing || _originBase == null)
            return;

        if (_bounds != null && !_bounds.Contains(worldPosition))
            return;

        var rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);

        if (_placedFlags.TryGetValue(_originBase, out var existingFlag))
        {
            existingFlag.transform.SetPositionAndRotation(worldPosition, rotation);
        }
        else
        {
            var flag = Instantiate(_flagPrefab, worldPosition, rotation);
            _placedFlags[_originBase] = flag;
            _originBase.SetStateSentUnit();

            FlagPlaced?.Invoke(_originBase, worldPosition);
        }
        
        Cancel();
    }

    public void Cancel()
    {
        IsPlacing = false;
        _originBase = null;

        if (_flagPreview != null)
        {
            Destroy(_flagPreview);
            _flagPreview = null;
            _previewRenderer = null;
        }
    }

    public void RemoveFlag(Base originBase)
    {
        if (_placedFlags.TryGetValue(originBase, out var flag))
        {
            if (flag != null)
                Destroy(flag);
            
            _placedFlags.Remove(originBase);
        }
    }

    private void SetTransparent(Renderer renderer, float alpha)
    {
        if (renderer == null)
            return;
        
        foreach (var material in renderer.materials)
        {
            if (material.HasProperty(ColorProperty))
            {
                var color = material.color;
                color.a = alpha;
                material.color = color;
            }
        }
    }
}
