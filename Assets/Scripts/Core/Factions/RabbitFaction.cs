using System.Linq;
using Core.HexSystem.Hex;
using Core.Unit.Group;
using Networking.Host;
using UnityEngine;

namespace Core.Factions
{
    [CreateAssetMenu(fileName = "RabbitFaction", menuName = "Factions/Rabbits", order = 1)]
    public class RabbitFaction : Faction
    {
        [Header("Rabbit Settings")] 
        [SerializeField] private float grassMoveSpeedFactor;
        [SerializeField] private float forestMoveSpeedFactor;
        [SerializeField] private float attackSpeedFactorAgainstHigherScore;
        
        public override float CalculateMoveSpeed(Player player, UnitGroup unitGroup)
        {
            var speed = base.CalculateMoveSpeed(player, unitGroup);
            var currentHex = player.GridData.GetCurrentHexFromUnitGroup(unitGroup);

            if(currentHex.HexType == HexType.Grass)
                speed *= grassMoveSpeedFactor;
            else if(currentHex.HexType == HexType.Forest)
                speed *= forestMoveSpeedFactor;
            
            return speed;
        }

        public override float CalculateAttackSpeed(Player player, UnitGroup ownUnitGroup, UnitGroup[] otherUnitGroups)
        {
            var attackSpeed = base.CalculateAttackSpeed(player, ownUnitGroup, otherUnitGroups);

            var enemyHasHigherScore = otherUnitGroups.Any(other =>
            {
                var enemyPlayer = HostSingleton.Instance.GameManager.GetPlayerByClientId(other.PlayerId);
                return enemyPlayer.Score > player.Score;
            });

            return enemyHasHigherScore ? attackSpeed * attackSpeedFactorAgainstHigherScore : attackSpeed;
        }
    }
}