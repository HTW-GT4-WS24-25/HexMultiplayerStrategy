using GameEvents;
using Unity.Netcode;
using UnityEngine;

namespace Unit
{
    public class UnitGroupCombatInitiator : MonoBehaviour
    {
        [SerializeField] private UnitGroup unitGroup;
        
        public void OnTriggerEnter(Collider other)
        {
            if(!unitGroup.Movement.CanMove() 
               || !other.CompareTag("UnitGroup")) 
                return;
            
            var collidedUnitGroup = other.GetComponentInParent<UnitGroup>();
            if (collidedUnitGroup.Movement.IsFighting || unitGroup.PlayerId == collidedUnitGroup.PlayerId)
                return;
            
            ServerEvents.Unit.OnCombatTriggered?.Invoke(collidedUnitGroup, unitGroup);
        }
    }
}