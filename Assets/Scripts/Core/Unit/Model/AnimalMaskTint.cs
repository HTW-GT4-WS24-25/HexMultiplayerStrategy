using UnityEngine;

namespace Core.Unit.Model
{
    public class AnimalMaskTint : MonoBehaviour
    {
        [SerializeField] private Renderer currentBody;
        [SerializeField] private Renderer currentWeapon;
        [SerializeField] private Renderer currentShield;

        public void ApplyMaterials(Material coloringMaterial)
        {
            currentBody.material = coloringMaterial;
            currentShield.material = coloringMaterial;
            currentWeapon.material = coloringMaterial;
        }
    }
}
