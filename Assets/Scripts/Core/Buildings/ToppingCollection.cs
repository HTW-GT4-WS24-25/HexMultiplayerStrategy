using System.Collections.Generic;
using Core.HexSystem.Hexagon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Buildings
{
    [CreateAssetMenu(fileName = "ToppingCollection", menuName = "ToppingCollection", order = 1)]
    public class ToppingCollection : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<HexType, Dictionary<BuildingType, Topping>> _toppingPrefabs = new();

        public Topping GetToppingPrefab(
            HexType hexType, 
            BuildingType buildingType = BuildingType.None)
        {
            return _toppingPrefabs[hexType][buildingType];
        }
    }
}