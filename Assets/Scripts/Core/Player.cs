using System.Collections.Generic;
using Core.Buildings;
using Core.Factions;
using Core.GameEvents;
using Core.HexSystem;
using Core.HexSystem.Hex;
using Core.PlayerData;
using Core.Unit.Group;
using Core.Unit.Model;
using Unity.Netcode;
using UnityEngine;

namespace Core
{
    public class Player
    {
        public readonly ulong ClientId;
        public readonly string Name;
        public readonly PlayerMoney Money = new();
        public readonly FactionRuntimeData FactionRuntimeData = new();
        
        public PlayerColor.ColorType PlayerColorType
        {
            get => _playerColorType;
            set
            {
                _playerColorType = value;
                ServerEvents.Player.OnPlayerColorChanged?.Invoke(ClientId, (int)_playerColorType);
            }
        }

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                ServerEvents.Player.OnPlayerScoreChanged?.Invoke(ClientId, _score);
            }
        }

        public int NumberOfControlledHexes => _hexagonsUnderControl.Count;
        
        public Faction Faction { get; set; }
        public GridData GridData { get; set; }
        
        private PlayerColor.ColorType _playerColorType;
        private int _score;
        private List<HexagonData> _hexagonsUnderControl = new();
        
        public Player(ulong clientId, string name)
        {
            ClientId = clientId;
            Name = name;

            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleDayNightCycleSwitched;
            ServerEvents.Unit.OnUnitGroupShouldReceiveMoveSpeedUpdate += UpdateUnitGroupMoveSpeed; // Todo: when should we stop listening?
        }

        public void IncrementScore(int amount)
        {
            Score += amount;
        }
        
        private void UpdateUnitGroupMoveSpeed(UnitGroup unitGroupToUpdate)
        {
            unitGroupToUpdate.Movement.MoveSpeed = CalculateMoveSpeed(unitGroupToUpdate);
        }

        #region ValueCalculation
        
        public int CalculateScoreToGainAtNightfall(List<BuildingYield> buildingYields)
        {
            var buildingScores = GetScoreFromBuildingYields(buildingYields);
            return Faction.CalculateScoreToGainAtNightfall(this) + buildingScores;
        }

        public int CalculateGoldToGainAtNightfall()
        {
            return Faction.CalculateGoldToGainAtNightfall(this);
        }

        public float CalculateDamageToDeal(UnitGroup ownUnitGroup, UnitGroup otherUnitGroup)
        {
            return Faction.CalculateDamageToDeal(this, ownUnitGroup, otherUnitGroup);
        }

        public float CalculateDamageToReceive(float damageToBeDealt, UnitGroup ownUnitGroup, UnitGroup otherUnitGroup)
        {
            return Faction.CalculateDamageToReceive(this, damageToBeDealt, ownUnitGroup, otherUnitGroup);
        }

        public float CalculateAttackSpeed(UnitGroup ownUnitGroup, UnitGroup[] otherUnitGroups)
        {
            return Faction.CalculateAttackSpeed(this, ownUnitGroup, otherUnitGroups);
        }

        public float CalculateMoveSpeed(UnitGroup unitGroup)
        {
            return Faction.CalculateMoveSpeed(this, unitGroup);
        }
        
        #endregion
        
        private void HandleDayNightCycleSwitched(DayNightCycle.DayNightCycle.CycleState cycleState)
        {
            if (cycleState == DayNightCycle.DayNightCycle.CycleState.Day)
                OnDawn();
            else
                OnNightfall();
        }

        public void OnDawn()
        {
            List<BuildingYield> yields = new();
            foreach (var hexagon in _hexagonsUnderControl)
            {
                if (hexagon.Building == null)
                    continue;

                var yield = hexagon.Building.OnDawn();
                if(yield != null)
                    yields.Add(yield);
            }
        }

        public void OnNightfall()
        {
            List<BuildingYield> yields = new();
            foreach (var hexagon in _hexagonsUnderControl)
            {
                if (hexagon.Building == null)
                    continue;

                var yield = hexagon.Building.OnNightFall();
                if(yield != null)
                    yields.Add(yield);
            }
            
            Score += CalculateScoreToGainAtNightfall(yields);
        }

        private int GetGoldFromBuildingYields(List<BuildingYield> yields)
        {
            var totalGoldYield = 0;
            yields.ForEach(y => totalGoldYield += y.Gold);
            
            return totalGoldYield;
        }
        
        private int GetScoreFromBuildingYields(List<BuildingYield> yields)
        {
            var totalScoreYield = 0;
            yields.ForEach(y => totalScoreYield += y.Score);
            
            return totalScoreYield;
        }

        public void OnGainedControlOfHex(HexagonData hexagon)
        {
            _hexagonsUnderControl.Add(hexagon);
        }
        
        public void OnLostControlOfHex(HexagonData hexagon)
        {
            _hexagonsUnderControl.Remove(hexagon);   
        }
    }
}
