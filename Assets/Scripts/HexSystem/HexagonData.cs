using System.Collections.Generic;
using System.Linq;
using Unit;
using Unity.Netcode;
using UnityEngine;

namespace HexSystem
{
    public class HexagonData
    {
        public AxialCoordinates Coordinates { get; set; }
        public List<ulong> UnitsOnHex { get; } = new();
        public List<ulong> UnitsInHexCenter { get; set; } = new();
        public ulong? StationaryUnitGroup { get; set; }
        public bool IsWarGround { get; set; }
        public ulong? ControllerPlayerId { get; set; }

        public HexagonData(AxialCoordinates coordinates)
        {
            Coordinates = coordinates;
        }
    }

    public class GridData : NetworkBehaviour
    {
        private readonly Dictionary<AxialCoordinates, HexagonData> _gridData = new();
        
        public GridData(int nRings)
        {
            foreach (var coordinate in HexagonGrid.GetHexRingsAroundCoordinates(new AxialCoordinates(0, 0), nRings))
            {
                _gridData.Add(coordinate, new HexagonData(coordinate));       
            }
        }

        #region Server

        public void PlaceUnitGroupOnHex(AxialCoordinates coordinate, UnitGroup unitGroupToAdd) 
        {
            PlaceUnitGroupOnHexClientRpc(coordinate, unitGroupToAdd.NetworkObjectId);   
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
            MoveUnitGroupToHexCenterClientRpc(coordinate, unitGroupToMove.NetworkObjectId);
        }

        public void UpdateControllingPlayerOfHex(AxialCoordinates coordinate, ulong playerId)
        {
            UpdateControllingPlayerOfHexClientRpc(coordinate, playerId);
        }

        public void UpdateStationaryUnitGroupOfHex(AxialCoordinates coordinate, UnitGroup newStationaryUnitGroup)
        {
            UpdateStationaryUnitGroupOfHexClientRpc(coordinate, newStationaryUnitGroup?.NetworkObjectId);
        }

        public void UpdateWarStateOnHex(AxialCoordinates coordinate, bool isWarGround)
        {
            UpdateWarStateOnHexClientRpc(coordinate, isWarGround);
        }

        #endregion

        #region Client

        [ClientRpc]
        private void PlaceUnitGroupOnHexClientRpc(AxialCoordinates coordinate, ulong unitGroupToAdd)
        {
            var hexagonData = _gridData[coordinate];
            hexagonData.UnitsOnHex.Add(unitGroupToAdd);
            hexagonData.UnitsInHexCenter.Add(unitGroupToAdd);
        }
        
        [ClientRpc]
        private void DeleteUnitGroupFromHexClientRpc(AxialCoordinates coordinate, ulong unitGroupToRemove)
        {
            var hexagonData = _gridData[coordinate];
            hexagonData.UnitsOnHex.Remove(unitGroupToRemove);
            hexagonData.UnitsInHexCenter.Remove(unitGroupToRemove);
        }
        
        [ClientRpc]
        private void RemoveUnitGroupFromHexCenterClientRpc(AxialCoordinates coordinate, ulong unitGroupToRemove)
        {
            var hexagonData = _gridData[coordinate];
            hexagonData.UnitsInHexCenter.Remove(unitGroupToRemove);
            
            if (newStationaryUnitGroup != null)
            {
                Debug.Assert(hexagonData.UnitsOnHex.Contains(newStationaryUnitGroup.Value),
                    "Tried to add stationary unit group that does not exist on hexagon!");
                Debug.Assert(hexagonData.UnitsInHexCenter.Contains(newStationaryUnitGroup.Value),
                    "Tried to add stationary unit group that does not exist in hexagon center!");
            }
        }
        
        [ClientRpc]
        private void MoveUnitGroupToHexClientRpc(AxialCoordinates oldCoordinate, AxialCoordinates newCoordinate, ulong unitGroupToMove)
        {
            var dataOfOldHexagon = _gridData[oldCoordinate];
            var dataOfNewHexagon = _gridData[newCoordinate];

            dataOfOldHexagon.UnitsOnHex.Remove(unitGroupToMove);
            dataOfNewHexagon.UnitsOnHex.Add(unitGroupToMove);
            
            Debug.Assert(hexagonData.UnitsOnHex.Contains(newStationaryUnitGroup.Value),
                "Tried to add stationary unit group that does not exist on hexagon!");
            Debug.Assert(hexagonData.UnitsInHexCenter.Contains(newStationaryUnitGroup.Value),
                "Tried to add stationary unit group that does not exist in hexagon center!");
        }

        [ClientRpc]
        private void MoveUnitGroupToHexCenterClientRpc(AxialCoordinates coordinate, ulong unitGroupToMove)
        {
            var hexagonData = _gridData[coordinate];
            hexagonData.UnitsInHexCenter.Add(unitGroupToMove);
        }

        [ClientRpc]
        private void UpdateControllingPlayerOfHexClientRpc(AxialCoordinates coordinate, ulong playerId)
        {
            var hexagonData = _gridData[coordinate];
            hexagonData.ControllerPlayerId = playerId;
        }

        [ClientRpc]
        private void UpdateStationaryUnitGroupOfHexClientRpc(AxialCoordinates coordinate, ulong? newStationaryUnitGroup)
        {
            var hexagonData = _gridData[coordinate];
            hexagonData.StationaryUnitGroup = newStationaryUnitGroup;

            if (newStationaryUnitGroup != null)
            {
                Debug.Assert(hexagonData.UnitsOnHex.Contains(newStationaryUnitGroup.Value),
                    "Tried to add stationary unit group that does not exist on hexagon!");
                Debug.Assert(hexagonData.UnitsInHexCenter.Contains(newStationaryUnitGroup.Value),
                    "Tried to add stationary unit group that does not exist in hexagon center!");
            }
        }

        [ClientRpc]
        private void UpdateWarStateOnHexClientRpc(AxialCoordinates coordinate, bool isWarGround)
        {
            var hexagonData = _gridData[coordinate];
            hexagonData.IsWarGround = isWarGround;
        }

        #endregion
    }
}