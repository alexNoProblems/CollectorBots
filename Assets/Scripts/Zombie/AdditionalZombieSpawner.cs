using System;
using System.Collections;
using UnityEngine;

public class AdditionalZombieSpawner : MonoBehaviour
{
    private const float FullBarProgress = 1f;

    [SerializeField] BrainStorage _storage;
    [SerializeField] ZombieSpawner _zombieSpawner;
    [SerializeField] private int _neededBrainsToSpawnZombie = 3;
    [SerializeField] private float _spawnDelay = 5f;
    [SerializeField] private float _minDuration = 0f;

    public event Action<float, float> CountdownStarted;
    public event Action<float> ProgressChanged;
    public event Action CountdownFinished;

    private bool _isCountingDown;

    private void OnEnable()
    {
        if (_storage != null)
            _storage.BrainsDeliveredChanged += OnDeliveredChanged;
    }

    private void OnDisable()
    {
        if (_storage != null)
            _storage.BrainsDeliveredChanged -= OnDeliveredChanged;
    }

    private void OnDeliveredChanged(int delivered)
    {
        if (!_isCountingDown && delivered >= _neededBrainsToSpawnZombie)
            StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        _isCountingDown = true;

        float duration = Mathf.Max(_minDuration, _spawnDelay);
        float time = 0f;

        CountdownStarted?.Invoke(duration, _minDuration);

        while (time < duration)
        {
            time += Time.deltaTime;
            float normalized = duration > _minDuration ? Mathf.Clamp01(time / duration) : FullBarProgress;
            ProgressChanged?.Invoke(normalized);

            yield return null;
        }

        _zombieSpawner?.Spawn();
        _storage.ResetCount();
        CountdownFinished?.Invoke();

        _isCountingDown = false;
    }
}
