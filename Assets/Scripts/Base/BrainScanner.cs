using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainScanner : MonoBehaviour
{
    [SerializeField] private float _radius = 20f;
    [SerializeField] private Transform _center;
    [SerializeField] private LayerMask _scanMask = ~0;
    [SerializeField] private QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Ignore;
    [SerializeField] private float _scanInterval = 6f;
    [SerializeField] private Fog _fogPrefab;
    [SerializeField] private float _fogDuration;
    [SerializeField] private bool _isAutoScanEnable = true;

    private BrainDispatcher _brains;
    private Coroutine _loopTime;
    private WaitForSeconds _waitForSeconds;
    private bool _isScanningNow;

    public event Action<float, Vector3> Scanned;

    public BrainDispatcher Dispatcher => _brains;

    private void Awake()
    {
        if (_center == null)
            _center = transform;

        _waitForSeconds = new WaitForSeconds(_scanInterval);
    }

    private void OnEnable()
    {
        if (_isAutoScanEnable)
            StartAutoScan();
    }

    private void OnDisable()
    {
        StopAutoScan();
    }

    public void Init(BrainDispatcher brains)
    {
        _brains = brains;
    }

    public void StartAutoScan()
    {
        if (_loopTime != null)
            return;

        _loopTime = StartCoroutine(ScanLoop());
    }

    public void StopAutoScan()
    {
        if (_loopTime == null)
            return;

        StopCoroutine(_loopTime);
        _loopTime = null;
    }

    public void Scan()
    {
        if (_isScanningNow)
            return;

        _isScanningNow = true;

        Physics.OverlapSphere(_center.position, _radius, _scanMask, _triggerInteraction);

        Scanned?.Invoke(_radius, _center.position);

        _isScanningNow = false;
    }

    private IEnumerator ScanLoop()
    {
        while (enabled)
        {
            yield return _waitForSeconds;

            Scan();
        }
    }
}
