using DG.Tweening;
using Helper;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI.Lobby
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class JoinCodeText : MonoBehaviour, IPointerDownHandler
    {
        private const string JoinCodePrefix = "Code: ";
        
        private TextMeshProUGUI _textMesh;
        private string _joinCode = "";
        private Tween _copyAnimationTween;
        
        private void Awake()
        {
            _textMesh = GetComponent<TextMeshProUGUI>();
        }

        public void SetJoinCode(string joinCode)
        {
            _joinCode = joinCode;
            _textMesh.text = JoinCodePrefix + joinCode;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HelperMethods.CopyToClipboard(_joinCode);
            PlayCopyAnimation();
        }

        private void PlayCopyAnimation()
        {
            _copyAnimationTween?.Kill();
            _copyAnimationTween = DOVirtual.Color(Color.black, Color.white, 0.7f, color => _textMesh.color = color).SetEase(Ease.OutCirc);
        }
    }
}