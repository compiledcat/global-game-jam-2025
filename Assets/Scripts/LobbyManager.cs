using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private RectTransform _lobbyPanel;
    [SerializeField] private LobbyPlayerBox _lobbyPlayerBoxPrefab;
    [SerializeField] private Transform _spawnLocation;

    [SerializeField] private List<string> _playerVerbs;
    [SerializeField] private List<string> _playerNouns;
    private IDictionary<string, InputDevice> _players = new Dictionary<string, InputDevice>();

    private string GenerateUniqueName()
    {
        string playerName;
        do
        {
            playerName =
                $"{_playerVerbs[Random.Range(0, _playerVerbs.Count)]} " +
                $"{_playerNouns[Random.Range(0, _playerNouns.Count)]}";
        } while (_players.ContainsKey(playerName));

        return playerName;
    }

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
        player.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        player.GetComponentInChildren<CinemachineCamera>().gameObject.SetActive(false);
        
        var offset = Random.insideUnitCircle * 5f;
        player.transform.position = _spawnLocation.position + new Vector3(offset.x, 0, offset.y);
        player.transform.forward = _spawnLocation.position - player.transform.position;
        Physics.SyncTransforms();

        var lobbyPlayerBox = Instantiate(_lobbyPlayerBoxPrefab, _lobbyPanel);
        var playerName = GenerateUniqueName();
        lobbyPlayerBox.SetPlayerIndex(player.playerIndex + 1);
        lobbyPlayerBox.SetPlayerName(playerName);

        _players.Add(playerName, player.devices[0]);
    }

    private void OnPlayerJoinGame(PlayerInput player)
    {
        var cinemachineBrain = player.GetComponentInChildren<CinemachineBrain>();
        var cinemachineCamera = player.GetComponentInChildren<CinemachineCamera>();
        cinemachineBrain.ChannelMask = (OutputChannels)(1 << player.playerIndex + 1);
        cinemachineCamera.OutputChannel = (OutputChannels)(1 << player.playerIndex + 1);
        
        //var offset = Random.insideUnitCircle * 5f;
        //player.transform.position += new Vector3(offset.x, 0, offset.y);
        SplineContainer spline = FindFirstObjectByType<SplineContainer>();
        player.transform.position = (Vector3)spline.Splines[0][0].Position + spline.transform.position;
        player.transform.forward = (Vector3)spline.Splines[0][1].Position - player.transform.position;
        Physics.SyncTransforms();
    }

    public async void BeginGame()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _playerInputManager.DisableJoining();
        _playerInputManager.splitScreen = true;
        _playerInputManager.playerJoinedEvent.RemoveListener(OnPlayerJoinLobby);
        _playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoinGame);

        await SceneManager.LoadSceneAsync("Scenes/Main");

        foreach (var player in _players)
        {
            var playerInput = _playerInputManager.JoinPlayer(pairWithDevice: player.Value);
            playerInput.GetComponent<DuckController>().PlayerName = player.Key;
        }
    }

    public static void EndGame()
    {
        Cursor.lockState = CursorLockMode.None;

        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in cameras)
        {
            cam.enabled = false;
        }

        Camera.main.enabled = true;
        Transform camDestinationTransform = GameObject.FindWithTag("GameEndCameraTransform").transform;
        Camera.main.transform.position = camDestinationTransform.position;
        Camera.main.transform.rotation = camDestinationTransform.rotation;
    }
}