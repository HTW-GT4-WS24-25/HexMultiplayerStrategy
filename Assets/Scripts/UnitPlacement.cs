using HexSystem;
using Networking.Host;
using Unit;
using Unity.Netcode;
using UnityEngine;

public class UnitPlacement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private UnitGroup unitGroupPrefab;

    private HexagonGrid _hexagonGrid;
    private GridData _gridData;

    public void Initialize(HexagonGrid hexagonGrid, GridData hexGridData)
    {
        _hexagonGrid = hexagonGrid;
        _gridData = hexGridData;
    }

    #region Server

    [Rpc(SendTo.Server)]
    private void RequestUnitPlacementRpc(AxialCoordinates coordinate, int unitAmount, ulong playerId)
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
            var stationaryUnitGroup = UnitGroup.UnitGroupsInGame[hexagonData.StationaryUnitGroup.Value];
            
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
        
        newUnitGroup.Initialize(unitAmount, playerId, hexagon, _gridData);
        
        _gridData.PlaceUnitGroupOnHex(coordinate, newUnitGroup);
    }

    #endregion

    #region Client

    public void HandlePlacementCommand(AxialCoordinates coordinate, int unitAmount)
    {
        RequestUnitPlacementRpc(coordinate, unitAmount, NetworkManager.Singleton.LocalClientId);
    }

    #endregion
}