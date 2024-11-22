using Networking.Client;
using Networking.Host;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EndOfGameScoreWindow : ScoreWindow
    {
        [SerializeField] private TextMeshProUGUI headerText;

        public override void UpdateScores(PlayerDataStorage.PlayerData[] sortedDescendingPlayerData)
        {
            base.UpdateScores(sortedDescendingPlayerData);
            
            var winnersName = sortedDescendingPlayerData[0].PlayerName;
            headerText.text = $"{winnersName} wins!";
        }

        public void GoBackToMenu()
        {
            ClientSingleton.Instance.GameManager.GoToMenu();
        }
    }
}