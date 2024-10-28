using TMPro;
using UnityEngine;

namespace UI
{
    public class LobbyUIPlayerEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        public ulong ClientId { get; private set; }
        public bool ReadyToPlay { get; private set; }

        public void Initialize(string playerName, ulong clientId)
        {
            UpdatePlayerName(playerName);
            ClientId = clientId;
            
            ReadyToPlay = true;
        }

        public void UpdatePlayerName(string playerName)
        {
            nameText.text = playerName;
        }
    }
}