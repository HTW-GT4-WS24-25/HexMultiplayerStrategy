namespace Core.Buildings
{
    public class BarrackBuilding : Building
    {
        public override BuildingType Type => BuildingType.Barrack;
        public override int MaxLevel => 3;
    }
}