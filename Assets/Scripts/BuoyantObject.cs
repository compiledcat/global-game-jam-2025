using System.Collections.Generic;
using UnityEngine;

public class BuoyantObject : MonoBehaviour
{
    private Rigidbody _rb;
    private const float _waterLevel = 0f; // todo maybe sample from waves if we have those?

    [SerializeField] private List<Vector3> _samplePositions = new();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        foreach (var samplePoint in _samplePositions)
        {
            var samplePointWorld = transform.TransformPoint(samplePoint);

            if (samplePointWorld.y < _waterLevel)
            {
                var buoyancyForce = (1f + Mathf.Abs(samplePointWorld.y - _waterLevel)) * Physics.gravity.magnitude;
                _rb.AddForceAtPosition(
                    Vector3.up * (buoyancyForce * (_rb.mass / _samplePositions.Count)),
                    samplePointWorld
                );
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var point in _samplePositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(point), 0.1f);
        }
    }
}