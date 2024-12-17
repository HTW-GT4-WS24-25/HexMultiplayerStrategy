using DG.Tweening;
using Player;
using Unit.Model;
using UnityEngine;

namespace Unit
{
    public class UnitDeathDummy : MonoBehaviour
    {
        [SerializeField] UnitAnimator unitAnimator;
        [SerializeField] AnimalMaskTint animalMaskTint;
        [SerializeField] private float deathAnimationDuration;

        public void Initialize(PlayerColor playerColor)
        {
            animalMaskTint.ApplyMaterials(playerColor.UnitColoringMaterial);
            unitAnimator.PlayDeathAnimation();
            DOVirtual.DelayedCall(deathAnimationDuration, () => { Destroy(gameObject); });
        }
    }
}