using System.Collections.Generic;
using UnityEngine;

public class BrainScanController : MonoBehaviour
{
    [SerializeField] private BrainScanner _scanner;
    [SerializeField] private ZombieDispatcher _zombies;
    
    public BrainScanner Scanner => _scanner;
    public ZombieDispatcher Zombies => _zombies;

    private void Awake()
    {
        if (_scanner != null)
            _scanner.StartAutoScan();
    }

    private void OnEnable()
    {
        if (_scanner != null)
            _scanner.BrainsScanned += OnBrainsScanned;
    }

    private void OnDisable()
    {
        if (_scanner != null)
            _scanner.BrainsScanned -= OnBrainsScanned;
    }
    
    private void OnBrainsScanned(IReadOnlyList<Brain> brains)
    {
        if (_zombies == null || brains == null)
            return;
        
        for (int i = 0; i < brains.Count; i++)
        {
            Brain brain = brains[i];
            
            if (brain == null)
                continue;

            if (!brain.TryReserve())
                continue;

            Zombie zombie = _zombies.FindAnyFreeZombie();

            if (zombie == null)
            {
                brain.CleanReservation();
                
                break;
            }
            
            _zombies.MarkBusyZombie(zombie);
            zombie.SetTarget(brain.transform);
        }
    }
}
