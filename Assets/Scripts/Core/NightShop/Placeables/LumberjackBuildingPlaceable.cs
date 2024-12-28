using Core.Buildings;
using Core.HexSystem.Hexagon;
using UnityEngine;

namespace Core.NightShop.Placeables
{
    [CreateAssetMenu(fileName = "LumberjackBuildingPlaceable", menuName = "Placeables/Buildings/LumberjackBuildingPlaceable", order = 0)]
    public class LumberjackBuildingPlaceable : BuildingPlaceable
    {
        protected override HexType[] ValidHexTypes { get; } = { HexType.Forest };
        protected override BuildingType BuildingType => BuildingType.Lumberjack;
    }
}