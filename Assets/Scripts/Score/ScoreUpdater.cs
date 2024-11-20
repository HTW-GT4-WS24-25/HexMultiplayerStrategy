using Networking.Host;
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
                        GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += cycleState =>
                        {
                                if (cycleState == DayNightCycle.CycleState.Night) DistributePlayerScoresToAll();
                        };
                }

                #region Server

                private void DistributePlayerScoresToAll()
                {
                        var scoresByPlayerId = _scoreCalculator.CalculatePlayerScores();

                        foreach (var (id, score) in scoresByPlayerId)
                        {
                                HostSingleton.Instance.GameManager.PlayerData.IncrementPlayerScore(id, score);
                        } 
                
                        DistributePlayerScoresClientRpc();
                }

                #endregion

                #region Client

                [ClientRpc]
                private void DistributePlayerScoresClientRpc()
                {
                        //scoreWindow.UpdateScores();
                }

                #endregion
        }
}