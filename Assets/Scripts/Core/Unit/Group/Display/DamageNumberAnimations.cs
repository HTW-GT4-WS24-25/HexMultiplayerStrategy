using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Core.Unit.Group.Display
{
    public class DamageNumberAnimations : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI numberText;
        
        [Header("Settings")]
        [SerializeField] private float damageAnimationTime;
        [SerializeField] private Color damageColor;
        [SerializeField] private float damageSize;
        
        private Tween _currentAnimation;

        [Button]
        public void PlayDamageAnimation()
        {
            _currentAnimation?.Kill();
            
            numberText.transform.localScale = Vector3.one;

            var animationSequence = DOTween.Sequence();
            animationSequence.Append(numberText.transform.DOScale(damageSize, damageAnimationTime).From());
            animationSequence.Join(DOVirtual.Color(damageColor, Color.white, damageAnimationTime, color => numberText.color = color));

            _currentAnimation = animationSequence;
        }
    }
}
