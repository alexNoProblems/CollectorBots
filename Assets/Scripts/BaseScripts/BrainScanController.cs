using UnityEngine;

public class BrainScanController : MonoBehaviour
{
    [SerializeField] private BrainScanner _scanner;
    [SerializeField] private BrainDispatcher _brains;
    [SerializeField] private ZombieDispatcher _zombies;
    [SerializeField] private BrainSpawner _brainSpawner;
    [SerializeField] private ZombieSpawner _zombieSpawner;

    public BrainScanner Scanner => _scanner;
    public BrainDispatcher Brains => _brains;
    public ZombieDispatcher Zombies => _zombies;

    private void Awake()
    {
        if (_scanner != null)
            _scanner.Init(_brains);
        
        if (_brainSpawner != null)
            _brainSpawner.Init(_brains);
        
        if (_zombieSpawner != null)
            _zombieSpawner.Init(_zombies, _scanner);
    }

    private void OnEnable()
    {
        if (_scanner != null)
            _scanner.Scanned += OnScanned;
    }

    private void OnDisable()
    {
        if (_scanner != null)
            _scanner.Scanned -= OnScanned;
    }

    private void OnScanned()
    {
        if (_brains == null || _zombies == null)
            return;

        int paired = 0;

        while (enabled)
        {
            Brain brain = _brains.FindFirstFreeBrain();
            Zombie zombie = _zombies.FindAnyFreeZombie();

            if (brain == null || zombie == null)
                break;

            if (_brains.TryClaim(brain, zombie))
            {
                _zombies.MarkBusyZombie(zombie);
                zombie.SetTarget(brain.transform);
                paired++;
            }
        }
    }
}
