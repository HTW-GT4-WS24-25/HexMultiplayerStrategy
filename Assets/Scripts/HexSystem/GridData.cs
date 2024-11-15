﻿using System.Collections.Generic;
using System.Linq;
using Unit;
using Unity.Netcode;
using UnityEngine;

namespace HexSystem
{
    public class GridData : NetworkBehaviour
    {
        private readonly Dictionary<AxialCoordinates, HexagonData> _hexDataByCoordinates = new();
        private readonly Dictionary<ulong, AxialCoordinates> _coordinatesByUnitGroups = new();

        public void SetupNewData(int nRings)
        {
            _hexDataByCoordinates.Clear();
            
            foreach (var coordinate in HexagonGrid.GetHexRingsAroundCoordinates(new AxialCoordinates(0, 0), nRings))
            {
                _hexDataByCoordinates.Add(coordinate, new HexagonData(coordinate));       
            }
        }

        public HexagonData GetHexagonDataOnCoordinate(AxialCoordinates coordinate)
        {
            Debug.Assert(_hexDataByCoordinates.ContainsKey(coordinate), "Tried to get HexagonDataOnCoordinate, but there is no hex on coordinate!");
            
            return _hexDataByCoordinates[coordinate]; // Todo: maybe we should find a way to make this immutable
        }

        #region Server

        public ulong? FirsPlayerUnitOnHexOrNull(AxialCoordinates coordinate, ulong playerId)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            if (hexagonData.UnitsOnHex.All(unitId => UnitGroup.UnitGroupsInGame[unitId].PlayerId != playerId))
                return null;
            
            return hexagonData.UnitsOnHex.First(unitId => UnitGroup.UnitGroupsInGame[unitId].PlayerId == playerId);
        }

        public bool DoesHexCenterContainUnitsFromOtherPlayers(AxialCoordinates coordinates, ulong playerId)
        {
            var hexagonData = _hexDataByCoordinates[coordinates];
            foreach (var unitId in hexagonData.UnitsInHexCenter)
            {
                if (UnitGroup.UnitGroupsInGame[unitId].PlayerId != playerId)
                    return true;
            }

            return false;
        }

        public void PlaceUnitGroupOnHex(AxialCoordinates coordinate, UnitGroup unitGroupToAdd) 
        {
            var unitGroupToAddId = unitGroupToAdd.NetworkObjectId;
            
            AddUnitToUnitsOnHexClientRpc(coordinate, unitGroupToAddId);
            AddUnitGroupToHexCenterClientRpc(coordinate, unitGroupToAddId);
            UpdateStationaryUnitGroupOfHexClientRpc(coordinate, true, unitGroupToAddId);
        }

        public void CopyUnitGroupOnHex(UnitGroup originalUnitGroup, UnitGroup copiedUnitGroup)
        {
            var originalUnitGroupId = originalUnitGroup.NetworkObjectId;
            var originalUnitCoordinates = _coordinatesByUnitGroups[originalUnitGroupId];
            
            var copiedUnitGroupId = copiedUnitGroup.NetworkObjectId;
            
            var hexagonData = _hexDataByCoordinates[originalUnitCoordinates];
            
            AddUnitToUnitsOnHexClientRpc(originalUnitCoordinates, copiedUnitGroupId);
    
            if (hexagonData.UnitsInHexCenter.Contains(originalUnitGroupId))
                AddUnitGroupToHexCenterClientRpc(originalUnitCoordinates, copiedUnitGroupId);
            
            if (hexagonData.StationaryUnitGroup == originalUnitGroupId)
                UpdateStationaryUnitGroupOfHexClientRpc(originalUnitCoordinates, true, copiedUnitGroupId);
        }

        public void DeleteUnitGroupFromHex(AxialCoordinates coordinate, UnitGroup unitGroupToRemove)
        {
            DeleteUnitGroupFromHexClientRpc(coordinate, unitGroupToRemove.NetworkObjectId);
        }

        public void RemoveUnitGroupFromHexCenter(AxialCoordinates coordinate, UnitGroup unitGroupToRemove)
        {
            RemoveUnitGroupFromHexCenterClientRpc(coordinate, unitGroupToRemove.NetworkObjectId);
        }

        public void MoveUnitGroupToHex(AxialCoordinates oldCoordinate, AxialCoordinates newCoordinate, UnitGroup unitGroupToMove)
        {
            MoveUnitGroupToHexClientRpc(oldCoordinate, newCoordinate, unitGroupToMove.NetworkObjectId);
        }

        public void MoveUnitGroupToHexCenter(AxialCoordinates coordinate, UnitGroup unitGroupToMove)
        {
            AddUnitGroupToHexCenterClientRpc(coordinate, unitGroupToMove.NetworkObjectId);
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

        public void RemoveStationaryUnitGroupFromHex(AxialCoordinates coordinate, UnitGroup unitGroupToRemove)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            
            if(hexagonData.StationaryUnitGroup == unitGroupToRemove.NetworkObjectId)
                UpdateStationaryUnitGroupOfHexClientRpc(coordinate, false, 0);
        }

        public void UpdateWarStateOnHex(AxialCoordinates coordinate, bool isWarGround)
        {
            UpdateWarStateOnHexClientRpc(coordinate, isWarGround);
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
            hexagonData.UnitsInHexCenter.Remove(unitGroupToRemove);
            if(hexagonData.StationaryUnitGroup != null && hexagonData.StationaryUnitGroup == unitGroupToRemove)
                hexagonData.StationaryUnitGroup = null;
            
            _coordinatesByUnitGroups.Remove(unitGroupToRemove);
        }
        
        [ClientRpc]
        private void RemoveUnitGroupFromHexCenterClientRpc(AxialCoordinates coordinate, ulong unitGroupToRemove)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            
            Debug.Assert(hexagonData.UnitsInHexCenter.Contains(unitGroupToRemove),
                "Tried to remove unit group from hex center that does not exist in hex center!");
            
            hexagonData.UnitsInHexCenter.Remove(unitGroupToRemove);
        }
        
        [ClientRpc]
        private void MoveUnitGroupToHexClientRpc(AxialCoordinates oldCoordinate, AxialCoordinates newCoordinate, ulong unitGroupToMove)
        {
            var dataOfOldHexagon = _hexDataByCoordinates[oldCoordinate];
            var dataOfNewHexagon = _hexDataByCoordinates[newCoordinate];
            
            Debug.Assert(dataOfOldHexagon.UnitsOnHex.Contains(unitGroupToMove),
                "Tried to move unit from hex that didn't contain it!");
            Debug.Assert(!dataOfNewHexagon.UnitsInHexCenter.Contains(unitGroupToMove),
                "Tried to move unit to hex that already did contain it!");

            dataOfOldHexagon.UnitsOnHex.Remove(unitGroupToMove);
            dataOfNewHexagon.UnitsOnHex.Add(unitGroupToMove);
            
            _coordinatesByUnitGroups[unitGroupToMove] = newCoordinate;
        }

        [ClientRpc]
        private void AddUnitGroupToHexCenterClientRpc(AxialCoordinates coordinate, ulong unitGroupToAdd)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            
            Debug.Assert(!hexagonData.UnitsInHexCenter.Contains(unitGroupToAdd),
                "Tried to add unit group to hex center that already contained it!");
            
            hexagonData.UnitsInHexCenter.Add(unitGroupToAdd);
            
            Debug.Assert(hexagonData.UnitsOnHex.Contains(unitGroupToAdd), "Tried to add unitGroup to center of hex which does not contain said unitGroup!");
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
                Debug.Assert(hexagonData.UnitsInHexCenter.Contains(newStationaryUnitGroup),
                    "Tried to add stationary unit group that does not exist in hexagon center!");
            }
        }

        [ClientRpc]
        private void UpdateWarStateOnHexClientRpc(AxialCoordinates coordinate, bool isWarGround)
        {
            var hexagonData = _hexDataByCoordinates[coordinate];
            hexagonData.IsWarGround = isWarGround;
        }

        #endregion
    }
}