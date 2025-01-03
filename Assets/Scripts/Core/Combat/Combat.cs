using System.Collections.Generic;
using System.Linq;
using Core.Unit.Group;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Combat
{
    public class Combat
    {
        public readonly CombatIndicator CombatIndicator;
        public readonly CombatArea CombatArea;

        public event UnityAction<Combat> OnCombatEnd;
        
        private readonly HashSet<UnitGroup> _unitGroups = new();
        private readonly Dictionary<UnitGroup, float> _attackSpeeds = new();
        private readonly Dictionary<UnitGroup, float> _attackChargeProgress = new();
        private readonly Dictionary<UnitGroup, float> _damageToDealToUnitGroup = new();

        public Combat(CombatIndicator combatIndicator, CombatArea combatArea, UnitGroup unitGroup1, UnitGroup unitGroup2)
        {
            CombatIndicator = combatIndicator;
            CombatArea = combatArea;

            AddNewUnitGroupToCombat(unitGroup1);
            AddNewUnitGroupToCombat(unitGroup2);
        }
        
        public void JoinCombat(UnitGroup unitGroup)
        {
            Debug.Assert(!_unitGroups.Contains(unitGroup), "Tried to join a combat group but it was already joined.");
            
            var existingUnitGroupOfPlayer = _unitGroups.FirstOrDefault(u => u.PlayerId.Equals(unitGroup.PlayerId));
            if (existingUnitGroupOfPlayer != null)
                existingUnitGroupOfPlayer.IntegrateUnitsOf(unitGroup);
            else
                AddNewUnitGroupToCombat(unitGroup);
        }

        public void ProcessCombatFrame()
        {
            foreach (var unitGroup in _unitGroups)
                _damageToDealToUnitGroup[unitGroup] = 0;
            
            var hasDamageToDeal = false;
            foreach (var unitGroup in _unitGroups)
            {
                _attackChargeProgress[unitGroup] += Time.deltaTime / _attackSpeeds[unitGroup];
                if (_attackChargeProgress[unitGroup] < 1) 
                    continue;
                
                unitGroup.PlayHitAnimationInSeconds(_attackSpeeds[unitGroup]);
                hasDamageToDeal = true;
                CalculateDamageToOtherUnits(unitGroup);
                _attackChargeProgress[unitGroup] -= 1;
            }
            
            if (hasDamageToDeal)
                DealDamageToUnitGroups();
            
            if (_unitGroups.Count <= 1)
                End();
        }
        
        private void AddNewUnitGroupToCombat(UnitGroup unitGroup)
        {
            unitGroup.StartFighting(CombatIndicator);
            
            _unitGroups.Add(unitGroup);
            _attackChargeProgress.Add(unitGroup, 0);
            _attackSpeeds.Add(unitGroup, AttackSpeedCalculator.Calculate(unitGroup));
            _damageToDealToUnitGroup.Add(unitGroup, 0);
            
            unitGroup.PlayHitAnimationInSeconds(_attackSpeeds[unitGroup]);
        }
        
        private void CalculateDamageToOtherUnits(UnitGroup unitGroup)
        {
            var otherUnitGroups = _unitGroups.Where(u => u != unitGroup);

            foreach (var otherUnitGroup in otherUnitGroups)
            {
                var damage = AttackDamageCalculator.Calculate(unitGroup, otherUnitGroup);
                _damageToDealToUnitGroup[otherUnitGroup] += damage;
            }
        }
        
        private void DealDamageToUnitGroups()
        {
            foreach (var (unitGroup, damage) in _damageToDealToUnitGroup)
            {
                if (damage <= float.Epsilon)
                    continue;
                unitGroup.TakeDamage(damage);
                
                if (unitGroup.UnitCount.Value <= 0)
                    KillUnitGroup(unitGroup);
            }
        }

        private void KillUnitGroup(UnitGroup unitGroup)
        {
            Debug.Assert(_unitGroups.Contains(unitGroup), "The passed unit group is unknown.");
            
            _unitGroups.Remove(unitGroup);
            _damageToDealToUnitGroup.Remove(unitGroup);
            _attackSpeeds.Remove(unitGroup);
            _attackChargeProgress.Remove(unitGroup);
            
            unitGroup.DieInCombat();
        }
        
        
        private void End()
        {
            foreach (var unitGroup in _unitGroups)
                unitGroup.EndFighting();

            OnCombatEnd?.Invoke(this);
        }
    }
}
