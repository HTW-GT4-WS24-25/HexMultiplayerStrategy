using Unity.Netcode;
using UnityEngine;

namespace Unit.Model
{
    public class AnimalMaskTint : MonoBehaviour
    {
        [SerializeField] private Renderer currentBody;
        [SerializeField] private Renderer currentWeapon;
        [SerializeField] private Renderer currentShield;

        public void ApplyMaterials(Material playerMaterial)
        {
            currentBody.material = playerMaterial;
            currentShield.material = playerMaterial;
            currentWeapon.material = playerMaterial;
        }
    }
}
