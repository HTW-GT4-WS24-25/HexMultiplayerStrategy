using DG.Tweening;
using Helper;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit
{
    public class UnitAnimator : MonoBehaviour
    { 
        [SerializeField] private Animator unitAnimator;
        [SerializeField] private float hitAnimationPercentageUntilHit;

        private const float CrossFadeDuration = 0.3f;
        private const float LowRelativeMoveSpeed = 0.4f;
        private const float HighRelativeMoveSpeed = 0.85f;
        private const float LowRelativeWalkAnimationSpeed = 1.45f;
        private const float HighRelativeWalkAnimationSpeed = 2.5f;

        private int _hitSpeedHash;
        private int _walkSpeedHash;
        private int _idleAnimationHash;
        private int _walkAnimationHash;
        private int _hitAnimationHash;
        private int _deathAnimationHash;

        private Tween _moveTestAnimation;
        private Tween _hitDelayTween;
        
        private void Awake()
        {
            _hitSpeedHash = Animator.StringToHash("HitSpeed");
            _walkSpeedHash = Animator.StringToHash("WalkSpeed");
            _idleAnimationHash = Animator.StringToHash("Idle");
            _walkAnimationHash = Animator.StringToHash("Walk");
            _hitAnimationHash = Animator.StringToHash("Attack");
            _deathAnimationHash = Animator.StringToHash("Die");
        }

        [Button]
        public void PlayHitAnimation(float hitDuration)
        {
            var hitSpeedFactor = 1 / hitDuration;
            var waitTimeBeforeAnimation = (1 - hitAnimationPercentageUntilHit) * hitDuration;
            
            _hitDelayTween = DOVirtual.DelayedCall(waitTimeBeforeAnimation, () =>
            {
                unitAnimator.SetFloat(_hitSpeedHash, hitSpeedFactor);    
                unitAnimator.Play(_hitAnimationHash, -1, 0f);
            });
        }

        public void StopFightAnimations()
        {
            _hitDelayTween?.Kill();
            unitAnimator.CrossFade(_idleAnimationHash, CrossFadeDuration);
        }

        [Button]
        private void MakeTestWalk(float moveSpeed)
        {
            _moveTestAnimation?.Kill();
            
            var moveTime = 7 / moveSpeed;
            transform.position = new Vector3(0, 0, -3.5f);
            _moveTestAnimation = transform.DOMove(new Vector3(0, 0, 3.5f), moveTime);
        }
        
        [Button]
        public void SetMoveSpeed(float moveSpeed)
        {
            if (Equals(moveSpeed, 0f))
            {
                unitAnimator.CrossFade(_idleAnimationHash, CrossFadeDuration);
                return;
            }
            
            var relativeMoveSpeed = HelperMethods.InverseLerpUnclamped(LowRelativeMoveSpeed, HighRelativeMoveSpeed, moveSpeed);
            var walkAnimationSpeed = Mathf.LerpUnclamped(LowRelativeWalkAnimationSpeed, HighRelativeWalkAnimationSpeed, relativeMoveSpeed);
            unitAnimator.SetFloat(_walkSpeedHash, walkAnimationSpeed);
            unitAnimator.CrossFade(_walkAnimationHash, CrossFadeDuration);
        }

        [Button]
        public void PlayDeathAnimation()
        {
            unitAnimator.Play(_deathAnimationHash);
        }
    }
}
