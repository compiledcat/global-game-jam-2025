using System.Linq;
using TMPro;
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

    [SerializeField] private TextMeshProUGUI _lapTimeText;
    [SerializeField] private TextMeshProUGUI _positionText;

    private InputAction _moveAction;
    private Vector2 _move;

    public string PlayerName;
    public int NextCheckpointIndex;

    private float _lapStartTime;

    private string GetOrdinal(int n)
    {
        switch (n)
        {
            case 1:
                return "st";
            case 2:
                return "nd";
            case 3:
                return "rd";
            default:
                return "th";
        }
    }

    public void AdvanceCheckpoint()
    {
        NextCheckpointIndex++;
        Debug.Log($"PASSED CHECKPOINT {NextCheckpointIndex - 1}");
        // todo check stuff idk
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _moveAction = _playerInput.actions["Move"];
    }

    private void Update()
    {
        _move = _moveAction.ReadValue<Vector2>();

        // bank into turns and acceleration
        var bank = Vector3.Dot(_rb.linearVelocity, transform.right) / _maxSpeed * _bankMultiplier;
        var pitch = Vector3.Dot(_rb.linearVelocity, transform.forward) / _maxSpeed * _bankMultiplier;
        _mesh.localRotation = Quaternion.Euler(-pitch, 0f, bank);
        
        if (_lapTimeText.gameObject.activeInHierarchy)
        {
            var sortedPlayers = LeaderboardManager.GetSortedPlayerArray().ToList();
            var lapTimeFormatted = System.TimeSpan.FromSeconds(Time.time - _lapStartTime).ToString("mm\\:ss\\.fff");
            _lapTimeText.text = lapTimeFormatted;

            var position = sortedPlayers.IndexOf(this) + 1;
            _positionText.text = $"<size=200%>{position}{GetOrdinal(position)}</size> / {sortedPlayers.Count}";
        }
    }

    private void FixedUpdate()
    {
        // todo sample dynamic water height
        if (transform.position.y <= 9.2f)
        {
            _rb.AddForce(transform.forward * (_move.y * _maxSpeed));
        }

        _rb.AddTorque(transform.up * (_move.x * _rotateSpeed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CheckpointScript checkpoint))
        {
            Debug.Log($"Collided with checkpoint {checkpoint.checkpointIndex} with current player checkpoint index of {NextCheckpointIndex - 1}");
            if (NextCheckpointIndex == checkpoint.checkpointIndex)
            {
                AdvanceCheckpoint();
            }
        }
    }
}