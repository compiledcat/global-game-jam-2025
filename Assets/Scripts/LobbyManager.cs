using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private RectTransform _lobbyPanel;
    [SerializeField] private LobbyPlayerBox _lobbyPlayerBoxPrefab;
    [SerializeField] private Transform _spawnLocation;

    private List<InputDevice> _playerDevices = new();

    public static LobbyManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        _playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoinBase);
        _playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoinLobby);
    }

    private void OnPlayerJoinBase(PlayerInput player)
    {
        player.gameObject.name = $"Player {player.playerIndex + 1}";
    }

    private void OnPlayerJoinLobby(PlayerInput player)
    {
        player.GetComponentInChildren<Camera>().gameObject.SetActive(false);
        var offset = Random.insideUnitCircle * 5f;
        player.transform.position = _spawnLocation.position + new Vector3(offset.x, 0, offset.y);
        Physics.SyncTransforms();

        Instantiate(_lobbyPlayerBoxPrefab, _lobbyPanel); // todo make this mean something

        _playerDevices.Add(player.devices[0]);
    }

    private void OnPlayerJoinGame(PlayerInput player)
    {
        Debug.Log("duck");
    }

    public async void BeginGame()
    {
        _playerInputManager.DisableJoining();
        _playerInputManager.splitScreen = true;
        _playerInputManager.playerJoinedEvent.RemoveListener(OnPlayerJoinLobby);
        _playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoinGame);

        await SceneManager.LoadSceneAsync("Scenes/Main");

        foreach (var device in _playerDevices)
        {
            _playerInputManager.JoinPlayer(pairWithDevice: device);
        }
    }
}