using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Lobby
{
    public class LobbyUIPlayerEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image colorField;
        public ulong ClientId { get; private set; }
        public bool ReadyToPlay { get; private set; } = false;

        public void Initialize(string playerName, Color playerColor, ulong clientId)
        {
            UpdatePlayerName(playerName);
            UpdatePlayerColor(playerColor);
            ClientId = clientId;
        }

        public void UpdatePlayerName(string playerName)
        {
            nameText.text = playerName;
        }

        public void UpdatePlayerColor(Color color)
        {
            colorField.color = color;

            if (color.a > float.Epsilon) // not transparent
                ReadyToPlay = true;
        }
    }
}