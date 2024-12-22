using System;

namespace Buildings
{
    public static class BuildingFactory
    {
        public static Building Create(BuildingType buildingType)
        {
            return buildingType switch
            {
                BuildingType.None => throw new ArgumentException("Invalid type."),
                BuildingType.Barrack => new BarrackBuilding(),
                BuildingType.Lumberjack => new LumberjackBuilding(),
                _ => throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null)
            };
        }
    }
}