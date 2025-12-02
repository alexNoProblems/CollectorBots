using UnityEngine;

public class ZombieSpawner : OriginSpawner<Zombie>
{
    private const int AllLayersMask = ~0;

    [SerializeField] private BrainStorage _storage;
    [SerializeField] private BrainScanner _scanner;
    [SerializeField] private FlagPlacer _flagPlacer;
    [SerializeField] private Base _ownerBase;
    [SerializeField] private float _minRadius = 4f;
    [SerializeField] private float _maxRadius = 5f;
    [SerializeField] private float _minSpawnDistance = 2f;
    [SerializeField] private int _maxSpawnTries = 10;    
    [SerializeField] private int _brainsForExpansion = 5;
    [SerializeField] private int _brainsPerZombie = 3;
    [SerializeField] private int _zombiesOnStart = 0;

    private ZombieDispatcher _dispatcher;
    private bool _expansionRequested;

    public int BrainsPerZombie => _brainsPerZombie;

    public bool IsExpansionBlocking => _expansionRequested;

    private void OnEnable()
    {
        if (_flagPlacer != null)
            _flagPlacer.FlagPlaced += OnFlagPlaced;
    }

    private void OnDisable()
    {
        if (_flagPlacer != null)
            _flagPlacer.FlagPlaced -= OnFlagPlaced;
    }

    public void Init(ZombieDispatcher dispatcher, BrainScanner scanner, BrainStorage storage, Base ownerBase, FlagPlacer flagPlacer, Transform basePoint)
    {
        _dispatcher = dispatcher;
        _scanner = scanner;
        _storage = storage;
        _ownerBase = ownerBase;
        _flagPlacer = flagPlacer;

        Base = basePoint != null ? basePoint : (_ownerBase != null ? _ownerBase.transform : transform);
    }

    public void SpawnInitialFromBase()
    { 
        SpawnInitial(_zombiesOnStart);
    }

    public void SpawnInitial(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 position = GetSpawnPosition();
            SpawnInternal(position, isConsumeBrains: false, isExpansionBlock: false);
        }
    }

    public Zombie SpawnOne(bool isConsumeBrains, bool isExpansionBlock)
    {
        Vector3 position = GetSpawnPosition();
        return SpawnInternal(position, isConsumeBrains, isExpansionBlock);
    }

    public override Zombie SpawnAt(Vector3 position)
    {
        return SpawnInternal(position, isConsumeBrains: true, isExpansionBlock: true);
    }

    public void NotifyExpansionFinished(Base originBase)
    {
        if (originBase != null && originBase == _ownerBase)
            _expansionRequested = false;
    }

    public bool CanSpawnNow()
    {
        if (_storage == null)
            return false;

        int available = GetAvailableBrains();

        return available >= _brainsPerZombie;
    }

    public void SetPool(PrefabPool<Zombie> pool)
    {
        Pool = pool;
    }

    protected override Vector3 GetSpawnPosition()
    {
        var origin = Base != null ? Base : transform;

        if (Base == null)
            Base = origin;
        
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

    private Zombie SpawnInternal(Vector3 position, bool isConsumeBrains, bool isExpansionBlock)
    {
        if (isExpansionBlock && _expansionRequested)
            return null;
        
        if (isConsumeBrains && (_storage == null || !_storage.TryClean(_brainsPerZombie)))
            return null;
        
        if (Pool == null)
            return null;
        
        Zombie zombie = Pool.Get();
        if (zombie == null)
            return null;
        
        var zombieGameObject = zombie.gameObject;

        if (zombieGameObject.activeSelf)
            zombieGameObject.SetActive(false);

        var zombieTransform = zombie.transform;
        zombieTransform.SetParent(null, true);
        zombieTransform.position = position;

        zombie.MakeDependencies(_dispatcher, _storage, Base);
        zombie.SetScanner(_scanner);
        zombie.SpawnTo(position);
        zombie.FinalizeSetup();

        zombieGameObject.SetActive(true);
        zombie.Init();

        OnSpawned(zombie);

        return zombie;
    }

    private void OnFlagPlaced(Base originBase, Vector3 _)
    {
        if (originBase != null && originBase == _ownerBase)
            _expansionRequested = true;
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

    private int GetAvailableBrains()
    {
        if (_storage == null)
            return 0;
        
        int delivered = _storage.DeliveredCount;

        if (_expansionRequested)
        {
            int blockedBrains = Mathf.Min(_brainsForExpansion, delivered);
            delivered -= blockedBrains;
        }

        return delivered;
    }
}
