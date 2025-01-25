using UnityEngine;
using UnityEngine.InputSystem;

public class DuckController : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 1f;
    [SerializeField] private float _bankMultiplier = 2f;

    [SerializeField] private Transform _mesh;

    [SerializeField] private PlayerInput _playerInput;
    
    private InputAction _moveAction;
    
    private float _throttle;
    private float _turn;

    public int NextCheckpointIndex;

    public void AdvanceCheckpoint()
    {
        NextCheckpointIndex++;
        // todo check stuff idk
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _moveAction = _playerInput.actions["Move"];
    }

    private void Update()
    {
        var move = _moveAction.ReadValue<Vector2>();
        _throttle = move.y;
        _turn = move.x;

        // bank into turns and acceleration
        var bank = Vector3.Dot(_rb.linearVelocity, transform.right) / _maxSpeed * _bankMultiplier;
        var pitch = Vector3.Dot(_rb.linearVelocity, transform.forward) / _maxSpeed * _bankMultiplier;
        _mesh.localRotation = Quaternion.Euler(-pitch, 0f, bank);
    }

    private void FixedUpdate()
    {
        _rb.AddForce(transform.forward * (_throttle * _maxSpeed));
        _rb.AddTorque(transform.up * (_turn * _rotateSpeed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Checkpoint checkpoint))
        {
            if (NextCheckpointIndex == checkpoint.CheckpointIndex)
            {
                AdvanceCheckpoint();
            }
        }
    }
}