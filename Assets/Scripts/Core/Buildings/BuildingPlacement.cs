using Core.GameEvents;
using Core.HexSystem;
using Core.HexSystem.Generation;
using Core.HexSystem.Hex;
using Unity.Netcode;
using UnityEngine;

namespace Core.Buildings
{
    public class BuildingPlacement : NetworkBehaviour
    {
        [SerializeField] private MapBuilder mapBuilder;
        [SerializeField] private ToppingCollection toppingCollection;
        
        private GridData _gridData;
        
        public void Initialize(GridData hexGridData)
        {
            _gridData = hexGridData;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient)
                return;
            
            ClientEvents.NightShop.OnBuildingPlacementCommand += HandleBuildingPlaceCommand;
            ClientEvents.NightShop.OnBuildingUpgradeCommand += HandleBuildingUpgradeCommand;
        }

        #region Server

        [Rpc(SendTo.Server)]
        private void RequestPlacementRpc(AxialCoordinates coordinate, BuildingType type)
        {
            var hexData = _gridData.GetHexagonDataOnCoordinate(coordinate);
            var existingBuilding = hexData.Building;
            
            Debug.Assert(existingBuilding == null);
            
            var newBuilding = BuildingFactory.Create(type);
            
            _gridData.SetBuildingOnHex(coordinate, newBuilding);
            SetBuildingToppingClientRpc(coordinate, hexData.HexType, newBuilding.Type);
        }
        
        [Rpc(SendTo.Server)]
        private void RequestUpgradeRpc(AxialCoordinates coordinate)
        {
            var hexData = _gridData.GetHexagonDataOnCoordinate(coordinate);
            var existingBuilding = hexData.Building;
            
            Debug.Assert(existingBuilding != null && existingBuilding.CanBeUpgraded);
            
            _gridData.UpgradeBuildingOnHex(coordinate);
            UpgradeBuildingToppingClientRpc(coordinate, existingBuilding.Level);
        }

        #endregion

        #region Client
        
        private void HandleBuildingPlaceCommand(AxialCoordinates coordinates, BuildingType buildingType)
        {
            RequestPlacementRpc(coordinates, buildingType);
        }
        
        private void HandleBuildingUpgradeCommand(AxialCoordinates coordinates)
        {
            RequestUpgradeRpc(coordinates);
        }

        [ClientRpc]
        private void SetBuildingToppingClientRpc(
            AxialCoordinates coordinate, 
            HexType hexType,
            BuildingType buildingType)
        {
            var toppingPrefab = toppingCollection.GetToppingPrefab(
                hexType, 
                buildingType);
         
            var hex = mapBuilder.Grid.Get(coordinate);
            hex.SetTopping(toppingPrefab);
        }

        [ClientRpc]
        private void UpgradeBuildingToppingClientRpc(AxialCoordinates coordinate, int newLevel)
        {
            var hex = mapBuilder.Grid.Get(coordinate);
            hex.SetToppingLevel(newLevel);
        }

        #endregion
    }
}