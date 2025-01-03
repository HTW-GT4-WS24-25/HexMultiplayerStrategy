using System.Collections.Generic;
using System.Linq;
using Core.PlayerData;
using DG.Tweening;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.UI.InGame.Score
{
    public class ScoreDisplay : MonoBehaviour
    {
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
        private readonly Dictionary<ulong, Tween> _scoreAnimationsByPlayerId = new();

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
                _scoreAnimationsByPlayerId.Add(data.ClientId, null);
            }
        }

        public void UpdateScoreForPlayer(ulong playerId, int newScore)
        {
            _scoreAnimationsByPlayerId[playerId]?.Kill();
            
            var oldScore = _lastScoresByPlayer[playerId];
            _scoreAnimationsByPlayerId[playerId] = DOVirtual.Int(oldScore, newScore, scoreAnimationTime,
                value =>
                {
                    _lastScoresByPlayer[playerId] = value;
                    _scoreFlagsByPlayerId[playerId].SetScore(value);
                }).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        UpdatePlayerPlacement();
                        _scoreAnimationsByPlayerId[playerId] = null;
                    });
        }

        public void PlayGrowShrinkAnimation()
        {
            var growAnimation = GetGrowAnimation();
            growAnimation.AppendInterval(delayBetweenContainerAndScoreAnimations);
            growAnimation.Append(victoryPointFlagContainer
                .DOScale(1, scoreFlagContainerGrowTime)
                .SetEase(Ease.InCirc));
        }

        public void PlayGrowAnimation()
        {
            var growAnimation = GetGrowAnimation();
        }

        private Sequence GetGrowAnimation()
        {
            var growAnimation = DOTween.Sequence();
            growAnimation.Append(victoryPointFlagContainer.DOScale(scoreFlagContainerGrowSize, scoreFlagContainerGrowTime)
                .SetEase(Ease.InCirc));
            
            return growAnimation;
        }

        public void UpdateScoresFromData(PlayerScoreDisplayData[] playerData)
        {
            var scoreAnimationSequence = CreateScoringSequence(playerData);
            
            scoreAnimationSequence.AppendInterval(delayBetweenContainerAndScoreAnimations);
            //scoreAnimationSequence.Append();
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
            scoringSequence.AppendCallback(UpdatePlayerPlacement);

            return scoringSequence;
        }

        private void UpdatePlayerPlacement()
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