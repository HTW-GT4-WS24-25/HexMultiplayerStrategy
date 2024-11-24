using HexSystem;
using Networking.Host;
using Player;
using Unit;
using Unity.Netcode;
using UnityEngine;

public class HexControlObserver : NetworkBehaviour
{
    [SerializeField] private GridData gridData; 
    [SerializeField] private MapBuilder mapBuilder;

    public void InitializeOnServer()
    {
        GameEvents.NETWORK_SERVER.OnHexControllerChanged += HandleHexControllerChanged;
        GameEvents.UNIT.OnUnitGroupReachedHexCenter += HandleUnitGroupReachedHexCenter;
    }

    #region Server

    private void HandleUnitGroupReachedHexCenter(UnitGroup unitGroup, AxialCoordinates hexCoordinates)
    {
        var hexagonData = gridData.GetHexagonDataOnCoordinate(hexCoordinates);
        var stationaryUnitGroupId = hexagonData.StationaryUnitGroup;
        
        if (stationaryUnitGroupId == null)
        {
            Debug.Log("Unit should become stationary");
            gridData.UpdateStationaryUnitGroupOfHex(hexCoordinates, unitGroup);
            HandleHexControllerChanged(hexagonData.Coordinates, unitGroup.PlayerId);
            return;
        }
        
        var stationaryUnitGroup = UnitGroup.UnitGroupsInGame[stationaryUnitGroupId.Value];

        if (stationaryUnitGroup.PlayerId != unitGroup.PlayerId)
        {
            Debug.Log("Initiating Combat in HexCenter");
            GameEvents.UNIT.OnCombatTriggered?.Invoke(stationaryUnitGroup, unitGroup);
        } 
        else if (!unitGroup.Movement.HasPath)
        {
            Debug.Log("Unit should be added to other stationary group");

            stationaryUnitGroup.AddUnits(unitGroup.UnitCount.Value);
            unitGroup.Delete();
        }
    }

    private void HandleHexControllerChanged(AxialCoordinates hexagonCoordinates, ulong playerId)
    {
        var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId);
        ChangeHexColorOnControlChangeClientRpc(hexagonCoordinates, (int)playerData.PlayerColorType);
        
        gridData.UpdateControllingPlayerOfHex(hexagonCoordinates, playerId);
    }

    #endregion

    #region Client

    [ClientRpc]
    private void ChangeHexColorOnControlChangeClientRpc(AxialCoordinates coordinates, int encodedColorType)
    {
        mapBuilder.Grid.Get(coordinates)
            .AdaptBorderToPlayerColor(PlayerColor.GetFromColorType(PlayerColor.IntToColorType(encodedColorType)));
    }

    #endregion
}