using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Buildings
{
    [CreateAssetMenu(fileName = "ToppingCollection", menuName = "ToppingCollection", order = 0)]
    public class ToppingCollection : ScriptableObject
    {
        [Header("GrassToppings")]
        [SerializeField] private GameObject _grassNone;
        [SerializeField] private GameObject _grassBarracks1;
        [SerializeField] private GameObject _grassBarracks2;
        [SerializeField] private GameObject _grassBarracks3;
        [Header("ForestToppings")] 
        [SerializeField] private GameObject _forestNone;
        [SerializeField] private GameObject _forestLumberjack1;
        [Header("MountainTopping")] 
        [SerializeField] private GameObject _mountainNone;
        
        private Dictionary<
            HexType, 
            Dictionary<
                BuildingType, 
                Dictionary<
                    int, 
                    GameObject>>> _toppingPrefabs = new();

        public void Initialize()
        {
            _toppingPrefabs = new Dictionary<HexType, Dictionary<BuildingType, Dictionary<int, GameObject>>>
            {
                {
                    HexType.Grass,
                    new Dictionary<BuildingType, Dictionary<int, GameObject>>
                    {
                        {
                            BuildingType.None, 
                            new Dictionary<int, GameObject>
                            {
                                { 1, _grassNone }
                            }
                        },
                        {
                            BuildingType.Barrack, 
                            new Dictionary<int, GameObject>
                            {
                                { 1, _grassBarracks1 },
                                { 2, _grassBarracks2 },
                                { 3, _grassBarracks3 }
                            }
                        }
                    }
                },
                {
                    HexType.Forest,
                    new Dictionary<BuildingType, Dictionary<int, GameObject>>
                    {
                        {
                            BuildingType.None, 
                            new Dictionary<int, GameObject>
                            {
                                { 1, _forestNone }
                            }
                        },
                        {
                            BuildingType.Lumberjack, 
                            new Dictionary<int, GameObject>
                            {
                                { 1, _forestLumberjack1 }
                            }
                        }
                    }
                },
                {
                    HexType.Mountain,
                    new Dictionary<BuildingType, Dictionary<int, GameObject>>
                    {
                        {
                            BuildingType.None, 
                            new Dictionary<int, GameObject>
                            {
                                { 1, _mountainNone }
                            }
                        }
                    }
                }
            };
            
            Debug.Log("ToppingCollection awaken!!");
        }

        public GameObject GetToppingPrefab(
            HexType hexType, 
            BuildingType buildingType = BuildingType.None, 
            int buildingLevel = 1)
        {
            return _toppingPrefabs[hexType][buildingType][buildingLevel];
        }
    }
}