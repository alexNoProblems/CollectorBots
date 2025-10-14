using System;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Transform _baseTransform;

    public Transform BasePoint => _baseTransform;
}
