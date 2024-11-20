using System.Linq;
using Networking.Host;
using NUnit.Framework;
using UI;
using Unity.Netcode;
using UnityEngine;

namespace Score
{
        public class ScoreUpdater: NetworkBehaviour
        {
                [SerializeField] ScoreWindow scoreWindow;
        
                private ScoreCalculator _scoreCalculator;

                public void Initialize(ScoreCalculator scoreCalculator)
                {
                        _scoreCalculator = scoreCalculator;
                }

                public override void OnNetworkSpawn()
                {
                        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += HandleDayNightCycleSwitched;
                }

                public override void OnNetworkDespawn()
                {
                        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState -= HandleDayNightCycleSwitched;
                }

                #region Server

                private void HandleDayNightCycleSwitched(DayNightCycle.CycleState newState)
                {
                        if (newState == DayNightCycle.CycleState.Night)
                                DistributePlayerScoresToAll();
                }

                private void DistributePlayerScoresToAll()
                {
                        var scoresByPlayerId = _scoreCalculator.CalculatePlayerScores();

                        foreach (var (id, score) in scoresByPlayerId)
                        {
                                HostSingleton.Instance.GameManager.PlayerData.IncrementPlayerScore(id, score);
                        }

                        var playerScoreList = HostSingleton.Instance.GameManager.PlayerData.GetAllPlayerData();
                        DistributePlayerScoresClientRpc(playerScoreList.OrderByDescending(data => data.PlayerScore).ToArray());
                }

                #endregion

                #region Client

                [ClientRpc]
                private void DistributePlayerScoresClientRpc(PlayerDataStorage.PlayerData[] playerData)
                {
                        scoreWindow.UpdateScores(playerData);
                        scoreWindow.gameObject.SetActive(true);
                }

                #endregion
        }
}