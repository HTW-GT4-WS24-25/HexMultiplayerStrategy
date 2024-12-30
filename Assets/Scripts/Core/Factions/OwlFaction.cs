using Core.Unit.Group;
using UnityEngine;

namespace Core.Factions
{
    [CreateAssetMenu(fileName = "OwlFaction", menuName = "Factions/Owls", order = 2)]
    public class OwlFaction : Faction
    {
        public override float CalculateMoveSpeed(Player player, UnitGroup unitGroup)
        {
            return baseMoveSpeed;
        }
    }
}