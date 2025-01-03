using Core.Combat;
using Core.HexSystem;
using Core.HexSystem.Hex;
using Core.Unit.Group;
using Networking.Host;
using UnityEngine;

namespace Core.Factions
{
    public abstract class Faction : ScriptableObject 
    {
        [field: SerializeField] public FactionType Type { get; private set; }
        [field: SerializeField] public int StartUnitCount { get; private set; } = 20;

        [TextArea]
        [SerializeField] protected string description;
        [SerializeField] protected float baseAttackSpeed = 1.5f;
        [SerializeField] protected float baseMoveSpeed = 0.15f;
        
        public virtual int CalculateScoreToGainAtNightfall(Player player)
        {
            return player.NumberOfControlledHexes;
        }

        public virtual int CalculateGoldToGainAtNightfall(Player player)
        {
            return HostSingleton.Instance.GameManager.MatchConfig.baseIncomePerNight;
        }

        public virtual float CalculateDamageToDeal(Player player, UnitGroup ownUnitGroup, UnitGroup otherUnitGroup)
        {
            return AttackDamageCalculator.Calculate(ownUnitGroup, otherUnitGroup);
        }

        public virtual float CalculateDamageToReceive(Player player, float damageToBeDealt, UnitGroup ownUnitGroup, UnitGroup otherUnitGroup)
        {
            return damageToBeDealt;
        }

        public virtual float CalculateAttackSpeed(Player player, UnitGroup ownUnitGroup, UnitGroup[] otherUnitGroups)
        {
            return baseAttackSpeed;
        }

        public virtual float CalculateMoveSpeed(Player player, UnitGroup unitGroup)
        {
            var hexagonData = player.GridData.GetCurrentHexFromUnitGroup(unitGroup);
            var hexTypeData = HexTypeDataProvider.Instance.GetData(hexagonData.HexType);
        
            return baseMoveSpeed * hexTypeData.MovementSpeedFactor;
        }
    }
}