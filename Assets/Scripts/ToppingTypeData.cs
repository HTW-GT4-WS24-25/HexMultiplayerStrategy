using HexSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHexTypeData", menuName = "HexTypeData")]
public class ToppingTypeData : ScriptableObject
{
    [field: SerializeField] public ToppingType Type { get; private set; }
    [field: SerializeField] public bool IsTraversable { get; private set; }
    [field: SerializeField] public float MovementSpeedFactor { get; private set; }
    [field: SerializeField] public Topping ToppingPrefab { get; private set; }
}