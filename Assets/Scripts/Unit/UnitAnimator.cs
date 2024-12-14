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

        private int _hitSpeedHash;
        private int _walkSpeedHash;
        private int _idleAnimationHash;
        private int _walkAnimationHash;
        private int _hitAnimationHash;
        private int _deathAnimationHash;

        private Tween _moveTestAnimation;
        
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
            unitAnimator.SetFloat(_hitSpeedHash, hitSpeedFactor);
            unitAnimator.Play(_hitAnimationHash, -1, 0f);
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
                unitAnimator.CrossFade(_idleAnimationHash, 0.3f);
                return;
            }
            
            var relativeMoveSpeed = HelperMethods.InverseLerpUnclamped(0.4f, 0.85f, moveSpeed);
            var walkAnimationSpeed = Mathf.LerpUnclamped(1.45f, 2.5f, relativeMoveSpeed);
            unitAnimator.SetFloat(_walkSpeedHash, walkAnimationSpeed);
            unitAnimator.CrossFade(_walkAnimationHash, 0.3f);
        }

        [Button]
        public void PlayDeathAnimation()
        {
            unitAnimator.Play(_deathAnimationHash);
        }
    }
}
