using System;
using System.Collections.Generic;
using Core.GameEvents;
using Core.HexSystem;
using Core.Unit.Group;
using Sirenix.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Core.Combat
{
    public class CombatController : NetworkBehaviour
    {
        [SerializeField] private DayNightCycle.DayNightCycle dayNightCycle;
        [SerializeField] private CombatIndicator combatIndicatorPrefab;
        [SerializeField] private CombatArea combatAreaPrefab;
        [SerializeField] private GridData gridData;

        private readonly List<Combat> _ongoingCombats = new();

        private float _deltaTime;
        private bool _isPaused;
        

        private void Update()
        {
            if (_ongoingCombats.IsNullOrEmpty() || _isPaused)
                return;
            
            TriggerAllCombatUpdates();
        }

        public void InitializeOnServer()
        {
            ServerEvents.Unit.OnCombatTriggered += InitiateCombat; 
            ClientEvents.DayNightCycle.OnSwitchedCycleState += UpdateIsPaused;
            
            UpdateIsPaused(dayNightCycle.cycleState);
        }

        private void UpdateIsPaused(DayNightCycle.DayNightCycle.CycleState newDayNightCycle)
        {
            _isPaused = newDayNightCycle == DayNightCycle.DayNightCycle.CycleState.Night;

            if (_isPaused)
            {
                _ongoingCombats.ForEach(combat => combat.PauseCombat());
            }
        }

        private void TriggerAllCombatUpdates()
        {
            for (var i = _ongoingCombats.Count - 1; i >= 0; i--)
                _ongoingCombats[i].ProcessCombatFrame();
        }
        
        private void InitiateCombat(UnitGroup unitGroup1, UnitGroup unitGroup2)
        {
            var combatPosition = (unitGroup1.transform.position + unitGroup2.transform.position)/2;
            var combatRadius = (unitGroup1.transform.position - unitGroup2.transform.position).magnitude + 0.5f;
            
            var combatIndicator = InitializeCombatIndicator(combatPosition, combatRadius);
            var combatArea = Instantiate(combatAreaPrefab, combatPosition, Quaternion.identity, transform);
            var combat = new Combat(combatIndicator, combatArea, unitGroup1, unitGroup2);
            combatArea.Initialize(combat, combatRadius);

            combat.OnCombatEnd += DeleteCombat;
            _ongoingCombats.Add(combat);
        }

        private CombatIndicator InitializeCombatIndicator(Vector3 combatPosition, float combatRadius)
        {
            var combatIndicator = Instantiate(combatIndicatorPrefab, combatPosition, Quaternion.identity, transform);
            combatIndicator.GetComponent<NetworkObject>().Spawn();
            combatIndicator.ShowForAll(combatRadius);
            return combatIndicator;
        }

        private void DeleteCombat(Combat combat)
        {
            Destroy(combat.CombatIndicator.gameObject);
            Destroy(combat.CombatArea.gameObject);
            
            combat.OnCombatEnd -= DeleteCombat;
            _ongoingCombats.Remove(combat);
        }
    }
}