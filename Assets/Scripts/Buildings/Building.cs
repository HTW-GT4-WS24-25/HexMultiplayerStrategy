namespace Buildings
{
    public abstract class Building
    {
        public abstract BuildingType Type { get; }
        public abstract int MaxLevel { get; }
        
        public int Level { get; protected set; } = 1;
        
        public bool CanBeUpgraded => Level < MaxLevel;

        public virtual void Upgrade()
        {
            Level++;
        }
        
        
    }
}