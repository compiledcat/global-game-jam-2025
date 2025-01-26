using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyantObject : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _bounceDamp = 0.04f;
    [SerializeField] private List<Vector3> _samplePoints = new();
    private Transform _waterObject;

    [SerializeField] private int _updateEveryNFrames = 1;

    private int _framesSinceLastUpdate;
    
    private WaterDisplacementManager _waterDisplacementManager;

    private void Awake()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _waterObject = GameObject.FindGameObjectWithTag("Water").transform;
        _waterDisplacementManager = FindAnyObjectByType<WaterDisplacementManager>();
    }

    private void FixedUpdate()
    {
        _framesSinceLastUpdate++;
        if (_framesSinceLastUpdate < _updateEveryNFrames) return;

        _framesSinceLastUpdate = 0;
        
        foreach (var point in _samplePoints)
        {
            var worldPos = transform.TransformPoint(point);

            var waveHeight = new Vector2(worldPos.x, worldPos.z);
            var forceFactor = 1f - (worldPos.y - (_waterObject.position.y + _waterDisplacementManager.GetHeightAtPosition(waveHeight)));
            var massModifier = _rb.mass / _samplePoints.Count;

            if (forceFactor > 0f)
            {
                var uplift = -Physics.gravity * (forceFactor - _rb.linearVelocity.y * _bounceDamp);
                _rb.AddForceAtPosition(uplift * (massModifier * _updateEveryNFrames), worldPos);
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