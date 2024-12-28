using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace Core.Combat
{
    public class CombatIndicator : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform combatCircle;
        
        [Header("Settings")]
        [SerializeField] private float radius;
        [SerializeField] private float showTime;
        [SerializeField] private float hideTime;

        private Tween _animationTween;

        #region Server

        public void ShowForAll(float combatRadius)
        {
            ShowClientRpc(combatRadius);
        }

        public void HideForAll()
        {
            HideClientRpc();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void ShowClientRpc(float combatRadius)
        {
            radius = combatRadius;
            _animationTween?.Kill();
            combatCircle.localScale = new Vector3(0, combatCircle.localScale.y, 0);
            _animationTween = combatCircle
                .DOScale(new Vector3(radius, combatCircle.localScale.y, radius), showTime)
                .SetEase(Ease.OutBack);
        }

        [ClientRpc]
        private void HideClientRpc()
        {
            _animationTween?.Kill();
            combatCircle.localScale = new Vector3(radius * 0.5f, combatCircle.localScale.y, radius * 0.5f);
            _animationTween = combatCircle
                .DOScale(new Vector3(0, combatCircle.localScale.y, 0), hideTime)
                .SetEase(Ease.InBack);
        }

        #endregion
    }
}
