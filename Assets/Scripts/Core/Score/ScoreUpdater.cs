using System.Linq;
using Core.GameEvents;
using Core.PlayerData;
using Core.UI.InGame;
using Core.UI.InGame.Score;
using DG.Tweening;
using Networking.Host;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Score
{
        public class ScoreUpdater: NetworkBehaviour
        {
                [SerializeField] ScoreDisplay scoreDisplay;
                [SerializeField] VictoryBanner victoryBanner;
                [SerializeField] private float delayTimeToShowVictoryBanner = 2f;
        
                private ScoreCalculator _scoreCalculator;

                public override void OnNetworkSpawn()
                {
                        if(!IsServer) 
                                return;
                        
                        ServerEvents.DayNightCycle.OnTurnEnded += DistributeScoresAtNight;
                        ServerEvents.DayNightCycle.OnGameEnded += DistributeScoresAtEndOfGame;
                        ServerEvents.Player.OnPlayerScoreChanged += HandlePlayerScoreChanged;
                }

                public override void OnNetworkDespawn()
                {
                        if(!IsServer)
                                return;
                        
                        ServerEvents.DayNightCycle.OnTurnEnded -= DistributeScoresAtNight;
                        ServerEvents.DayNightCycle.OnGameEnded -= DistributeScoresAtEndOfGame;
                        ServerEvents.Player.OnPlayerScoreChanged -= HandlePlayerScoreChanged;
                }

                #region Server
                
                public void Initialize(ScoreCalculator scoreCalculator)
                {
                        _scoreCalculator = scoreCalculator;
                        InitializeVictoryPointDisplayClientRpc(GetPlayersInDisplayDataSortedByScore());
                }
                
                private void HandlePlayerScoreChanged(ulong playerId, int newScore)
                {
                        UpdatePlayerScoreDisplayClientRpc(playerId, newScore);
                }

                private void DistributeScoresAtNight()
                {
                        //IncrementScoresAtNight();
                        
                        DistributeScoresAtNightClientRpc(GetPlayersInDisplayDataSortedByScore());
                }

                private void DistributeScoresAtEndOfGame()
                {
                        //IncrementScoresAtNight();
                        var sortedDisplayData = GetPlayersInDisplayDataSortedByScore();
                        var winnerDisplayData = sortedDisplayData[0];
                        var winnerName = HostSingleton.Instance.GameManager.GetPlayerByClientId(winnerDisplayData.ClientId).Name;
                        
                        DistributeScoresAtEndOfGameClientRpc(winnerDisplayData.ClientId, winnerName, winnerDisplayData.PlayerColorType);
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
                private void UpdatePlayerScoreDisplayClientRpc(ulong playerId, int newScore)
                {
                        scoreDisplay.UpdateScoreForPlayer(playerId, newScore);
                }

                [ClientRpc]
                private void DistributeScoresAtNightClientRpc(ScoreDisplay.PlayerScoreDisplayData[] playerData)
                {
                        //scoreDisplay.UpdateScoresFromData(playerData);
                        scoreDisplay.PlayGrowShrinkAnimation();
                }

                [ClientRpc]
                private void DistributeScoresAtEndOfGameClientRpc(ulong winnerClientId, FixedString32Bytes winnerName, PlayerColor.ColorType winnerColorType)
                {
                        scoreDisplay.PlayGrowAnimation();
                        DOVirtual.DelayedCall(delayTimeToShowVictoryBanner, () =>
                        {
                                var winnerIdentifier =
                                        winnerClientId != NetworkManager.LocalClientId ? winnerName.Value : "You";
                                victoryBanner.ShowFor(winnerIdentifier, winnerColorType);
                        });
                }
                
                #endregion
        }
}