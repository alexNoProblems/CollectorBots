using System;
using System.Collections;
using UnityEngine;

public class BaseConstructor : MonoBehaviour
{
    private const float DirectionThreshold = 0.001f;
    private const float RaycastStartHeight = 10f;
    private const float RaycastDistance = 50f;

    [SerializeField] private GameObject _basePrefab;
    [SerializeField] private GameObject _dustFxPrefab;
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private FlagPlacer _flagPlacer;
    [SerializeField] private PrefabPool<Zombie> _zombiePool;
    [SerializeField] private BaseRoot _initialBaseRoot;
    [SerializeField] private int _initialBaseZombies = 3;
    [SerializeField] private int _newBaseZombies = 1;
    [SerializeField] private float _buildDuration = 3f;
    [SerializeField] private float _zombieStandoff = 1.5f;

    private Coroutine _running;
    private WaitForSeconds _waitForSeconds;

    private void Awake()
    {
        _waitForSeconds = new WaitForSeconds(_buildDuration);

        if (_initialBaseRoot == null)
            return;

        _initialBaseRoot.InjectDependencies(this, _flagPlacer);

        var root = _initialBaseRoot;

        if (root.Spawner != null && root.Dispatcher != null && root.Scanner != null)
        {
            if (_zombiePool != null)
                root.Spawner.SetPool(_zombiePool);

            Transform basePoint = (root.Base != null && root.Base.BasePoint != null) ? root.Base.BasePoint : root.transform;

            root.Spawner.Init(root.Dispatcher, root.Scanner, root.Storage, root.Base, _flagPlacer, basePoint);
            root.Spawner.SpawnInitial(_initialBaseZombies);
        }

        if (root.AdditionalSpawner != null)
            root.AdditionalSpawner.Init(root.Storage, root.Spawner, root.Expansion, root.Dispatcher);
    }

    public void Init(FlagPlacer flagPlacer)
    {
        _flagPlacer = flagPlacer;
    }

    public void StartConstruction(Vector3 buildPosition, Zombie builder)
    {
        if (_running != null)
            StopCoroutine(_running);

        _running = StartCoroutine(BuildRoutine(buildPosition, builder));
    }

    private IEnumerator BuildRoutine(Vector3 buildPosition, Zombie builder)
    {
        if (builder != null)
            builder.Despawn();

        GameObject dustFX = SpawnDustFX(buildPosition);

        yield return _waitForSeconds;

        if (dustFX != null)
            Destroy(dustFX);

        Vector3 newBasePosition = SnapToGround(buildPosition);
        Quaternion rotation = _baseTransform != null ? _baseTransform.rotation : _basePrefab.transform.rotation;

        GameObject baseObject = Instantiate(_basePrefab, newBasePosition, rotation);

        var root = baseObject.GetComponent<BaseRoot>();

        if (root == null)
        {
            _running = null;

            yield break;
        }

        root.InjectDependencies(this, _flagPlacer);

        Base newBase = root.Base;
        BrainStorage newStorage = root.Storage;
        ZombieDispatcher newDispatcher = root.Dispatcher;
        BrainScanner newScanner = root.Scanner;
        ExpansionHandler newExpansion = root.Expansion;
        ZombieSpawner newSpawner = root.Spawner;
        BrainScanController newScanCtrl = root.ScanController;

        if (newSpawner != null && newScanner != null && newDispatcher != null)
        {
            if (_zombiePool != null)
                newSpawner.SetPool(_zombiePool);

            Transform basePoint = (newBase != null && newBase.BasePoint != null) ? newBase.BasePoint : root.transform;

            newSpawner.Init(newDispatcher, newScanner, newStorage, newBase,_flagPlacer, basePoint);

            newSpawner.SpawnInitial(_newBaseZombies);
        }

        root.AdditionalSpawner?.Init(newStorage, newSpawner, newExpansion,
        newDispatcher);

        if (newScanCtrl != null)
            newScanCtrl.ForceInitAfterConstruction();

        _running = null;
    }

    private GameObject SpawnDustFX(Vector3 at)
    {
        if (_dustFxPrefab == null)
            return null;

        var dustFX = Instantiate(_dustFxPrefab, SnapToGround(at), Quaternion.identity);
        var particleSystem = dustFX.GetComponent<ParticleSystem>();

        if (particleSystem == null)
            particleSystem = dustFX.GetComponentInChildren<ParticleSystem>();

        particleSystem.Play();

        return dustFX;
    }

    private void CreateBase(Vector3 buildPosition, out Base newBase, out BrainStorage newStorage, out Transform baseTransform)
    {
        Vector3 newBasePosition = SnapToGround(buildPosition);
        Quaternion rotation = _baseTransform != null ? _baseTransform.rotation : _basePrefab.transform.rotation;
        
        GameObject baseObject = Instantiate(_basePrefab, newBasePosition, rotation);

        newBase = baseObject.GetComponent<Base>();
        newStorage = baseObject.GetComponent<BrainStorage>();
        baseTransform = baseObject.transform;
    }

    private Vector3 SnapToGround(Vector3 position)
    {
        var ray = new Ray(position + Vector3.up * RaycastStartHeight, Vector3.down);

        if (Physics.Raycast(ray, out var hit, RaycastDistance))
        {
            if (hit.collider.GetComponent<Ground>() != null)
                return new Vector3(position.x, hit.point.y, position.z);
        }
        return position;
    }
}
