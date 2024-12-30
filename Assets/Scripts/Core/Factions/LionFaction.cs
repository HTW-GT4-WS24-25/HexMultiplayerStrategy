using Core.Unit.Group;
using UnityEngine;

namespace Core.Factions
{
    [CreateAssetMenu(fileName = "LionFaction", menuName = "Factions/Lions", order = 4)]
    public class LionFaction : Faction
    {
        [Header("Lion Settings")] 
        [SerializeField] private float damageReductionFactorOnBuilding;

        public override float CalculateDamageToReceive(Player player, float damageToBeDealt, UnitGroup ownUnitGroup, UnitGroup otherUnitGroup)
        {
            var damage = base.CalculateDamageToReceive(player, damageToBeDealt, ownUnitGroup, otherUnitGroup);
            var currentHex = player.GridData.GetCurrentHexFromUnitGroup(ownUnitGroup);
            var standsOnHexWithOwnBuilding = currentHex.ControllerPlayerId == player.ClientId && currentHex.Building != null;
            
            return standsOnHexWithOwnBuilding ? damage * damageReductionFactorOnBuilding : damage; 
        }
    }
}