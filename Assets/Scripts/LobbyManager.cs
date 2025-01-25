using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private RectTransform _lobbyPanel;
    [SerializeField] private LobbyPlayerBox _lobbyPlayerBoxPrefab;
    [SerializeField] private Transform _spawnLocation;

    private void Awake()
    {
        _playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoin);
    }

    private void OnPlayerJoin(PlayerInput player)
    {
        var lobbyPlayerBox = Instantiate(_lobbyPlayerBoxPrefab, _lobbyPanel);
        
        player.GetComponentInChildren<Camera>().gameObject.SetActive(false);
        
        var offset = Random.insideUnitCircle * 5f;
        player.transform.position = _spawnLocation.position + new Vector3(offset.x, 0, offset.y);
        Physics.SyncTransforms();
    }
}