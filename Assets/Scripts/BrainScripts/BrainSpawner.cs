using System.Collections;
using UnityEngine;

public class BrainSpawner : OriginSpawner<Brain>
{
    [SerializeField] private float _minDistance = 3f;
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _spawnInterval = 15f;
    [SerializeField] private float _spawnHeightOffset = 1f;

    private Coroutine _spawnRoutine;
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        _waitForSeconds = new WaitForSeconds(_spawnInterval);
    }

    protected override void Start()
    {
        base.Start();
        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (enabled)
        {
            Spawn();

            yield return _waitForSeconds;
        }
    }

    protected override Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition;

        if (_minDistance > 0)
            spawnPosition = SpawnUtils.RandomPointInAnnulusXZ(Base.position, _minDistance, _maxDistance);
        else
            spawnPosition = SpawnUtils.RandomPointInCircleXZ(Base.position, _maxDistance);

        spawnPosition.y += _spawnHeightOffset;

        return spawnPosition;
    }
}
