using Core.GameEvents;
using Core.HexSystem;
using Core.HexSystem.Hex;
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

            if (IsServer)
                ServerEvents.Unit.OnUnitsShouldBeSpawnedOnBarrackHex += SpawnUnitsOnBarrackHex;
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
                ClientEvents.NightShop.OnUnitPlacementCommand -= HandlePlacementCommand;
            
            if (IsServer)
                ServerEvents.Unit.OnUnitsShouldBeSpawnedOnBarrackHex -= SpawnUnitsOnBarrackHex;
        }
    
        #region Server

        [Rpc(SendTo.Server)]
        private void RequestPlacementRpc(AxialCoordinates coordinate, int unitAmount,  ulong playerId)
        {
            var player = HostSingleton.Instance.GameManager.GetPlayerByClientId(playerId);
            TryAddUnitsToHex(coordinate, player, unitAmount);
        }

        public bool TryAddUnitsToHex(AxialCoordinates coordinate, Player player, int addAmount)
        {
            // TODO: Check if hex is owned by player and not at war
            var hexagonData = _gridData.GetHexagonDataOnCoordinate(coordinate);

            if (hexagonData.ControllerPlayerId != player.ClientId)
                return false;
        
            if (hexagonData.StationaryUnitGroup != null)
            {
                var stationaryUnitGroup = Group.UnitGroup.UnitGroupsInGame[hexagonData.StationaryUnitGroup.Value];
            
                stationaryUnitGroup.AddUnits(addAmount);
            }
            else
            {
                PlaceUnitGroupOnHex(coordinate, player.ClientId, addAmount);
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
        
        private void SpawnUnitsOnBarrackHex(HexagonData hexagon, int unitAmount)
        {
            var controllingPlayer =
                HostSingleton.Instance.GameManager.GetPlayerByClientId(hexagon.ControllerPlayerId!.Value);
            
            TryAddUnitsToHex(hexagon.Coordinates, controllingPlayer, unitAmount);
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