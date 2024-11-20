using System.Collections.Generic;
using Networking.Host;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class ScoreWindow : MonoBehaviour
    {
        [SerializeField] private Transform scoreListHolder;
        [SerializeField] private TextMeshProUGUI scoreEntryUiPrefab;

        public void UpdateScores(List<PlayerDataStorage.PlayerData> sortedDescendingPlayerData)
        {
            DestroyScoreEntries();
            
            foreach (var data in sortedDescendingPlayerData)
            {
                var scoreEntry = Instantiate(scoreEntryUiPrefab, scoreListHolder.transform);
                scoreEntry.text = $"{data.PlayerName}: {data.PlayerScore}";
            }
        }

        private void DestroyScoreEntries()
        {
            for (var i = scoreListHolder.childCount -1 ; i >= 0; i--)
            {
                Destroy(scoreListHolder.GetChild(i));
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
