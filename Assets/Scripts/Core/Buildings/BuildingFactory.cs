using System;
using Core.HexSystem.Hex;

namespace Core.Buildings
{
    public static class BuildingFactory
    {
        public static Building Create(BuildingType buildingType, HexagonData hexagon)
        {
            return buildingType switch
            {
                BuildingType.None => throw new ArgumentException("Invalid type."),
                BuildingType.Barrack => new BarrackBuilding(hexagon),
                BuildingType.Lumberjack => new LumberjackBuilding(hexagon),
                _ => throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null)
            };
        }
    }
}