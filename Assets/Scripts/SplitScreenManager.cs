using UnityEngine;
using UnityEngine.InputSystem;


public class SplitScreenManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager;

    private void OnEnable()
    {
        _playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoined);
        _playerInputManager.playerLeftEvent.AddListener(OnPlayerLeft);
    }

    private void OnDisable()
    {
        _playerInputManager.playerJoinedEvent.RemoveListener(OnPlayerJoined);
        _playerInputManager.playerLeftEvent.RemoveListener(OnPlayerLeft);
    }

    private void OnPlayerJoined(PlayerInput arg0)
    {
        Debug.Log("player joined");
    }

    private void OnPlayerLeft(PlayerInput arg0)
    {
        Debug.Log("player left");
    }

    private void Start()
    {
        //     int numCams = GameState.NumPlayers;
        //     if (numCams == 3) numCams = 4; // fourth camera will be used for general stuff
        //
        //     GameObject[] camObjects = new GameObject[numCams];
        //     for (int i = 0; i < numCams; ++i)
        //     {
        //         camObjects[i] = new GameObject($"Player {i + 1} Camera");
        //         switch (numCams)
        //         {
        //             case 1:
        //             {
        //                 camObjects[i].AddComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        //                 break;
        //             }
        //             case 2:
        //             {
        //                 camObjects[i].AddComponent<Camera>().rect = new Rect(i * 0.5f, 0, 0.5f, 1);
        //                 break;
        //             }
        //             case 4:
        //             {
        //                 if (i == 0 || i == 1)
        //                 {
        //                     camObjects[i].AddComponent<Camera>().rect = new Rect(i * 0.5f, 0.5f, 0.5f, 0.5f);
        //                 }
        //                 else
        //                 {
        //                     camObjects[i].AddComponent<Camera>().rect = new Rect((i - 2) * 0.5f, 0.0f, 0.5f, 0.5f);
        //                 }
        //
        //                 break;
        //             }
        //         }
        //     }
    }
}