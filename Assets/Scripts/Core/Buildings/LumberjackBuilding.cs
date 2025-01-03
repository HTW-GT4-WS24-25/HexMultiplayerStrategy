using Core.HexSystem.Hex;

namespace Core.Buildings
{
    public class LumberjackBuilding : Building
    {
        public override BuildingType Type => BuildingType.Lumberjack;
        public override int MaxLevel => 1;
        
        public LumberjackBuilding(HexagonData hexagon) : base(hexagon)
        { }

        public override BuildingYield OnNightFall()
        {
            return new BuildingYield(Type, 3, Level);
        }
    }
}