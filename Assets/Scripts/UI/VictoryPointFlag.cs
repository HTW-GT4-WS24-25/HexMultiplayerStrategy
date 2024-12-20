using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class VictoryPointFlag : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI placementText;
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [Header("Settings")]
        [SerializeField] private float scoreNumberPunchScaleStrength = 0.1f;
        [SerializeField] private float scoreNumberPunchScaleTime = 0.3f;

        private Tween _scoreAnimationTween;

        public void SetColor(Color color)
        {
            background.color = color;
        }

        public void SetPlacement(int placement)
        {
            placementText.text = $"{placement}.";
        }

        [Button]
        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
            _scoreAnimationTween?.Kill();
            scoreText.transform.localScale = Vector3.one;

            _scoreAnimationTween = scoreText.transform.DOPunchScale(Vector3.one * scoreNumberPunchScaleStrength,
                scoreNumberPunchScaleTime);
        }
    }
}