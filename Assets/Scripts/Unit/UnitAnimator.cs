using DG.Tweening;
using Helper;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Unit
{
    [RequireComponent(typeof(Animator))]
    public class UnitAnimator : MonoBehaviour
    { 
        Animator _unitAnimator;

        private int _hitSpeedHash;
        private int _walkSpeedHash;
        private int _hitAnimationHash;
        private int _isWalkingHash;
        private int _deathHash;

        private Tween _moveTestAnimation;
        
        private void Awake()
        {
            _unitAnimator = GetComponent<Animator>();
            _hitSpeedHash = Animator.StringToHash("HitSpeed");
            _hitAnimationHash = Animator.StringToHash("Attack01");
            _isWalkingHash = Animator.StringToHash("IsWalking");
            _walkSpeedHash = Animator.StringToHash("WalkSpeed");
            _deathHash = Animator.StringToHash("Die");
        }

        [Button]
        public void PlayHitAnimation(float hitDuration)
        {
            var hitSpeedFactor = 1 / hitDuration;
            _unitAnimator.SetFloat(_hitSpeedHash, hitSpeedFactor);
            _unitAnimator.Play(_hitAnimationHash, -1, 0f);
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
                _unitAnimator.SetBool(_isWalkingHash, false);
                return;
            }
            
            var relativeMoveSpeed = HelperMethods.InverseLerpUnclamped(0.4f, 0.85f, moveSpeed);
            var walkAnimationSpeed = Mathf.LerpUnclamped(1.45f, 2.5f, relativeMoveSpeed);
            _unitAnimator.SetFloat(_walkSpeedHash, walkAnimationSpeed);
            _unitAnimator.SetBool(_isWalkingHash, true);
        }

        [Button]
        public void PlayDeathAnimation()
        {
            _unitAnimator.Play(_deathHash);
        }
    }
}
