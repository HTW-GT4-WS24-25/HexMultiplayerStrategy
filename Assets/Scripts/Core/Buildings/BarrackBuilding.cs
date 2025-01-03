using Core.GameEvents;
using Core.HexSystem.Hex;

namespace Core.Buildings
{
    public class BarrackBuilding : Building
    {
        public override BuildingType Type => BuildingType.Barrack;
        public override int MaxLevel => 3;
        
        public BarrackBuilding(HexagonData hexagon) : base(hexagon)
        { }   

        public override BuildingYield OnDawn()
        {
            ServerEvents.Unit.OnUnitsShouldBeSpawnedOnBarrackHex?.Invoke(Hexagon, Level);
            return null;
        }

        public override BuildingYield OnNightFall()
        {
            return new BuildingYield(Type, 3 + Level*2, 0);
        }
    }
}