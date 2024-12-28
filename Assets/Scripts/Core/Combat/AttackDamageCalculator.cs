using Core.Unit.Group;
using UnityEngine;

namespace Core.Combat
{
    public static class AttackDamageCalculator
    {
        private static AnimationCurve _damageCurve;

        public static float Calculate(UnitGroup damageDealer, UnitGroup target)
        {
            if(_damageCurve == null)
                LoadDamageCurve();
            
            var count = damageDealer.UnitCount.Value;
            //Get HexData for Forestdefence by Target
            //Other Multipliers
            return _damageCurve!.Evaluate(count);
        }

        private static void LoadDamageCurve()
        {
            var damageCurves = Resources.Load<DamageCurves>("DamageCurves");
            _damageCurve = damageCurves.Default;
        }
    }
}