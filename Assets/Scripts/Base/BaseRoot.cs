using UnityEngine;

public class BaseRoot : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private BrainStorage _storage;
    [SerializeField] private ZombieDispatcher _dispatcher;
    [SerializeField] private BrainScanner _scanner;
    [SerializeField] private ExpansionHandler _expansion;
    [SerializeField] private ZombieSpawner _spawner;
    [SerializeField] private BrainScanController _scanController;
    [SerializeField] private AdditionalZombieSpawner _additionalSpawner;

    public Base Base => _base;
    public BrainStorage Storage => _storage;
    public ZombieDispatcher Dispatcher => _dispatcher;
    public BrainScanner Scanner => _scanner;
    public ExpansionHandler Expansion => _expansion;
    public ZombieSpawner Spawner => _spawner;
    public BrainScanController ScanController => _scanController;
    public AdditionalZombieSpawner AdditionalSpawner => _additionalSpawner;

    public void Initialize(BaseConstructor constructor, FlagPlacer flagPlacer, PrefabPool<Zombie> zombiePool,
        int zombiesOnStart)
    {
        if (_expansion != null)
            _expansion.Init(constructor, flagPlacer);
        
        if (_spawner != null)
        {
            Transform basePoint = _base != null && _base.BasePoint != null ? _base.BasePoint : (_base != null ? _base.transform : transform);
            
            _spawner.Init(
                dispatcher: _dispatcher,
                scanner: _scanner,
                storage: _storage,
                ownerBase: _base,
                flagPlacer: flagPlacer,
                basePoint: basePoint);
        }

        if (zombiePool != null)
            _spawner.SetPool(zombiePool);

        if (zombiesOnStart > 0)
            _spawner.SpawnInitial(zombiesOnStart);

        if (_additionalSpawner != null)
            _additionalSpawner.Init(_storage, _spawner, _expansion, _dispatcher);
    }
    public void InjectDependencies(BaseConstructor constructor, FlagPlacer flagPlacer)
    {
        Initialize(constructor, flagPlacer, null, 0);
    }
}
