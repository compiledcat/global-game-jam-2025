using UnityEngine;

public class DuckController : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 1f;
    [SerializeField] private float _bankMultiplier = 2f;

    [SerializeField] private Transform _mesh;

    private float _throttle;
    private float _turn;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _throttle = Input.GetAxis("Vertical");
        _turn = Input.GetAxis("Horizontal");

        // bank into turns and acceleration
        var bank = Vector3.Dot(_rb.linearVelocity, transform.right) * _bankMultiplier;
        var pitch = Vector3.Dot(_rb.linearVelocity, transform.forward) * _bankMultiplier;
        _mesh.localRotation = Quaternion.Euler(-pitch, 0f, bank);
    }

    private void FixedUpdate()
    {
        _rb.AddForce(transform.forward * (_throttle * _maxSpeed));
        _rb.AddTorque(transform.up * (_turn * _rotateSpeed));
    }
}