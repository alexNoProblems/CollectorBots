using System;
using System.Collections;
using UnityEngine;

public class AdditionalZombieSpawner : MonoBehaviour
{
    private const float FullBarProgress = 1f;

    [SerializeField] private int _neededBrainsToSpawnZombie = 3;
    [SerializeField] private int _minZombiesForExpansion = 1;
    [SerializeField] private float _spawnDelay = 5f;
    [SerializeField] private float _minDuration = 0f;

    private BrainStorage _storage;
    private ZombieSpawner _zombieSpawner;
    private ExpansionHandler _expansionHandler;
    private ZombieDispatcher _zombieDispatcher;
    private Coroutine _countdownRoutine;
    private bool _isCountingDown;

    public event Action<float, float> CountdownStarted;
    public event Action<float> ProgressChanged;
    public event Action CountdownFinished;

    private void OnDisable()
    {
        if (_storage != null)
            _storage.BrainsDeliveredChanged -= OnDeliveredChanged;

        CancelCountdown();
    }

    public void Init( BrainStorage storage, ZombieSpawner spawner, ExpansionHandler expansion, ZombieDispatcher dispatcher)
    {
        if (_storage != null)
            _storage.BrainsDeliveredChanged -= OnDeliveredChanged;

        _storage = storage;
        _zombieSpawner = spawner;
        _expansionHandler = expansion;
        _zombieDispatcher = dispatcher;

        if (_storage != null)
            _storage.BrainsDeliveredChanged += OnDeliveredChanged;
    }

    private bool IsBlockedByExpansion(int totalZombies)
    {
        bool expansionInProgress = _expansionHandler != null && _expansionHandler.IsExpansionInProgress;
        bool expansionBlocking   = _zombieSpawner != null && _zombieSpawner.IsExpansionBlocking;

        bool block = (expansionInProgress || expansionBlocking) && totalZombies > _minZombiesForExpansion;

        return block;
    }

    private void OnDeliveredChanged(int delivered)
    {
        if (_storage == null || _zombieSpawner == null)
            return;

        int totalZombies = _zombieDispatcher != null ? _zombieDispatcher.TotalZombieCount : 0;
        
        if (IsBlockedByExpansion(totalZombies))
        {
            CancelCountdown();

            return;
        }

        if (!_isCountingDown && delivered >= _neededBrainsToSpawnZombie)
            _countdownRoutine = StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        _isCountingDown = true;

        float duration = Mathf.Max(_minDuration, _spawnDelay);
        float time = 0f;

        CountdownStarted?.Invoke(duration, _minDuration);
        
        while (time < duration)
        {
            int totalZombies = _zombieDispatcher != null ? _zombieDispatcher.TotalZombieCount : 0;

            if (IsBlockedByExpansion(totalZombies))
            {
                CancelCountdown();

                yield break;
            }

            time += Time.deltaTime;
            float normalized = duration > _minDuration ? Mathf.Clamp01(time / duration) : FullBarProgress;

            ProgressChanged?.Invoke(normalized);

            yield return null;
        }
        
        if (_zombieSpawner != null)
        {
            Zombie zombie = _zombieSpawner.SpawnOne(
                isConsumeBrains: true,
                isExpansionBlock: false
            );
        }

        CountdownFinished?.Invoke();

        _isCountingDown = false;
        _countdownRoutine = null;
    }

    private void CancelCountdown()
    {
        if (_countdownRoutine != null)
        {
            StopCoroutine(_countdownRoutine);
            _countdownRoutine = null;
        }

        if (_isCountingDown)
            CountdownFinished?.Invoke();

        _isCountingDown = false;
    }
}
