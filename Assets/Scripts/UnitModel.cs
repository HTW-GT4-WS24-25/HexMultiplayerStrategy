using System.Collections.Generic;
using Unit;
using Unit.Model;
using UnityEngine;

public class UnitModel : MonoBehaviour
{
    [field: SerializeField] public ModelType Type { get; private set; }
    [field: SerializeField] public UnitAnimator Animator { get; private set; }
    [field: SerializeField] public AnimalMaskTint MaskTint { get; private set; }
    
    public static void AddModelPrefabToStorage(UnitModel unitModelPrefab) => _unitModelPrefabStorage.Add(unitModelPrefab.Type, unitModelPrefab);
    public static UnitModel GetModelPrefabFromType(ModelType type) => _unitModelPrefabStorage[type];
    
    private static Dictionary<ModelType, UnitModel> _unitModelPrefabStorage = new();
    
    public enum ModelType
    {
        Rabbit = 0,
        Lion = 1,
        Lizard = 2,
        Owl = 3
    }
}