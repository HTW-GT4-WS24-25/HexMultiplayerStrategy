using System.Linq;
using GameEvents;
using Networking.Host;
using UI;
using Unity.Netcode;
using UnityEngine;

namespace Score
{
        public class ScoreUpdater: NetworkBehaviour
        {
                [SerializeField] ScoreWindow nightScoreWindow;
                [SerializeField] EndOfGameScoreWindow endOfGameScoreWindow;
        
                private ScoreCalculator _scoreCalculator;

                public void Initialize(ScoreCalculator scoreCalculator)
                {
                        _scoreCalculator = scoreCalculator;
                }

                public override void OnNetworkSpawn()
                {
                        ServerEvents.DayNightCycle.OnTurnEnded += DistributeScoresAtNight;
                        ServerEvents.DayNightCycle.OnGameEnded += HandleEndOfGame;
                }

                public override void OnNetworkDespawn()
                {
                        ServerEvents.DayNightCycle.OnTurnEnded -= DistributeScoresAtNight;
                        ServerEvents.DayNightCycle.OnGameEnded -= HandleEndOfGame;
                }

                #region Server

                private void DistributeScoresAtNight()
                {
                        IncrementScoresAtNight();
                        
                        DistributeScoresAtNightClientRpc(GetPlayerDataSortedByScore());
                }
                
                private void HandleEndOfGame()
                {
                        DistributeScoresAtEndOfGame();
                }

                private void DistributeScoresAtEndOfGame()
                {
                        IncrementScoresAtNight();
                        
                        DistributeScoresAtEndOfGameClientRpc(GetPlayerDataSortedByScore());
                }

                private void IncrementScoresAtNight()
                {
                        var scoresByPlayerId = _scoreCalculator.CalculatePlayerScores();

                        foreach (var (id, score) in scoresByPlayerId)
                        {
                                HostSingleton.Instance.GameManager.PlayerData.IncrementPlayerScore(id, score);
                        }    
                }

                private PlayerDataStorage.PlayerData[] GetPlayerDataSortedByScore()
                {
                        var playerScoreList = HostSingleton.Instance.GameManager.PlayerData.GetAllPlayerData();
                        return playerScoreList.OrderByDescending(data => data.PlayerScore).ToArray();
                }

                #endregion

                #region Client

                [ClientRpc]
                private void DistributeScoresAtNightClientRpc(PlayerDataStorage.PlayerData[] playerData)
                {
                        nightScoreWindow.UpdateScores(playerData);
                        nightScoreWindow.gameObject.SetActive(true);
                }

                [ClientRpc]
                private void DistributeScoresAtEndOfGameClientRpc(PlayerDataStorage.PlayerData[] playerData)
                {
                        endOfGameScoreWindow.UpdateScores(playerData);
                        endOfGameScoreWindow.gameObject.SetActive(true);
                }
                
                #endregion
        }
}