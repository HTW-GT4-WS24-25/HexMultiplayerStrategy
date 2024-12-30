using System.Collections.Generic;
using System.Linq;
using Core.Buildings;
using Core.GameEvents;
using Core.HexSystem.Hex;
using Core.Unit;
using Core.Unit.Group;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;

namespace Core.HexSystem
{
    public class GridData : NetworkBehaviour
    {
        private readonly Dictionary<AxialCoordinates, HexagonData> _hexDataByCoordinates = new();
        private readonly Dictionary<ulong, AxialCoordinates> _coordinatesByUnitGroups = new();

        public void SetupNewData(int[] mapData, int nRings)
        {
            _hexDataByCoordinates.Clear();
            
            var dataIndex = 0;
            foreach (var coordinate in HexagonGrid.GetHexRingsAroundCoordinates(AxialCoordinates.Zero, nRings))
            {
                var hexType = (HexType)mapData[dataIndex++];
                _hexDataByCoordinates.Add(coordinate, new HexagonData(coordinate, hexType));  
            }
        }

        public HexagonData GetHexagonDataOnCoordinate(AxialCoordinates coordinate)
        {
            Debug.Assert(_hexDataByCoordinates.ContainsKey(coordinate), "Tried to get HexagonDataOnCoordinate, but there is no hex on coordinate!");
            
            return _hexDataByCoordinates[coordinate]; // Todo: maybe we should find a way to make this immutable
        }
        
        public IEnumerable<HexagonData> GetAllHexData()
        {
            return _hexDataByCoordinates.Values;
        }

        public HexagonData GetCurrentHexFromUnitGroup(UnitGroup unitGroup)
        {
            var coordinates = _coordinatesByUnitGroups[unitGroup.NetworkObjectId];
            return _hexDataByCoordinates[coordinates];
        }

        #region Server

        public override void OnNetworkSpawn()
        {
            ServerEvents.Unit.OnUnitGroupWithIdDeleted += DeleteUnitGroup;
            ServerEvents.Unit.OnUnitGroupReachedNewHex += MoveUnitGroupToHex;
            ServerEvents.Unit.OnUnitGroupLeftHexCenter += RemoveStationaryUnitGroupFromHex;
        }

        public override void OnNetworkDespawn()
        {
            ServerEvents.Unit.OnUnitGroupWithIdDeleted -= DeleteUnitGroup;
            ServerEvents.Unit.OnUnitGroupReachedNewHex -= MoveUnitGroupToHex;
            ServerEvents.Unit.OnUnitGroupLeftHexCenter -= RemoveStationaryUnitGroupFromHex;
        }

        public void PlaceUnitGroupOnHex(AxialCoordinates coordinate, UnitGroup unitGroupToAdd)
        {
            var unitGroupToAddId = unitGroupToAdd.NetworkObjectId;
            AddUnitToUnitsOnHexClientRpc(coordinate, unitGroupToAddId);
            UpdateStationaryUnitGroupOfHexClientRpc(coordinate, true, unitGroupToAddId);
            
            ServerEvents.Unit.OnUnitGroupShouldReceiveMoveSpeedUpdate?.Invoke(unitGroupToAdd);
        }

        public void CopyUnitGroupOnHex(UnitGroup originalUnitGroup, UnitGroup copiedUnitGroup)
        {
            var originalUnitGroupId = originalUnitGroup.NetworkObjectId;
            var originalUnitCoordinates = _coordinatesByUnitGroups[originalUnitGroupId];
            
            var copiedUnitGroupId = copiedUnitGroup.NetworkObjectId;
           
            AddUnitToUnitsOnHexClientRpc(originalUnitCoordinates, copiedUnitGroupId);
            var hexagonData = _hexDataByCoordinates[originalUnitCoordinates];
            
            ServerEvents.Unit.OnUnitGroupShouldReceiveMoveSpeedUpdate?.Invoke(copiedUnitGroup);
            
            if (hexagonData.StationaryUnitGroup == originalUnitGroupId)
                UpdateStationaryUnitGroupOfHexClientRpc(originalUnitCoordinates, true, copiedUnitGroupId);
        }

        public void DeleteUnitGroup(ulong unitGroupIdToRemove)
        {
            var unitGroupToRemove = UnitGroup.UnitGroupsInGame[unitGroupIdToRemove];
            
            var coordinates = _coordinatesByUnitGroups[unitGroupToRemove.NetworkObjectId];
            DeleteUnitGroupFromHexClientRpc(coordinates, unitGroupToRemove.NetworkObjectId);
        }

        public void MoveUnitGroupToHex(UnitGroup unitGroupToMove, AxialCoordinates newCoordinate)
        {
            var unitGroupId = unitGroupToMove.NetworkObjectId;
            if (_coordinatesByUnitGroups.TryGetValue(unitGroupId, out var oldCoordinate))
                MoveUnitGroupToHexClientRpc(oldCoordinate, newCoordinate, unitGroupId);
            else
                AddUnitToUnitsOnHexClientRpc(newCoordinate, unitGroupId);
            
            ServerEvents.Unit.OnUnitGroupShouldReceiveMoveSpeedUpdate?.Invoke(unitGroupToMove);
        }

        public void UpdateControllingPlayerOfHex(AxialCoordinates coordinate, ulong playerId)
        {
            UpdateControllingPlayerOfHexClientRpc(coordinate, playerId);
        }
        
        public void UpdateStationaryUnitGroupOfHex(AxialCoordinates coordinate, UnitGroup newStationaryUnitGroup)
        {
            Debug.Assert(newStationaryUnitGroup != null, "Tried to update stationary group with null!");
            
            UpdateStationaryUnitGroupOfHexClientRpc(coordinate, newStationaryUnitGroup, newStationaryUnitGroup.NetworkObjectId);   
        }
        
        public void SetBuildingOnHex(AxialCoordinates coordinate, Building building)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            hexagonData.Building = building;
            
            SetBuildingOnHexClientRpc(coordinate, building.Type);
        }

        public void UpgradeBuildingOnHex(AxialCoordinates coordinate)
        {
            UpgradeBuildingOnHexClientRpc(coordinate);
        }

        private void RemoveStationaryUnitGroupFromHex(UnitGroup unitGroupToRemove)
        {
            var unitGroupId = unitGroupToRemove.NetworkObjectId;
            var coordinate = _coordinatesByUnitGroups[unitGroupId];
            var hexagonData = _hexDataByCoordinates[coordinate];
            
            if(hexagonData.StationaryUnitGroup == unitGroupId)
                UpdateStationaryUnitGroupOfHexClientRpc(coordinate, false, 0);
        }

        #endregion

        #region Client

        [ClientRpc]
        private void AddUnitToUnitsOnHexClientRpc(AxialCoordinates coordinate, ulong unitGroupToAdd)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            
            Debug.Assert(!hexagonData.UnitsOnHex.Contains(unitGroupToAdd),
                "Tried to add unit group to hex that already contained it!");
            
            hexagonData.UnitsOnHex.Add(unitGroupToAdd);
            
            Debug.Assert(!_coordinatesByUnitGroups.ContainsKey(unitGroupToAdd), "Tried to add unit group to hex that is already assigned to a hex!");
            _coordinatesByUnitGroups.Add(unitGroupToAdd, coordinate);
        }
        
        [ClientRpc]
        private void DeleteUnitGroupFromHexClientRpc(AxialCoordinates coordinate, ulong unitGroupToRemove)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            
            Debug.Assert(hexagonData.UnitsOnHex.Contains(unitGroupToRemove),
                "Tried to delete unit group from hex that does not exist in hex!");
            
            hexagonData.UnitsOnHex.Remove(unitGroupToRemove);
            if(hexagonData.StationaryUnitGroup != null && hexagonData.StationaryUnitGroup == unitGroupToRemove)
                hexagonData.StationaryUnitGroup = null;
            
            _coordinatesByUnitGroups.Remove(unitGroupToRemove);
        }
        
        [ClientRpc]
        private void MoveUnitGroupToHexClientRpc(AxialCoordinates oldCoordinate, AxialCoordinates newCoordinate, ulong unitGroupToMove)
        {
            var dataOfOldHexagon = _hexDataByCoordinates[oldCoordinate];
            var dataOfNewHexagon = _hexDataByCoordinates[newCoordinate];
            
            Debug.Assert(dataOfOldHexagon.UnitsOnHex.Contains(unitGroupToMove),
                "Tried to move unit from hex that didn't contain it!");

            dataOfOldHexagon.UnitsOnHex.Remove(unitGroupToMove);
            dataOfNewHexagon.UnitsOnHex.Add(unitGroupToMove);
            
            _coordinatesByUnitGroups[unitGroupToMove] = newCoordinate;
        }

        [ClientRpc]
        private void UpdateControllingPlayerOfHexClientRpc(AxialCoordinates coordinate, ulong playerId)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            Debug.Assert(hexagonData.ControllerPlayerId != playerId, "Hexagon is already controlled by player!");
            
            hexagonData.ControllerPlayerId = playerId;
        }

        [ClientRpc]
        private void UpdateStationaryUnitGroupOfHexClientRpc(AxialCoordinates coordinate, bool newStationaryGroupExists, ulong newStationaryUnitGroup)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            hexagonData.StationaryUnitGroup = newStationaryGroupExists ? newStationaryUnitGroup : null;

            if (newStationaryGroupExists)
            {
                Debug.Assert(hexagonData.UnitsOnHex.Contains(newStationaryUnitGroup),
                    "Tried to add stationary unit group that does not exist on hexagon!");
            }
        }

        [Rpc(SendTo.NotServer)]
        private void SetBuildingOnHexClientRpc(AxialCoordinates coordinate, BuildingType buildingType)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            
            var newBuilding = BuildingFactory.Create(buildingType);
            hexagonData.Building = newBuilding;
        }

        [ClientRpc]
        private void UpgradeBuildingOnHexClientRpc(AxialCoordinates coordinate)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            hexagonData.Building.Upgrade();
        }

        #endregion
    }
}