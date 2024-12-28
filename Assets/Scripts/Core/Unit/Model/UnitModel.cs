using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Unit.Model
{
    public class UnitModel : MonoBehaviour
    {
        [field: SerializeField] public ModelType Type { get; private set; }
        [field: SerializeField] public UnitAnimator Animator { get; private set; }
        [field: SerializeField] public AnimalMaskTint MaskTint { get; private set; }

        [SerializeField] private AnimalOutline outline;
    
        public static void AddModelPrefabToStorage(UnitModel unitModelPrefab) => _unitModelPrefabStorage.Add(unitModelPrefab.Type, unitModelPrefab);
        public static UnitModel GetModelPrefabFromType(ModelType type) => _unitModelPrefabStorage[type];
    
        private static Dictionary<ModelType, UnitModel> _unitModelPrefabStorage = new();

        public void ActivateSelectedOutline() => outline.ActivateSelectedOutline();
        public void ActivateHoverOutline() => outline.ActivateHoverOutline();
        public void DeactivateHoverOutline() => outline.DeactivateOverOutline();
        public void DeactivateSelectedOutline() => outline.DeactivateSelectedOutline();
    
        public enum ModelType
        {
            Rabbit = 0,
            Lion = 1,
            Lizard = 2,
            Owl = 3
        }
    }
}