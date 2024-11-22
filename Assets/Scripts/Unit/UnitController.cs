using System.Collections.Generic;
using System.Linq;
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
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement += HandleHexClick;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += HandleDayNightSwitchState;
            GameEvents.UNIT.OnUnitSelectionSliderUpdate += UpdateSelectedUnitCount;
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
        }

        #region Server

        [Rpc(SendTo.Server)]
        private void RequestMoveCommandRpc(AxialCoordinates coordinates, ulong requestedUnitId, int selectionUnitCount)
        {
            var requestUnitGroup = UnitGroup.UnitGroupsInGame[requestedUnitId];
            var requestedDestination = mapBuilder.Grid.Get(coordinates);
            var newUnitPath = GetPathForUnitGroup(requestUnitGroup, requestedDestination);

            if (selectionUnitCount < requestUnitGroup.UnitCount.Value && requestUnitGroup.UnitCount.Value > 1)
                SplitUnitGroup(requestUnitGroup, selectionUnitCount);
            
            requestUnitGroup.Movement.SetAllWaypoints(newUnitPath);
        }

        private List<Hexagon> GetPathForUnitGroup(UnitGroup unitGroup, Hexagon clickedHex)
        {
            var currentUnitCoordinates = unitGroup.Movement.NextHexagon.Coordinates;
            var clickedCoordinates = clickedHex.Coordinates;

            return mapBuilder.Grid.GetPathBetween(currentUnitCoordinates, clickedCoordinates);
        }

        private void SplitUnitGroup(UnitGroup unitGroup, int selectionUnitCount)
        {
            var splitUnitGroup =
                Instantiate(unitGroupPrefab, unitGroup.transform.position, Quaternion.identity);
            splitUnitGroup.NetworkObject.Spawn();
            splitUnitGroup.Initialize(
                unitGroup.UnitCount.Value - selectionUnitCount,
                unitGroup.PlayerId,
                unitGroup.Movement.NextHexagon,
                gridData);
            
            splitUnitGroup.Movement.CopyValuesFrom(unitGroup.Movement);
            
            unitGroup.UnitCount.Value = selectionUnitCount;
            
            gridData.CopyUnitGroupOnHex(unitGroup, splitUnitGroup);
        }

        #endregion

        #region Client

        private static void HandleDayNightSwitchState(DayNightCycle.CycleState newCycleState)
        {
            if (newCycleState == DayNightCycle.CycleState.Night)
                GameEvents.UNIT.OnUnitGroupDeselected?.Invoke();
        }

        private void HandleHexClick(Hexagon clickedHex)
        {
            if (_selectedUnitGroup != null && clickedHex.isTraversable)
            {
                RequestMoveCommandRpc(clickedHex.Coordinates, _selectedUnitGroup.NetworkObjectId, _clientSelectionUnitCount);
            }
            else
            {
                SelectUnitGroupOnHex(clickedHex);
            }
        }

        private void SelectUnitGroupOnHex(Hexagon hex)
        {
            var unitOnHex = gridData.FirsPlayerUnitOnHexOrNull(hex.Coordinates, NetworkManager.Singleton.LocalClientId);
            if (unitOnHex != null)
            {
                if (_selectedUnitGroup != null)
                {
                    _selectedUnitGroup.DisableHighlight();
                    _selectedUnitGroup.OnUnitCountUpdated -= HandleUnitCountOfSelectedChanged;
                }
                
                _selectedUnitGroup = UnitGroup.UnitGroupsInGame[unitOnHex.Value];
                _selectedUnitGroup.EnableHighlight();
                _selectedUnitGroup.OnUnitCountUpdated += HandleUnitCountOfSelectedChanged;
                
                GameEvents.UNIT.OnUnitGroupSelected?.Invoke(_selectedUnitGroup);
            }
        }

        private void HandleUnitCountOfSelectedChanged(int newUnitCount)
        {
            GameEvents.UNIT.OnUnitCountOfSelectedChanged?.Invoke(newUnitCount);
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

        private void DeselectDeletedUnit(UnitGroup unitGroup)
        {
            if (_selectedUnitGroup != unitGroup)
                return;

            GameEvents.UNIT.OnUnitGroupDeselected.Invoke();
        }

        #endregion
    }
}