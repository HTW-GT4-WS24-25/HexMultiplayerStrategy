using Core.PlayerData;
using DG.Tweening;
using UnityEngine;

namespace Core.Unit.Model
{
    public class UnitDeathDummy : MonoBehaviour
    {
        [SerializeField] private Transform unitHolder;
        [SerializeField] private float deathAnimationDuration;

        public void Initialize(PlayerColor playerColor, UnitModel.ModelType unitModelType)
        {
            var unitModelPrefab = UnitModel.GetModelPrefabFromType(unitModelType);
            var unitModel = Instantiate(unitModelPrefab, unitHolder.position, unitHolder.rotation, unitHolder);
            unitModel.MaskTint.ApplyColoringMaterial(playerColor.UnitColoringMaterial);
            unitModel.Animator.PlayDeathAnimation();
            
            DOVirtual.DelayedCall(deathAnimationDuration, () => { Destroy(gameObject); });
        }
    }
}