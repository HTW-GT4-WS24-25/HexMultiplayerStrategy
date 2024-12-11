using Unit;
using UnityEngine;

namespace Combat
{
    public class AttackSpeedCalculator : MonoBehaviour
    {
        private const float DefaultAttackSpeed = 1.5f;

        public static float Calculate(UnitGroup unitGroup)
        {
            return DefaultAttackSpeed;
        }
    }
}