using Core.GameEvents;
using UnityEngine;

namespace Core.Unit.Group
{
    public class CombatInitiator : MonoBehaviour
    {
        [SerializeField] private UnitGroup unitGroup;
        
        public void OnTriggerEnter(Collider other)
        {
            if(!unitGroup.CanMove 
               || !other.CompareTag("UnitGroup")) 
                return;
            
            var collidedUnitGroup = other.GetComponentInParent<UnitGroup>();
            if (collidedUnitGroup.IsFighting || unitGroup.PlayerId == collidedUnitGroup.PlayerId)
                return;
            
            ServerEvents.Unit.OnCombatTriggered?.Invoke(collidedUnitGroup, unitGroup);
        }
    }
}