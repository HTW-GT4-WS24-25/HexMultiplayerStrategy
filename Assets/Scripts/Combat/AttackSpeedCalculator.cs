using Unit;
using UnityEngine;

namespace Combat
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