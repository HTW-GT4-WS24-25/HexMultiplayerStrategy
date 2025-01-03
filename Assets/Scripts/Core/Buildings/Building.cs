using Core.HexSystem.Hex;

namespace Core.Buildings
{
    public abstract class Building
    {
        public abstract BuildingType Type { get; }
        public abstract int MaxLevel { get; }
        
        public int Level { get; protected set; } = 1;
        public HexagonData Hexagon { get; private set; }
        
        public bool CanBeUpgraded => Level < MaxLevel;

        public Building(HexagonData hexagon)
        {
            Hexagon = hexagon;
        }

        public virtual void Upgrade()
        {
            Level++;
        }

        public virtual BuildingYield OnDawn()
        {
            return null;
        }

        public virtual BuildingYield OnNightFall()
        {
            return null;
        }
    }
}