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