using System.Collections.Generic;
using System.Linq;
using Core.Unit.Group;
using UnityEngine;

namespace Core.Factions
{
	[CreateAssetMenu(fileName = "LizardFaction", menuName = "Factions/Lizards", order = 3)]
    public class LizardFaction : Faction
    {
        [Header("Lizard Settings")] 
        [SerializeField] private float attackSpeedIncreaseFactorOnHit = 0.95f;
        [SerializeField] private float healPortionOfEnemyHit = 0.5f;

        public override float CalculateAttackSpeed(Player player, UnitGroup ownUnitGroup, UnitGroup[] otherUnitGroups)
        {
            var attackSpeed = base.CalculateAttackSpeed(player, ownUnitGroup, otherUnitGroups);

            // Todo: this assumes that CalculateAttackSpeed is always and only called at the start of an attack...
            
            var combatData = ReadHitDataOrCreateNew(player.FactionRuntimeData, ownUnitGroup, otherUnitGroups);
            if ((combatData.hitsDealt) % 3 == 0) // every third hit
            {
                ownUnitGroup.Heal(combatData.lastDamageTaken * healPortionOfEnemyHit);
            }
            
            var increasedAttackSpeed = attackSpeed * Mathf.Pow(attackSpeedIncreaseFactorOnHit, combatData.hitsDealt);
            combatData.hitsDealt++;
            player.FactionRuntimeData.Write(ownUnitGroup, combatData);
            return increasedAttackSpeed;
        }

        public override float CalculateDamageToReceive(Player player, float damageToBeDealt, UnitGroup ownUnitGroup, UnitGroup otherUnitGroup)
        {
            var runtimeData = player.FactionRuntimeData; 
            if (runtimeData.HasKey(ownUnitGroup)) // Todo: if there is no combat data (should not happen in current implementation) then the lastDamageTaken will not be saved
            {
                var oldCombatData = runtimeData.Read<PersistentCombatData>(ownUnitGroup);
                if (oldCombatData.enemies.Contains(otherUnitGroup))
                {
                    oldCombatData.lastDamageTaken = damageToBeDealt; // Todo: this assumes that there will be no further effects affecting damage after faction effects...
                    runtimeData.Write(ownUnitGroup, oldCombatData);
                }
            }
            
            return damageToBeDealt;
        }

        private PersistentCombatData ReadHitDataOrCreateNew(FactionRuntimeData runtimeData, UnitGroup ownUnitGroup, UnitGroup[] otherUnitGroups)
        {
            if (runtimeData.HasKey(ownUnitGroup))
            {
                var oldHitData = runtimeData.Read<PersistentCombatData>(ownUnitGroup);
                if(AreUnitGroupsEqual(oldHitData.enemies, otherUnitGroups))
                    return oldHitData;
            }
            
            return new PersistentCombatData { enemies = otherUnitGroups, hitsDealt = 0, lastDamageTaken = 0};
        }

        private bool AreUnitGroupsEqual(UnitGroup[] unitGroupsA, UnitGroup[] unitGroupsB)
        {
            if (unitGroupsA.Length != unitGroupsB.Length)
                return false;
            
            return unitGroupsA.All(unitGroup => unitGroupsB.Any(otherUnitGroup => otherUnitGroup.NetworkObjectId == unitGroup.NetworkObjectId));
        }

        public struct PersistentCombatData
        {
            public UnitGroup[] enemies;
            public int hitsDealt;
            public float lastDamageTaken;
        }
    }
}