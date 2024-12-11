using Unit;
using UnityEngine;

namespace Combat
{
    public class AttackDamageCalculator : MonoBehaviour
    {
        private const float DefaultDamage = 0.2f;

        public static float Calculate(UnitGroup damageDealer, UnitGroup target)
        {
            var count = damageDealer.UnitCount;
            //Get HexData for Forestdefence by Target
            //Other Multipliers
            return DefaultDamage * count.Value;
        }
    }
}