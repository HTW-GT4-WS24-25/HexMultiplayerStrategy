using Core.Unit.Group;

namespace Core.Combat
{
    public static class AttackSpeedCalculator
    {
        private const float DefaultAttackSpeed = 1.5f;

        public static float Calculate(UnitGroup unitGroup)
        {
            return DefaultAttackSpeed;
        }
    }
}