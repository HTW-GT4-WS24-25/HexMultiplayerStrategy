using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Buildings
{
    [CreateAssetMenu(fileName = "ToppingCollection", menuName = "ToppingCollection", order = 0)]
    public class ToppingCollection : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<
            HexType, 
            Dictionary<
                BuildingType, 
                Dictionary<
                    int, 
                    GameObject>>> _toppingPrefabs = new();

        public GameObject GetToppingPrefab(
            HexType hexType, 
            BuildingType buildingType = BuildingType.None, 
            int buildingLevel = 1)
        {
            return _toppingPrefabs[hexType][buildingType][buildingLevel];
        }
    }
}