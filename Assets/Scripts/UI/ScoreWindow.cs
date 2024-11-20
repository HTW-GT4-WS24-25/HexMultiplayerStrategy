using Networking.Host;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreWindow : MonoBehaviour
    {
        [SerializeField] protected VerticalLayoutGroup scoreListHolder;
        [SerializeField] protected PlayerScoreUIEntry scoreEntryUiPrefab;

        public virtual void UpdateScores(PlayerDataStorage.PlayerData[] sortedDescendingPlayerData)
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
