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
    private BrainDispatcher _brains;

    private void Awake()
    {
        _waitForSeconds = new WaitForSeconds(_spawnInterval);
    }

    protected override void Start()
    {
        _spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (_brains == null)
            yield return null;

        while (enabled)
        {
            Spawn();

            yield return _waitForSeconds;
        }
    }

    public void Init(BrainDispatcher brains)
    {
        _brains = brains;
    }

    public override Brain SpawnAt(Vector3 position)
    {
        Brain brain = base.SpawnAt(position);

        if(brain != null)
            _brains.Register(brain);

        return brain;
    }

    protected override Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition;

        if (_minDistance > 0)
            spawnPosition = SpawnUtils.RandomPointInAnnulusXZ(_base.position, _minDistance, _maxDistance);
        else
            spawnPosition = SpawnUtils.RandomPointInCircleXZ(_base.position, _maxDistance);

        spawnPosition.y += _spawnHeightOffset;

        return spawnPosition;
    }
}
