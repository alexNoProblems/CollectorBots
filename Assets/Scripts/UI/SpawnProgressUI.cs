using UnityEngine;
using UnityEngine.UI;

public class SpawnProgressUI : MonoBehaviour
{
    [SerializeField] private AdditionalZombieSpawner _additionalSpawner;
    [SerializeField] private Image _fill;
    [SerializeField] private CanvasGroup _group;

    [SerializeField, Range(0f, 1f)] private float _hiddenAlpha = 0f;
    [SerializeField, Range(0f, 1f)] private float _visibleAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float _emptyBar = 0f;
    [SerializeField, Range(0f, 1f)] private float _fullBar = 0f;

    private void Awake()
    {
        if (_group != null)
            _group.alpha = _hiddenAlpha;
        
        if (_fill != null)
            _fill.fillAmount = _emptyBar;
    }

    private void OnEnable()
    {
        if(_additionalSpawner != null)
        {
            _additionalSpawner.CountdownStarted += OnStarted;
            _additionalSpawner.ProgressChanged += OnProgress;
            _additionalSpawner.CountdownFinished += OnFinished;
        }
    }
    private void OnDisable()
    {
        if(_additionalSpawner != null)
        {
            _additionalSpawner.CountdownStarted -= OnStarted;
            _additionalSpawner.ProgressChanged -= OnProgress;
            _additionalSpawner.CountdownFinished -= OnFinished;
        }
    }

    private void OnStarted(float duration, float _)
    {
        if (_group != null) 
            _group.alpha = _visibleAlpha;

        if (_fill  != null) 
            _fill.fillAmount = _emptyBar;
    }

    private void OnProgress(float normalized)
    {
        if (_fill != null)
            _fill.fillAmount = normalized;
    }

    private void OnFinished()
    {
        if (_fill != null) 
            _fill.fillAmount = _fullBar;

        if (_group != null) 
            _group.alpha = _hiddenAlpha;
    }
}
