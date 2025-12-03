using System;
using System.Collections;
using UnityEngine;

public class BaseConstructor : MonoBehaviour
{
    private const float DirectionThreshold = 0.001f;
    private const float RaycastStartHeight = 10f;
    private const float RaycastDistance = 50f;

    [SerializeField] private BaseRoot _basePrefab;
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

        _initialBaseRoot.Initialize(
            constructor: this,
            flagPlacer: _flagPlacer,
            zombiePool: _zombiePool,
            zombiesOnStart: _initialBaseZombies);
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

        BaseRoot baseObject = Instantiate(_basePrefab, newBasePosition, rotation);

        var root = baseObject.GetComponent<BaseRoot>();

        if (root == null)
        {
            _running = null;

            yield break;
        }

        root.Initialize(
            constructor: this,
            flagPlacer: _flagPlacer,
            zombiePool: _zombiePool,
            zombiesOnStart: _newBaseZombies);
        
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
        
        BaseRoot baseObject = Instantiate(_basePrefab, newBasePosition, rotation);

        var root = baseObject.GetComponent<BaseRoot>();
        
        newBase = root != null ? root.Base : null;
        newStorage = root != null ? root.Storage : null;
        baseTransform = root != null ? root.transform : baseObject.transform;
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
