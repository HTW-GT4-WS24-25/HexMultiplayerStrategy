using GameEvents;
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
        ServerEvents.Player.OnInitialPlayerUnitsPlaced += HandleHexControllerChanged;
        ServerEvents.Unit.OnUnitGroupReachedHexCenter += HandleUnitGroupReachedHexCenter;
    }

    #region Server

    private void HandleUnitGroupReachedHexCenter(UnitGroup unitGroup, AxialCoordinates hexCoordinates)
    {
        var hexagonData = gridData.GetHexagonDataOnCoordinate(hexCoordinates);
        Debug.Log("UnitGroupReachedHexCenter");
        if (hexagonData.StationaryUnitGroup == null)
        {
            Debug.Log("Unit should become stationary");
            gridData.UpdateStationaryUnitGroupOfHex(hexCoordinates, unitGroup);
            if(gridData.GetHexagonDataOnCoordinate(hexCoordinates).ControllerPlayerId != unitGroup.PlayerId)
                HandleHexControllerChanged(hexagonData.Coordinates, unitGroup.PlayerId);
            
            return;
        }

        var stationaryUnitGroup = UnitGroup.UnitGroupsInGame[hexagonData.StationaryUnitGroup.Value];
        
        if (stationaryUnitGroup.PlayerId != unitGroup.PlayerId)
        {
            Debug.Log("Initiating Combat in HexCenter");
            ServerEvents.Unit.OnCombatTriggered?.Invoke(stationaryUnitGroup, unitGroup);
        } 
        else if (!unitGroup.Movement.hasMovementLeft)
        {
            Debug.Log("Unit should be added to other stationary group");

            stationaryUnitGroup.IntegrateUnitsOf(unitGroup);
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