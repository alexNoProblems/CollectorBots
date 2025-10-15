using UnityEngine;

public class ZombieSpawner : OriginSpawner<Zombie>
{
    private const int AllLayersMask = ~0;

    [SerializeField] private BrainStorage _storage;
    [SerializeField] private BrainScanner _scanner;
    [SerializeField] private float _minRadius = 4f;
    [SerializeField] private float _maxRadius = 5f;
    [SerializeField] private float _minSpawnDistance = 2f;
    [SerializeField] private int _maxSpawnTries = 10;

    private ZombieDispatcher _dispatcher;

    public void Init(ZombieDispatcher dispatcher, BrainScanner scanner)
    {
        _dispatcher = dispatcher;
        _scanner = scanner;
    }

    public override Zombie SpawnAt(Vector3 position)
    {
        if (Pool == null)
            return null;

        Zombie zombie = Pool.Get();

        if (zombie == null)
            return null;

        var zombieGameobject = zombie.gameObject;
        
        if (zombieGameobject.activeSelf)
            zombieGameobject.SetActive(false);


        var zombieTransform = zombie.transform;
        zombieTransform.SetParent(null, true);
        zombieTransform.position = position;

        zombie.MakeDependencies(_dispatcher, _storage, Base);
        zombie.Init(ReleaseToPool);
        zombie.SetScanner(_scanner);
        zombie.SpawnTo(position);

        zombieGameobject.SetActive(true);

        return zombie;
    }
    protected override Vector3 GetSpawnPosition()
    {
        for (int i = 0; i < _maxSpawnTries; i++)
        {
            var position = SpawnUtils.RandomPointInAnnulusXZ(Base.position, _minRadius, _maxRadius);
            position.y = Base.position.y;

            if (IsFreeFromOtherZombie(position, _minSpawnDistance))
                return position;
        }

         var fallback = SpawnUtils.RandomPointInAnnulusXZ(Base.position, _minRadius, _maxRadius);
            fallback.y = Base.position.y;

        return fallback;
    }

    private bool IsFreeFromOtherZombie(Vector3 position, float radius)
    {
        var hits = Physics.OverlapSphere(position, radius, AllLayersMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].GetComponent<Zombie>() != null)
                return false;
        }

        return true;
    }

    private void ReleaseToPool(Zombie zombie)
    {
        zombie.gameObject.SetActive(false);
        Pool.Release(zombie);
    }
}
