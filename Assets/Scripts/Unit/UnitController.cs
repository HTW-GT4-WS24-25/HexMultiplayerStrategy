using System;
using System.Collections.Generic;
using System.Linq;
using Helper;
using HexSystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unit
{
    public class UnitController : NetworkBehaviour
    {
        [SerializeField] private MapBuilder mapBuilder;
        [SerializeField] private UnitGroup unitGroupPrefab;
    
        private UnitGroup _selectedUnitGroup;
        private int _selectedUnitCount = 1;

        public override void OnNetworkSpawn()
        {
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement += HandleHexClick;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += HandleDayNightSwitchState;
            GameEvents.UNIT.OnUnitSelectionSliderUpdate += UpdateSelectedUnitCount;
            GameEvents.UNIT.OnUnitGroupSelected += SetSelectedUnit;
            GameEvents.UNIT.OnUnitGroupDeselected += DeselectUnit;
            GameEvents.UNIT.OnUnitGroupDeleted += DeselectDeletedUnit;
        }

        public override void OnNetworkDespawn()
        {
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement -= HandleHexClick;
            GameEvents.UNIT.OnUnitGroupDeselected -= DeselectUnit;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += HandleDayNightSwitchState;
            GameEvents.UNIT.OnUnitSelectionSliderUpdate -= UpdateSelectedUnitCount;
            GameEvents.UNIT.OnUnitGroupDeleted -= DeselectDeletedUnit;
            GameEvents.UNIT.OnUnitGroupSelected -= SetSelectedUnit;
        }

        #region Server
        
        [Rpc(SendTo.Server)]
        private void RequestUnitSelectionFromHexRpc(AxialCoordinates coordinates)
        {
            var requestedClickHex = mapBuilder.Grid.Get(coordinates);
            var clickedHex = requestedClickHex.GetComponent<ServerHexagon>();
            
            clickedHex.UnitGroups.RemoveAll(unitGroup => unitGroup == null); //Unfortunately necessary, because sometimes deleted Groups are still referenced from the hex
            var unitGroupToSelect = clickedHex.UnitGroups.FirstOrDefault(); // Make it only select controlled units
            
            if(unitGroupToSelect != null)
                unitGroupToSelect.SetAsSelectedForControllingPlayer();
        }
        
        [Rpc(SendTo.Server)]
        private void RequestMoveCommandRpc(AxialCoordinates coordinates)
        {
            var requestedDestination = mapBuilder.Grid.Get(coordinates);
            var hexagons = GetPathForSelectedUnitGroup(requestedDestination);
            
            if(_selectedUnitCount < _selectedUnitGroup.UnitCount.Value && _selectedUnitGroup.UnitCount.Value > 1)
            {
                var splitUnit = SplitSelectedUnit();
                var splitUnitFollowsOldOne = !hexagons.Contains(_selectedUnitGroup.Movement.PreviousHexagon.ClientHexagon);
                
                splitUnit.SetAsSelectedForControllingPlayer();
                
                if(splitUnitFollowsOldOne)
                    hexagons.Insert(0, _selectedUnitGroup.Movement.NextHexagon.ClientHexagon);
            }

            var newUnitPath = hexagons.Select(hex => hex.GetComponent<ServerHexagon>()).ToList();
            _selectedUnitGroup.Movement.SetAllWaypoints(newUnitPath);
        }

        private List<ClientHexagon> GetPathForSelectedUnitGroup(ClientHexagon clickedHex)
        {
            var currentUnitCoordinates = _selectedUnitGroup.Movement.NextHexagon.Coordinates;
            var clickedCoordinates = clickedHex.Coordinates;
                
            return mapBuilder.Grid.GetPathBetween(currentUnitCoordinates, clickedCoordinates);
        }
        
        private UnitGroup SplitSelectedUnit()
        {
            _selectedUnitGroup.SubtractUnits(_selectedUnitCount);
            
            var splitUnitGroup = Instantiate(unitGroupPrefab, _selectedUnitGroup.transform.position, Quaternion.identity);
            splitUnitGroup.Initialize(_selectedUnitGroup.Movement.NextHexagon, _selectedUnitCount, _selectedUnitGroup.PlayerId);

            return splitUnitGroup;
        }
        
        #endregion

        #region Client

        private static void HandleDayNightSwitchState(DayNightCycle.CycleState newCycleState)
        {
            if (newCycleState == DayNightCycle.CycleState.Night)
                GameEvents.UNIT.OnUnitGroupDeselected?.Invoke();
        }

        private void HandleHexClick(ClientHexagon clickedHex)
        {
            if (_selectedUnitGroup != null && clickedHex.isTraversable)
            {
                RequestMoveCommandRpc(clickedHex.Coordinates);
            }
            else
            {
                RequestUnitSelectionFromHexRpc(clickedHex.Coordinates);
            }
        }
        
        private void SetSelectedUnit(UnitGroup unitGroupToSelect)
        {
            if (_selectedUnitGroup != null) _selectedUnitGroup.DisableHighlight();

            _selectedUnitGroup = unitGroupToSelect;
            
            _selectedUnitGroup.EnableHighlight();
        }

        private void DeselectUnit()
        {
            _selectedUnitGroup.DisableHighlight();
            _selectedUnitGroup = null;
        }

        private void UpdateSelectedUnitCount(int count)
        {
            _selectedUnitCount = count;
        }
        
        private void DeselectDeletedUnit(UnitGroup unitGroup)
        {
            if (_selectedUnitGroup != unitGroup) 
                return;
            
            GameEvents.UNIT.OnUnitGroupDeselected.Invoke();
        }

        #endregion
    }
}
