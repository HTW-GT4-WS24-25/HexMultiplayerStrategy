using System.Collections.Generic;
using Core.Factions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Unit.Model
{
    public class UnitModel : MonoBehaviour
    {
        [field: SerializeField] public FactionType Type { get; private set; }
        [field: SerializeField] public UnitAnimator Animator { get; private set; }
        [field: SerializeField] public AnimalMaskTint MaskTint { get; private set; }

        [SerializeField] private AnimalOutline outline;
    
        public static void AddModelPrefabToStorage(UnitModel unitModelPrefab) => _unitModelPrefabStorage.Add(unitModelPrefab.Type, unitModelPrefab);
        public static UnitModel GetModelPrefabFromFactionType(FactionType type) => _unitModelPrefabStorage[type];
    
        private static Dictionary<FactionType, UnitModel> _unitModelPrefabStorage = new();

        public void ActivateSelectedOutline() => outline.ActivateSelectedOutline();
        public void ActivateHoverOutline() => outline.ActivateHoverOutline();
        public void DeactivateHoverOutline() => outline.DeactivateOverOutline();
        public void DeactivateSelectedOutline() => outline.DeactivateSelectedOutline();
    }
}