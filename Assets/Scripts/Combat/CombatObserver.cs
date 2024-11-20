using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HexSystem;
using Sirenix.Utilities;
using Unit;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public class CombatObserver : NetworkBehaviour
    {
        [SerializeField] private CombatIndicator combatIndicatorPrefab;
        [SerializeField] private CombatArea combatAreaPrefab;
        [SerializeField] private GridData gridData;

        private readonly List<Combat> _ongoingCombats = new();

        private const float AttackSpeed = 1.5f;
        private float _deltaTime;

        private void Update()
        {
            if (_ongoingCombats.IsNullOrEmpty())
                return;
            
            _deltaTime += Time.deltaTime;
            if (_deltaTime < AttackSpeed) return;
            
            _deltaTime = 0f;
            TriggerAllCombatSteps();
        }

        public void InitializeOnServer()
        {
            GameEvents.UNIT.OnCombatTriggered += InitiateCombat; 
        }

        private void TriggerAllCombatSteps()
        {
            for (var i = _ongoingCombats.Count - 1; i >= 0; i--)
            {
                var combat = _ongoingCombats[i];
                var combatIsOver = combat.TriggerCombatStep();
                
                if (combatIsOver)
                    DeleteCombat(combat);
            }
        }
        
        private void InitiateCombat(UnitGroup unitGroup1, UnitGroup unitGroup2)
        {
            var combatPosition = (unitGroup1.transform.position + unitGroup2.transform.position)/2;
            var combatRadius = (unitGroup1.transform.position - unitGroup2.transform.position).magnitude + 0.5f;
            
            var combatIndicator = InitializeCombatIndicator(combatPosition, combatRadius);
            var combatArea = Instantiate(combatAreaPrefab, combatPosition, Quaternion.identity, transform);
            var combat = new Combat(combatIndicator, combatArea, unitGroup1, unitGroup2);
            combatArea.Initialize(combat, combatRadius);
            
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
            combat.End();
            Destroy(combat.CombatIndicator.gameObject);
            Destroy(combat.CombatArea.gameObject);
            _ongoingCombats.Remove(combat);
        }
    }
}