﻿using UnityEngine;

namespace Core.HexSystem.Hex
{
    [CreateAssetMenu(fileName = "NewHexTypeData", menuName = "HexTypeData")]
    public class HexTypeData : ScriptableObject
    {
        [field: SerializeField] public HexType Type { get; private set; }
        [field: SerializeField] public bool IsTraversable { get; private set; }
        [field: SerializeField] public float MovementSpeedFactor { get; private set; }
        [field: SerializeField] public Hexagon HexagonPrefab { get; private set; }
        [field: SerializeField] public Topping DefaultToppingPrefab { get; private set; }
    }
}