using DG.Tweening;
using Networking.Host;
using Player;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class VictoryBanner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image colorLine;
        [SerializeField] private TextMeshProUGUI victoryText;
        [SerializeField] private CanvasGroup alphaGroup;

        [Header("Settings")] 
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float fadeEndScale;
        
        [Button]
        public void ShowFor(PlayerDataStorage.PlayerData player)
        {
            var winnerIdentifier = player.ClientId == NetworkManager.Singleton.LocalClientId ? "You" : player.PlayerName;
            victoryText.text = $"{winnerIdentifier} won!";
            var playerColorValue = PlayerColor.GetFromColorType(player.PlayerColorType).BaseColor;
            
            gameObject.SetActive(true);
            var fadeInSequence = DOTween.Sequence();
            fadeInSequence.Append(DOVirtual.Float(0, 1f, fadeInDuration, a => alphaGroup.alpha = a));
            fadeInSequence.Join(DOVirtual.Color(Color.white, playerColorValue, fadeInDuration, color => colorLine.color = color));
            fadeInSequence.Join(transform.DOScale(1.5f, fadeInDuration));
            fadeInSequence.SetEase(Ease.OutSine);
        }
    }
}
