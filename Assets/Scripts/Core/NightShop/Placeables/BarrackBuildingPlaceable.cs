using Core.Buildings;
using Core.HexSystem.Hex;
using UnityEngine;

namespace Core.NightShop.Placeables
{
    [CreateAssetMenu(fileName = "BarrackBuildingPlaceable", menuName = "Placeables/Buildings/BarrackBuildingPlaceable", order = 0)]
    public class BarrackBuildingPlaceable : BuildingPlaceable
    {
        protected override HexType[] ValidHexTypes { get; } = { HexType.Grass };
        protected override BuildingType BuildingType => BuildingType.Barrack;
    }
}