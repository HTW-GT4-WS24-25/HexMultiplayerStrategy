using System.Collections.Generic;
using System.Linq;
using Core.PlayerData;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.UI.InGame.Score
{
    public class ScoreDisplay : MonoBehaviour
    {
        [FormerlySerializedAs("victoryPointFlagPrefab")]
        [Header("References")]
        [SerializeField] private ScoreFlag scoreFlagPrefab;
        [SerializeField] private Transform victoryPointFlagContainer;
        
        [Header("Settings")]
        [SerializeField] private float scoreAnimationTime = 2;
        [SerializeField] private float scoreFlagContainerGrowSize = 1.5f;
        [SerializeField] private float scoreFlagContainerGrowTime = 1f;
        [SerializeField] private float delayBetweenContainerAndScoreAnimations = 0.5f;
        
        private readonly Dictionary<ulong, ScoreFlag> _scoreFlagsByPlayerId = new();
        private readonly Dictionary<ulong, int> _lastScoresByPlayer = new();

        public void Initialize(PlayerScoreDisplayData[] playerData)
        {
            var firstPlacementValue = 1;
            foreach (var data in playerData)
            {
                var newFlag = Instantiate(scoreFlagPrefab, victoryPointFlagContainer);
                newFlag.SetColor(PlayerColor.GetFromColorType(data.PlayerColorType).BaseColor);
                newFlag.SetPlacement(firstPlacementValue++);
                _scoreFlagsByPlayerId.Add(data.ClientId, newFlag);
                _lastScoresByPlayer.Add(data.ClientId, data.Score);
            }
        }

        public void UpdateScoresFromData(PlayerScoreDisplayData[] playerData)
        {
            var scoreAnimationSequence = CreateScoringSequence(playerData);
            
            scoreAnimationSequence.AppendInterval(delayBetweenContainerAndScoreAnimations);
            scoreAnimationSequence.Append(victoryPointFlagContainer.DOScale(1f, scoreFlagContainerGrowTime).SetEase(Ease.InCirc));
        }

        public void UpdateScoresForGameEnd(PlayerScoreDisplayData[] playerData, TweenCallback onFinished)
        {
            var scoreAnimationSequence = CreateScoringSequence(playerData);
            scoreAnimationSequence.OnComplete(onFinished.Invoke);
        }

        private Sequence CreateScoringSequence(PlayerScoreDisplayData[] playerData)
        {
            var scoringSequence = DOTween.Sequence();
            scoringSequence.Append(victoryPointFlagContainer.DOScale(scoreFlagContainerGrowSize, scoreFlagContainerGrowTime).SetEase(Ease.OutCirc));
            scoringSequence.AppendInterval(delayBetweenContainerAndScoreAnimations);
            foreach (var data in playerData)
            {
                var oldScore = _lastScoresByPlayer[data.ClientId];
                scoringSequence.Join(DOVirtual.Int(oldScore, data.Score, scoreAnimationTime,
                    value => _scoreFlagsByPlayerId[data.ClientId].SetScore(value)).SetEase(Ease.InOutSine));
                _lastScoresByPlayer[data.ClientId] = data.Score;
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
                var flag = _scoreFlagsByPlayerId[playerId];
                flag.transform.SetSiblingIndex(placement - 1);
                flag.SetPlacement(placement++);
            }
        }
        
        public struct PlayerScoreDisplayData : INetworkSerializable
        {
            public ulong ClientId;
            public int Score;
            public PlayerColor.ColorType PlayerColorType;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref Score);
                serializer.SerializeValue(ref PlayerColorType);
            }
        }
    }
}