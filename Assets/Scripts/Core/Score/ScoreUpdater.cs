using System.Linq;
using Core.GameEvents;
using Core.UI.InGame;
using Core.UI.InGame.Score;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Score
{
        public class ScoreUpdater: NetworkBehaviour
        {
                [SerializeField] ScoreDisplay scoreDisplay;
                [SerializeField] VictoryBanner victoryBanner;
        
                private ScoreCalculator _scoreCalculator;

                public override void OnNetworkSpawn()
                {
                        if(!IsServer) 
                                return;
                        
                        ServerEvents.DayNightCycle.OnTurnEnded += DistributeScoresAtNight;
                        ServerEvents.DayNightCycle.OnGameEnded += DistributeScoresAtEndOfGame;
                }

                public override void OnNetworkDespawn()
                {
                        if(!IsServer)
                                return;
                        
                        ServerEvents.DayNightCycle.OnTurnEnded -= DistributeScoresAtNight;
                        ServerEvents.DayNightCycle.OnGameEnded -= DistributeScoresAtEndOfGame;
                }

                #region Server
                
                public void Initialize(ScoreCalculator scoreCalculator)
                {
                        _scoreCalculator = scoreCalculator;
                        InitializeVictoryPointDisplayClientRpc(GetPlayersInDisplayDataSortedByScore());
                }

                private void DistributeScoresAtNight()
                {
                        IncrementScoresAtNight();
                        
                        DistributeScoresAtNightClientRpc(GetPlayersInDisplayDataSortedByScore());
                }

                private void DistributeScoresAtEndOfGame()
                {
                        IncrementScoresAtNight();
                        var sortedDisplayData = GetPlayersInDisplayDataSortedByScore();
                        var winnerDisplayData = sortedDisplayData[0];
                        var winnerName = HostSingleton.Instance.GameManager
                                .GetPlayerByClientId(winnerDisplayData.ClientId).Name;
                        
                        DistributeScoresAtEndOfGameClientRpc(sortedDisplayData, winnerName);
                }

                private void IncrementScoresAtNight()
                {
                        var scoresByPlayerId = _scoreCalculator.CalculatePlayerScores();

                        foreach (var (id, scoreIncrement) in scoresByPlayerId)
                        {
                                var player = HostSingleton.Instance.GameManager.GetPlayerByClientId(id);
                                player.IncrementScore(scoreIncrement);
                        }    
                }

                private ScoreDisplay.PlayerScoreDisplayData[] GetPlayersInDisplayDataSortedByScore()
                {
                        var players = HostSingleton.Instance.GameManager.GetPlayers();
                        var displayData = players.Select(player => new ScoreDisplay.PlayerScoreDisplayData()
                        {
                                ClientId = player.ClientId,
                                Score = player.Score,
                                PlayerColorType = player.PlayerColorType,
                        });
                        return displayData.OrderByDescending(data => data.Score).ToArray();
                }

                #endregion

                #region Client

                [ClientRpc]
                private void InitializeVictoryPointDisplayClientRpc(ScoreDisplay.PlayerScoreDisplayData[] playerData)
                {
                        scoreDisplay.Initialize(playerData);
                }

                [ClientRpc]
                private void DistributeScoresAtNightClientRpc(ScoreDisplay.PlayerScoreDisplayData[] playerData)
                {
                        scoreDisplay.UpdateScoresFromData(playerData);
                }

                [ClientRpc]
                private void DistributeScoresAtEndOfGameClientRpc(ScoreDisplay.PlayerScoreDisplayData[] playerData, string winnerName)
                {
                        var winner = playerData[0];
                        scoreDisplay.UpdateScoresForGameEnd(playerData, () =>
                        {
                                var winnerIdentifier =
                                        winner.ClientId != NetworkManager.LocalClientId ? winnerName : "You";
                                victoryBanner.ShowFor(winnerIdentifier, winner.PlayerColorType); 
                        });
                }
                
                #endregion
        }
}