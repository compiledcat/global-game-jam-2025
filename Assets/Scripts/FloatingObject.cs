using System.Collections.Generic;
using UnityEngine;

public class BuoyancyObject : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _bounceDamp = 0.04f;
    [SerializeField] private List<Vector3> _samplePoints = new();
    private Transform _waterObject;

    private void Awake()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _waterObject = GameObject.FindGameObjectWithTag("Water").transform;
    }

    private void FixedUpdate()
    {
        foreach (var point in _samplePoints)
        {
            var worldPos = transform.TransformPoint(point);

            var forceFactor = 1f - (worldPos.y - (_waterObject.position.y));
            var massModifier = _rb.mass / _samplePoints.Count;

            if (forceFactor > 0f)
            {
                var uplift = -Physics.gravity * (forceFactor - _rb.linearVelocity.y * _bounceDamp);
                _rb.AddForceAtPosition(uplift * massModifier, worldPos);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (var point in _samplePoints)
        {
            Gizmos.DrawSphere(transform.TransformPoint(point), 0.1f);
        }
    }
}