using HexSystem;
using Networking.Host;
using Player;
using Unit;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class HexControlAndCombatObserver : NetworkBehaviour
{
    [SerializeField] private GridData gridData; 
    [SerializeField] private MapBuilder mapBuilder;

    public void InitializeOnServer()
    {
        GameEvents.UNIT.OnUnitEnteredHexCenterArea += HandleUnitEnteredHexCenterArea;
        GameEvents.UNIT.OnUnitLeftHexCenterArea += HandleUnitLeftHexCenterArea;
        GameEvents.NETWORK_SERVER.OnHexControllerChanged += HandleHexControllerChanged;
    }

    #region Server

    private void HandleUnitEnteredHexCenterArea(Hexagon hexagon, UnitGroup unitGroup)
    {
        gridData.MoveUnitGroupToHexCenter(hexagon.Coordinates, unitGroup);
        
        var hexagonData = gridData.GetHexagonDataOnCoordinate(hexagon.Coordinates);
        if (hexagonData.IsWarGround)
        {
            // TODO: Implement combat
        }
        else if (gridData.DoesHexCenterContainUnitsFromOtherPlayers(hexagonData.Coordinates, unitGroup.PlayerId))
        {
            // TODO: Start combat
        }
        else if (hexagonData.ControllerPlayerId != unitGroup.PlayerId)
        {
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(unitGroup.PlayerId);
            HandleHexControllerChanged(hexagon, playerData);
        }
    }
    
    private void HandleUnitLeftHexCenterArea(Hexagon hexagon, UnitGroup unitGroup)
    {
        gridData.RemoveUnitGroupFromHexCenter(hexagon.Coordinates, unitGroup);
    }

    private void HandleHexControllerChanged(Hexagon hexagon, PlayerDataStorage.PlayerData newControllerData)
    {
        ChangeHexColorOnControlChangeClientRpc(hexagon.Coordinates, (int)newControllerData.PlayerColorType);
        
        gridData.UpdateControllingPlayerOfHex(hexagon.Coordinates, newControllerData.ClientId);
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