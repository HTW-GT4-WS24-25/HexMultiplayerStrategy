using System.Collections.Generic;
using Core.GameEvents;
using Core.HexSystem;
using Core.HexSystem.Generation;
using Core.HexSystem.Hex;
using Core.HexSystem.VFX;
using Core.Unit.Group;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Unit
{
    public class UnitController : NetworkBehaviour
    {
        [SerializeField] private GridData gridData;
        [SerializeField] private MapBuilder mapBuilder;
        [SerializeField] private UnitGroup unitGroupPrefab;
        [SerializeField] private MouseOverHexHighlighter mouseOverHexHighlighter;

        private UnitGroup _selectedUnitGroup;
        private int _clientSelectionUnitCount;

        public override void OnNetworkSpawn()
        {
            ClientEvents.Input.OnHexSelectedForUnitMovement += HandleHexClick;
            ClientEvents.Input.OnUnitGroupSelected += SelectUnitGroup;
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleDayNightSwitchState;
            ClientEvents.Unit.OnUnitSelectionSliderUpdate += UpdateSelectedUnitCount;
            ClientEvents.Unit.OnUnitGroupDeselected += CancelCurrentUnitSelection;
            
            if(IsServer)
                ServerEvents.Unit.OnUnitGroupWithIdDeleted += DeselectUnitGroupIfSelectedClientRpc;
        }

        public override void OnNetworkDespawn()
        {
            ClientEvents.Input.OnHexSelectedForUnitMovement -= HandleHexClick;
            ClientEvents.Input.OnUnitGroupSelected -= SelectUnitGroup;
            ClientEvents.DayNightCycle.OnSwitchedCycleState -= HandleDayNightSwitchState;
            ClientEvents.Unit.OnUnitSelectionSliderUpdate -= UpdateSelectedUnitCount;
            ClientEvents.Unit.OnUnitGroupDeselected -= CancelCurrentUnitSelection;
            
            if(IsServer)
                ServerEvents.Unit.OnUnitGroupWithIdDeleted -= DeselectUnitGroupIfSelectedClientRpc;
        }

        #region Server

        [Rpc(SendTo.Server)]
        private void RequestMoveCommandRpc(AxialCoordinates coordinates, ulong requestedUnitId, int selectionUnitCount)
        {
            var requestUnitGroup = UnitGroup.UnitGroupsInGame[requestedUnitId];
            if (!requestUnitGroup.CanMove)
                return;
            
            var requestedDestination = mapBuilder.Grid.Get(coordinates);
            var newUnitPath = GetPathForUnitGroup(requestUnitGroup, requestedDestination);
            
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
            
            gridData.CopyUnitGroupOnHex(unitGroup, splitUnitGroup);
            
            splitUnitGroup.InitializeAsSplitFrom(unitGroup, unitGroup.UnitCount.Value - selectionUnitCount);
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

        private static void HandleDayNightSwitchState(DayNightCycle.DayNightCycle.CycleState newCycleState)
        {
            if (newCycleState == DayNightCycle.DayNightCycle.CycleState.Night)
                ClientEvents.Unit.OnUnitGroupDeselected?.Invoke();
        }

        private void HandleHexClick(Hexagon clickedHex)
        {
            if (_selectedUnitGroup != null && clickedHex.isTraversable)
            {
                RequestMoveCommandRpc(clickedHex.Coordinates, _selectedUnitGroup.NetworkObjectId, _clientSelectionUnitCount);
            }
        }

        private void SelectUnitGroup(UnitGroup unitGroup)
        {
            CancelCurrentUnitSelection();
            
            _selectedUnitGroup = unitGroup;
            _selectedUnitGroup.EnableSelectionHighlight();
            _selectedUnitGroup.OnUnitCountUpdated += HandleUnitCountOfSelectedChanged;
            mouseOverHexHighlighter.Enable();
        }

        private void HandleUnitCountOfSelectedChanged(int newUnitCount)
        {
            ClientEvents.Unit.OnUnitCountOfSelectedChanged?.Invoke(newUnitCount);
        }

        private void CancelCurrentUnitSelection()
        {
            if (_selectedUnitGroup == null)
                return;
            
            _selectedUnitGroup.OnUnitCountUpdated -= HandleUnitCountOfSelectedChanged;
            _selectedUnitGroup.DisableSelectionHighlight();
            _selectedUnitGroup = null;
            mouseOverHexHighlighter.Disable();
        }

        private void UpdateSelectedUnitCount(int count)
        {
            _clientSelectionUnitCount = count;
        }

        #endregion
    }
}