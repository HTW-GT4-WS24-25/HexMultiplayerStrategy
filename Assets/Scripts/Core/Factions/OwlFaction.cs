using Core.HexSystem.Hex;
using Core.Unit.Group;
using UnityEngine;

namespace Core.Factions
{
    [CreateAssetMenu(fileName = "OwlFaction", menuName = "Factions/Owls", order = 2)]
    public class OwlFaction : Faction
    {
        [Header("Owl Settings")] 
        [SerializeField] private float forestAttackSpeedFactor;
        
        public override float CalculateAttackSpeed(Player player, UnitGroup ownUnitGroup, UnitGroup[] otherUnitGroups)
        {
            var attackSpeed =  base.CalculateAttackSpeed(player, ownUnitGroup, otherUnitGroups);
            var currentHex = player.GridData.GetCurrentHexFromUnitGroup(ownUnitGroup);

            return currentHex.HexType == HexType.Forest ? attackSpeed * forestAttackSpeedFactor : attackSpeed;
        }

        public override float CalculateMoveSpeed(Player player, UnitGroup unitGroup)
        {
            return baseMoveSpeed;
        }
    }
}