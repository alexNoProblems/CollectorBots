using UnityEngine;

public class FlagHandler : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] MouseClicker _clicker;
    [SerializeField] private FlagPlacer _flagPlacer;
   
    private void OnEnable()
    {
        _inputReader.Clicked += OnClicked;
        _inputReader.Canceled += OnCanceled;
    }

    private void OnDisable()
    {
        _inputReader.Clicked -= OnClicked;
        _inputReader.Canceled -= OnCanceled;
    }

    private void Update()
    {
        if (_flagPlacer.IsPlacing && !_inputReader.IsRotating())
        {
            if (_clicker.TryRaycastComponent<Ground>(_inputReader.Pointer(), out var hit, out _))
                _flagPlacer.UpdatePreview(hit.point, hit.normal);
        }
    }

    private void OnClicked()
    {
        if (_inputReader.IsRotating())
            return;
        
        if (_clicker.TryRaycastComponent<Base>(_inputReader.Pointer(), out var baseHit, out var selectedBase))
        {
            _flagPlacer.BeginPlacement(selectedBase);

            return;
        }

        if (_flagPlacer.IsPlacing && _clicker.TryRaycastComponent<Ground>(_inputReader.Pointer(), out var groundHit, out _))
            _flagPlacer.PlaceAt(groundHit.point, groundHit.normal);
    }

    private void OnCanceled()
    {
        if (_flagPlacer.IsPlacing)
            _flagPlacer.Cancel();
    }
}
