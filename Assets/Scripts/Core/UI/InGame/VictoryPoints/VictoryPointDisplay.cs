using System.Collections.Generic;
using System.Linq;
using Core.Player;
using DG.Tweening;
using Networking.Host;
using UnityEngine;

namespace Core.UI.InGame.VictoryPoints
{
    public class VictoryPointDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VictoryPointFlag victoryPointFlagPrefab;
        [SerializeField] private Transform victoryPointFlagContainer;
        
        [Header("Settings")]
        [SerializeField] private float scoreAnimationTime = 2;
        [SerializeField] private float victoryFlagContainerGrowSize = 1.5f;
        [SerializeField] private float victoryFlagContainerGrowTime = 1f;
        [SerializeField] private float delayBetweenContainerAndScoreAnimations = 0.5f;
        
        private readonly Dictionary<ulong, VictoryPointFlag> _victoryPointFlagsByPlayerId = new();
        private readonly Dictionary<ulong, int> _lastScoresByPlayer = new();
        private Tween _scoreAnimation;

        public void Initialize(PlayerDataStorage.PlayerData[] playerData)
        {
            var firstPlacementValue = 1;
            foreach (var data in playerData)
            {
                var newFlag = Instantiate(victoryPointFlagPrefab, victoryPointFlagContainer);
                newFlag.SetColor(PlayerColor.GetFromColorType(data.PlayerColorType).BaseColor);
                newFlag.SetPlacement(firstPlacementValue++);
                _victoryPointFlagsByPlayerId.Add(data.ClientId, newFlag);
                _lastScoresByPlayer.Add(data.ClientId, data.PlayerScore);
            }
        }

        public void UpdateScoresFromData(PlayerDataStorage.PlayerData[] playerData)
        {
            var scoreAnimationSequence = CreateScoringSequence(playerData);
            
            scoreAnimationSequence.AppendInterval(delayBetweenContainerAndScoreAnimations);
            scoreAnimationSequence.Append(victoryPointFlagContainer.DOScale(1f, victoryFlagContainerGrowTime).SetEase(Ease.InCirc));
            _scoreAnimation = scoreAnimationSequence;
        }

        public void UpdateScoresForGameEnd(PlayerDataStorage.PlayerData[] playerData, TweenCallback onFinished)
        {
            var scoreAnimationSequence = CreateScoringSequence(playerData);
            scoreAnimationSequence.OnComplete(onFinished.Invoke);
            _scoreAnimation = scoreAnimationSequence;
        }

        private Sequence CreateScoringSequence(PlayerDataStorage.PlayerData[] playerData)
        {
            var scoringSequence = DOTween.Sequence();
            scoringSequence.Append(victoryPointFlagContainer.DOScale(victoryFlagContainerGrowSize, victoryFlagContainerGrowTime).SetEase(Ease.OutCirc));
            scoringSequence.AppendInterval(delayBetweenContainerAndScoreAnimations);
            foreach (var data in playerData)
            {
                var oldScore = _lastScoresByPlayer[data.ClientId];
                scoringSequence.Join(DOVirtual.Int(oldScore, data.PlayerScore, scoreAnimationTime,
                    value => _victoryPointFlagsByPlayerId[data.ClientId].SetScore(value)).SetEase(Ease.InOutSine));
                _lastScoresByPlayer[data.ClientId] = data.PlayerScore;
            }
            scoringSequence.AppendCallback(DeterminePlayerPlacement);

            return scoringSequence;
        }

        private void DeterminePlayerPlacement()
        {
            var sortedScoresByPlayerId = _lastScoresByPlayer.ToList();
            sortedScoresByPlayerId.Sort((x, y) => y.Value.CompareTo(x.Value));

            var placement = 1;
            foreach (var (playerId, _) in sortedScoresByPlayerId)
            {
                var flag = _victoryPointFlagsByPlayerId[playerId];
                flag.transform.SetSiblingIndex(placement - 1);
                flag.SetPlacement(placement++);
            }
        }
    }
}