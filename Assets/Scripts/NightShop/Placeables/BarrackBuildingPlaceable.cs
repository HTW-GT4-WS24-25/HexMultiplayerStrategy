using Buildings;
using GameEvents;
using HexSystem;
using UnityEngine;

namespace NightShop.Placeables
{
    [CreateAssetMenu(fileName = "BarrackBuildingPlaceable", menuName = "Placeables/Buildings/BarrackBuildingPlaceable", order = 0)]
    public class BarrackBuildingPlaceable : BuildingPlaceable
    {
        protected override HexType[] ValidHexTypes { get; } = { HexType.Grass };
        protected override BuildingType BuildingType => BuildingType.Barrack;
    }
}