using TMPro;
using UnityEngine;

public class LobbyPlayerBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _playerIndexText;

    public void SetPlayerName(string playerName) => _playerNameText.text = playerName;
    public void SetPlayerIndex(int index) => _playerIndexText.text = index.ToString();
}