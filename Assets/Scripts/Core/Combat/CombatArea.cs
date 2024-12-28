using Core.Unit.Group;
using UnityEngine;

namespace Core.Combat
{
    public class CombatArea : MonoBehaviour
    {
        private float _radius;
        private Combat _combat;

        public void Initialize(Combat combat, float radius)
        {
            _combat = combat;
            GetComponent<CapsuleCollider>().radius = radius;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("UnitGroup"))
                return;
            
            var collidedUnitGroup = other.GetComponentInParent<UnitGroup>();
            
            if (collidedUnitGroup.IsFighting)
                return;

            _combat.JoinCombat(collidedUnitGroup);
        }
    }
}