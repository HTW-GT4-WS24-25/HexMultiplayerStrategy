using System.Linq;
using Core.GameEvents;
using Core.UI.InGame;
using Core.UI.InGame.VictoryPoints;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;

namespace Core.Score
{
        public class ScoreUpdater: NetworkBehaviour
        {
                [SerializeField] VictoryPointDisplay victoryPointDisplay;
                [SerializeField] VictoryBanner victoryBanner;
        
                private ScoreCalculator _scoreCalculator;

                public override void OnNetworkSpawn()
                {
                        if(!IsServer) 
                                return;
                        
                        ServerEvents.DayNightCycle.OnTurnEnded += DistributeScoresAtNight;
                        ServerEvents.DayNightCycle.OnGameEnded += HandleEndOfGame;
                }

                public override void OnNetworkDespawn()
                {
                        if(!IsServer)
                                return;
                        
                        ServerEvents.DayNightCycle.OnTurnEnded -= DistributeScoresAtNight;
                        ServerEvents.DayNightCycle.OnGameEnded -= HandleEndOfGame;
                }

                #region Server
                
                public void Initialize(ScoreCalculator scoreCalculator)
                {
                        _scoreCalculator = scoreCalculator;
                        InitializeVictoryPointDisplayClientRpc(GetPlayerDataSortedByScore());
                }

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
                private void InitializeVictoryPointDisplayClientRpc(PlayerDataStorage.PlayerData[] playerData)
                {
                        victoryPointDisplay.Initialize(playerData);
                }

                [ClientRpc]
                private void DistributeScoresAtNightClientRpc(PlayerDataStorage.PlayerData[] playerData)
                {
                        victoryPointDisplay.UpdateScoresFromData(playerData);
                }

                [ClientRpc]
                private void DistributeScoresAtEndOfGameClientRpc(PlayerDataStorage.PlayerData[] playerData)
                {
                        var winner = playerData[0];
                        victoryPointDisplay.UpdateScoresForGameEnd(playerData, () =>
                        {
                               victoryBanner.ShowFor(winner); 
                        });
                }
                
                #endregion
        }
}