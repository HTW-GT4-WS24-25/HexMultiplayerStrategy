namespace Core.Buildings
{
    public class BuildingYield
    {
        public readonly BuildingType Type;
        public readonly int Score;
        public readonly int Gold;

        public BuildingYield(BuildingType type, int score, int gold)
        {
            Type = type;
            Score = score;
            Gold = gold;
        }
    }
}