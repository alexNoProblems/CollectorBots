using UnityEngine;
using UnityEngine.UI;

public class SpawnProgressUI : MonoBehaviour
{
    [SerializeField] private AdditionalZombieSpawner _additionalSpawner;
    [SerializeField] private Slider _slider;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private Image _zombieHead;

    [SerializeField, Range(0f, 1f)] private float _hiddenAlpha = 0f;
    [SerializeField, Range(0f, 1f)] private float _visibleAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float _sliderMinValue = 0f;
    [SerializeField, Range(0f, 1f)] private float _sliderMaxValue = 1f;
    private void Awake()
    {
        if (_group != null)
            _group.alpha = _hiddenAlpha;
        
        if (_slider != null)
        {
            _slider.minValue = _sliderMinValue;
            _slider.maxValue = _sliderMaxValue;
            _slider.value = _sliderMinValue;
        }

        if (_zombieHead != null)
            _zombieHead.enabled = false;
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

        if (_zombieHead != null)
            _zombieHead.enabled = false;
    }

    private void OnStarted(float duration, float _)
    {
        if (_group != null) 
            _group.alpha = _visibleAlpha;

        if (_slider != null) 
            _slider.value = _sliderMinValue;
        
        if (_zombieHead != null)
            _zombieHead.enabled = true;
    }

    private void OnProgress(float normalized)
    {
        if (_slider != null)
            _slider.value = normalized;
    }

    private void OnFinished()
    {
        if (_slider != null) 
            _slider.value = _sliderMaxValue;

        if (_group != null) 
            _group.alpha = _hiddenAlpha;

        if (_zombieHead != null)
            _zombieHead.enabled = false;
    }
}
