using System.Collections.Generic;
using System.Linq;
using GameEvents;
using HexSystem;
using Unity.Netcode;
using UnityEngine;

namespace Unit
{
    public class UnitController : NetworkBehaviour
    {
        [SerializeField] private GridData gridData;
        [SerializeField] private MapBuilder mapBuilder;
        [SerializeField] private UnitGroup unitGroupPrefab;

        private UnitGroup _selectedUnitGroup;
        private int _clientSelectionUnitCount;

        public override void OnNetworkSpawn()
        {
            ClientEvents.Input.OnHexSelectedForUnitSelectionOrMovement += HandleHexClick;
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleDayNightSwitchState;
            ClientEvents.Unit.OnUnitSelectionSliderUpdate += UpdateSelectedUnitCount;
            ClientEvents.Unit.OnUnitGroupDeselected += DeselectUnit;
            
            if(IsServer)
                ServerEvents.Unit.OnUnitGroupWithIdDeleted += DeselectUnitGroupIfSelectedClientRpc;
        }

        public override void OnNetworkDespawn()
        {
            ClientEvents.Input.OnHexSelectedForUnitSelectionOrMovement -= HandleHexClick;
            ClientEvents.DayNightCycle.OnSwitchedCycleState -= HandleDayNightSwitchState;
            ClientEvents.Unit.OnUnitSelectionSliderUpdate -= UpdateSelectedUnitCount;
            ClientEvents.Unit.OnUnitGroupDeselected -= DeselectUnit;
            
            if(IsServer)
                ServerEvents.Unit.OnUnitGroupWithIdDeleted -= DeselectUnitGroupIfSelectedClientRpc;
        }

        #region Server

        [Rpc(SendTo.Server)]
        private void RequestMoveCommandRpc(AxialCoordinates coordinates, ulong requestedUnitId, int selectionUnitCount)
        {
            var requestUnitGroup = UnitGroup.UnitGroupsInGame[requestedUnitId];
            var requestedDestination = mapBuilder.Grid.Get(coordinates);
            var newUnitPath = GetPathForUnitGroup(requestUnitGroup, requestedDestination);

            if (!requestUnitGroup.CanMove)
                return;
            
            if (selectionUnitCount < requestUnitGroup.UnitCount.Value && requestUnitGroup.UnitCount.Value > 1)
                SplitUnitGroup(requestUnitGroup, selectionUnitCount);
            
            requestUnitGroup.WaypointQueue.UpdateWaypoints(newUnitPath);
        }

        private List<Hexagon> GetPathForUnitGroup(UnitGroup unitGroup, Hexagon clickedHex)
        {
            var pathStartCoordinates = unitGroup.Movement.GoalHexagon.Coordinates;
            var clickedCoordinates = clickedHex.Coordinates;

            return mapBuilder.Grid.GetPathBetween(pathStartCoordinates, clickedCoordinates);
        }

        private void SplitUnitGroup(UnitGroup unitGroup, int selectionUnitCount)
        {
            var splitUnitGroup =
                Instantiate(unitGroupPrefab, unitGroup.transform.position, Quaternion.identity);
            splitUnitGroup.NetworkObject.Spawn();
            splitUnitGroup.Initialize(
                unitGroup.UnitCount.Value - selectionUnitCount,
                unitGroup.PlayerId, unitGroup.Movement.StartHexagon, gridData);
            splitUnitGroup.WaypointQueue.UpdateWaypoints(unitGroup.WaypointQueue.GetCurrentAndNextWaypoints(), true);
            
            unitGroup.UnitCount.Value = selectionUnitCount;
            gridData.CopyUnitGroupOnHex(unitGroup, splitUnitGroup);
        }

        #endregion

        #region Client
        
        [ClientRpc]
        private void DeselectUnitGroupIfSelectedClientRpc(ulong unitGroupId)
        {
            var unitGroup = UnitGroup.UnitGroupsInGame[unitGroupId];
            if (_selectedUnitGroup != unitGroup)
                return;

            ClientEvents.Unit.OnUnitGroupDeselected?.Invoke();
        }

        private static void HandleDayNightSwitchState(DayNightCycle.CycleState newCycleState)
        {
            if (newCycleState == DayNightCycle.CycleState.Night)
                ClientEvents.Unit.OnUnitGroupDeselected?.Invoke();
        }

        private void HandleHexClick(Hexagon clickedHex)
        {
            if (_selectedUnitGroup != null && clickedHex.isTraversable)
            {
                Debug.Log("Move Command Registered");
                RequestMoveCommandRpc(clickedHex.Coordinates, _selectedUnitGroup.NetworkObjectId, _clientSelectionUnitCount);
            }
            else
            {
                SelectUnitGroupOnHex(clickedHex);
            }
        }

        private void SelectUnitGroupOnHex(Hexagon hex)
        {
            var unitOnHex = gridData.FirstPlayerUnitOnHexOrNull(hex.Coordinates, NetworkManager.Singleton.LocalClientId);
            if (unitOnHex == null) 
                return;
            
            if (_selectedUnitGroup != null)
            {
                _selectedUnitGroup.DisableHighlight();
                _selectedUnitGroup.OnUnitCountUpdated -= HandleUnitCountOfSelectedChanged;
            }
                
            _selectedUnitGroup = UnitGroup.UnitGroupsInGame[unitOnHex.Value];
            _selectedUnitGroup.EnableHighlight();
            _selectedUnitGroup.OnUnitCountUpdated += HandleUnitCountOfSelectedChanged;
                
            ClientEvents.Unit.OnUnitGroupSelected?.Invoke(_selectedUnitGroup);
        }

        private void HandleUnitCountOfSelectedChanged(int newUnitCount)
        {
            ClientEvents.Unit.OnUnitCountOfSelectedChanged?.Invoke(newUnitCount);
        }

        private void DeselectUnit()
        {
            if (_selectedUnitGroup == null)
                return;
            
            _selectedUnitGroup.DisableHighlight();
            _selectedUnitGroup = null;
        }

        private void UpdateSelectedUnitCount(int count)
        {
            _clientSelectionUnitCount = count;
        }

        #endregion
    }
}