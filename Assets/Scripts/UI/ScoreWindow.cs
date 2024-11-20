using System.Collections.Generic;
using Networking.Host;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class ScoreWindow : MonoBehaviour
    {
        [SerializeField] private VerticalLayoutGroup scoreListHolder;
        [SerializeField] private PlayerScoreUIEntry scoreEntryUiPrefab;

        public void UpdateScores(PlayerDataStorage.PlayerData[] sortedDescendingPlayerData)
        {
            DestroyScoreEntries();
            
            foreach (var data in sortedDescendingPlayerData)
            {
                var playerColor = PlayerColor.GetFromColorType(data.PlayerColorType).baseColor;
                var playerScoreText = $"{data.PlayerName}: {data.PlayerScore}";
                
                var scoreEntry = Instantiate(scoreEntryUiPrefab, scoreListHolder.transform);
                scoreEntry.Initialize(playerColor, playerScoreText);
            }
        }

        private void DestroyScoreEntries()
        {
            for (var i = scoreListHolder.transform.childCount -1 ; i >= 0; i--)
            {
                Destroy(scoreListHolder.transform.GetChild(i).gameObject);
            }
        }
    }
}
