using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace Combat
{
    public class Combat
    {
        public readonly CombatIndicator CombatIndicator;
        public readonly CombatArea CombatArea;
        
        private readonly List<UnitGroup> _unitGroups;
        private readonly HashSet<UnitGroup> _killedUnitGroups;

        public Combat(CombatIndicator combatIndicator, CombatArea combatArea, UnitGroup unitGroup1, UnitGroup unitGroup2)
        {
            CombatIndicator = combatIndicator;
            CombatArea = combatArea;
            
            unitGroup1.Movement.IsFighting = true;
            unitGroup2.Movement.IsFighting = true;
            _unitGroups = new List<UnitGroup> { unitGroup1, unitGroup2 };
            _killedUnitGroups = new HashSet<UnitGroup>();
        }

        public bool TriggerCombatStep()
        {
            foreach (var unitGroup in _unitGroups)
                DamageOtherUnits(unitGroup);
            
            DeleteKilledUnits();
            
            var combatIsOver = _unitGroups.Count <= 1;
            return combatIsOver;
        }
        
        public void End()
        {
            foreach (var unitGroup in _unitGroups)
                unitGroup.Movement.IsFighting = false;
        }

        public void JoinCombat(UnitGroup unitGroup)
        {
            Debug.Assert(_unitGroups.Contains(unitGroup), "Tried to join a combat group but it was already joined.");
            
            var existingUnitGroupOfPlayer = _unitGroups.Find(u => u.PlayerId.Equals(unitGroup.PlayerId));
            if (existingUnitGroupOfPlayer != null)
            {
                existingUnitGroupOfPlayer.AddUnits(unitGroup.UnitCount.Value);
                unitGroup.Delete();
            }
            else
            {
                unitGroup.Movement.IsFighting = true;
                _unitGroups.Add(unitGroup);
            }
        }
        
        private void DamageOtherUnits(UnitGroup unitGroup)
        {
            var otherUnitGroups = _unitGroups.FindAll(u => u != unitGroup);
            foreach (var otherUnitGroup in otherUnitGroups)
            {
                otherUnitGroup.TakeDamage(1);
                if (otherUnitGroup.UnitCount.Value <= 0)
                    _killedUnitGroups.Add(otherUnitGroup);
            }
        }
        
        private void DeleteKilledUnits()
        {
            foreach (var unitGroup in _killedUnitGroups)
            {
                _unitGroups.Remove(unitGroup);
                unitGroup.Delete();
            }
            
            _killedUnitGroups.Clear();
        }
    }
}