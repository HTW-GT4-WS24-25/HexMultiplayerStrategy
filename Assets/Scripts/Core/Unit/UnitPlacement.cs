﻿using Core.GameEvents;
using Core.HexSystem;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;

namespace Core.Unit
{
    public class UnitPlacement : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Group.UnitGroup unitGroupPrefab;

        private HexagonGrid _hexagonGrid;
        private GridData _gridData;

        public void Initialize(HexagonGrid hexagonGrid, GridData hexGridData)
        {
            _hexagonGrid = hexagonGrid;
            _gridData = hexGridData;
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
                ClientEvents.NightShop.OnUnitPlacementCommand += HandlePlacementCommand;
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
                ClientEvents.NightShop.OnUnitPlacementCommand -= HandlePlacementCommand;
        }
    
        #region Server

        [Rpc(SendTo.Server)]
        private void RequestPlacementRpc(AxialCoordinates coordinate, int unitAmount,  ulong playerId)
        {
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId);
            TryAddUnitsToHex(coordinate, playerData, unitAmount);
        }

        public bool TryAddUnitsToHex(AxialCoordinates coordinate, PlayerDataStorage.PlayerData playerData, int addAmount)
        {
            // TODO: Check if hex is owned by player and not at war
            var hexagonData = _gridData.GetHexagonDataOnCoordinate(coordinate);

            if (hexagonData.ControllerPlayerId != playerData.ClientId)
                return false;
        
            if (hexagonData.StationaryUnitGroup != null)
            {
                var stationaryUnitGroup = Group.UnitGroup.UnitGroupsInGame[hexagonData.StationaryUnitGroup.Value];
            
                stationaryUnitGroup.AddUnits(addAmount);
            }
            else
            {
                PlaceUnitGroupOnHex(coordinate, playerData.ClientId, addAmount);
            }

            return true;
        }

        private void PlaceUnitGroupOnHex(AxialCoordinates coordinate, ulong playerId, int unitAmount)
        {
            var hexagon = _hexagonGrid.Get(coordinate);
        
            var newUnitGroup = Instantiate(unitGroupPrefab, hexagon.transform.position, Quaternion.identity);
            var newUnitGroupNetworkObject = newUnitGroup.GetComponent<NetworkObject>();
            newUnitGroupNetworkObject.Spawn();
        
            newUnitGroup.InitializeOnHexCenter(unitAmount, playerId, hexagon);
        
            _gridData.PlaceUnitGroupOnHex(coordinate, newUnitGroup);
        }

        #endregion

        #region Client

        private void HandlePlacementCommand(AxialCoordinates coordinate, int unitAmount)
        {
            RequestPlacementRpc(coordinate, unitAmount, NetworkManager.Singleton.LocalClientId);
        }

        #endregion
    }
}