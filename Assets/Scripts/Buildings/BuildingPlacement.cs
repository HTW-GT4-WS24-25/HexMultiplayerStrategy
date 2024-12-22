using System.Collections.Generic;
using GameEvents;
using HexSystem;
using Unity.Netcode;
using UnityEngine;

namespace Buildings
{
    public class BuildingPlacement : NetworkBehaviour
    {
        [SerializeField] private ToppingCollection toppingCollection;

        private HexagonGrid _hexagonGrid;
        private GridData _gridData;
        
        public void Initialize(HexagonGrid hexagonGrid, GridData hexGridData)
        {
            _hexagonGrid = hexagonGrid; 
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
            
            hexData.Building = newBuilding;
            SetBuildingToppingClientRpc(coordinate, hexData.HexType, newBuilding.Type, newBuilding.Level);
        }
        
        [Rpc(SendTo.Server)]
        private void RequestUpgradeRpc(AxialCoordinates coordinate)
        {
            var hexData = _gridData.GetHexagonDataOnCoordinate(coordinate);
            var existingBuilding = hexData.Building;
            
            Debug.Assert(existingBuilding != null && existingBuilding.CanBeUpgraded);
            
            hexData.Building.Upgrade();
            
            SetBuildingToppingClientRpc(coordinate, hexData.HexType, existingBuilding.Type, existingBuilding.Level);
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
            BuildingType buildingType, 
            int buildingLevel)
        {
            var buildingPrefab = toppingCollection.GetToppingPrefab(
                hexType, 
                buildingType, 
                buildingLevel);
         
            var hex = _hexagonGrid.Get(coordinate);
            hex.SetTopping(buildingPrefab);
        }

        #endregion
    }
}