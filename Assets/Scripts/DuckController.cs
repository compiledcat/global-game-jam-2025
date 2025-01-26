using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DuckController : MonoBehaviour
{
    private Rigidbody _rb;

    private bool hasControl = true; //Player loses control once they finish the race

    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 1f;
    [SerializeField] private float _bankMultiplier = 2f;

    [SerializeField] private Transform _mesh;

    [SerializeField] private PlayerInput _playerInput;

    [SerializeField] private TextMeshProUGUI _lapTimeText;
    [SerializeField] private TextMeshProUGUI _positionText;

    public uint lapCounter; //0-indexed

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
        Debug.Log($"PASSED CHECKPOINT {NextCheckpointIndex}");
        if ((NextCheckpointIndex % CheckpointHandler.checkpoints.Length == 0) && NextCheckpointIndex != 0)
        {
            Debug.Log($"PASSED CHECKPOINT {NextCheckpointIndex}");
            Debug.Log($"PASSED LAP {lapCounter}");
            if (lapCounter == 2)
            {
                //Finished final lap
                Debug.Log("RACE COMPLETE.");

                hasControl = false;
                if (LeaderboardManager.GetSortedPlayerArray().Last() == this)
                {
                    //Player is in last place
                    //=> All players have finished
                    //=> Race is over
                    LobbyManager.EndGame();
                }
            }

            lapCounter++;
        }

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
        if (hasControl)
        {
            _move = _moveAction.ReadValue<Vector2>();
        }

        // bank into turns and acceleration
        var bank = Vector3.Dot(_rb.linearVelocity, transform.right) / _maxSpeed * _bankMultiplier;
        var pitch = Vector3.Dot(_rb.linearVelocity, transform.forward) / _maxSpeed * _bankMultiplier;
        _mesh.localRotation = Quaternion.Euler(-pitch, 0f, bank);

        if (_lapTimeText.gameObject.activeInHierarchy)
        {
            var sortedPlayers = LeaderboardManager.GetSortedPlayerArray().ToList();
            var lapTimeFormatted = System.TimeSpan.FromSeconds(Time.time - _lapStartTime).ToString("mm\\:ss\\.fff");
            _lapTimeText.text = $"{lapTimeFormatted}\nLap {lapCounter + 1}/3";

            var position = sortedPlayers.IndexOf(this) + 1;
            _positionText.text = $"<size=200%>{position}{GetOrdinal(position)}</size> / {sortedPlayers.Count}";
        }
    }

    private void FixedUpdate()
    {
        // todo sample dynamic water height
        if (transform.position.y <= 1f)
        {
            _rb.AddForce(transform.forward * (_move.y * _maxSpeed));
        }

        _rb.AddTorque(transform.up * (_move.x * _rotateSpeed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CheckpointScript checkpoint))
        {
            Debug.Log(
                $"Collided with checkpoint {checkpoint.checkpointIndex} with current player checkpoint index of {NextCheckpointIndex - 1}");
            if (NextCheckpointIndex == checkpoint.checkpointIndex)
            {
                AdvanceCheckpoint();
            }
        }
    }
}