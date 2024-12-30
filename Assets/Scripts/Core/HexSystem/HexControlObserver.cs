using Core.GameEvents;
using Core.HexSystem.Generation;
using Core.PlayerData;
using Core.Unit.Group;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;

namespace Core.HexSystem
{
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
            if (hexagonData.StationaryUnitGroup == null)
            {
                gridData.UpdateStationaryUnitGroupOfHex(hexCoordinates, unitGroup);
                if(gridData.GetHexagonDataOnCoordinate(hexCoordinates).ControllerPlayerId != unitGroup.PlayerId)
                    HandleHexControllerChanged(hexagonData.Coordinates, unitGroup.PlayerId);
            
                return;
            }

            var stationaryUnitGroup = UnitGroup.UnitGroupsInGame[hexagonData.StationaryUnitGroup.Value];
        
            if (stationaryUnitGroup.PlayerId != unitGroup.PlayerId)
            {
                ServerEvents.Unit.OnCombatTriggered?.Invoke(stationaryUnitGroup, unitGroup);
            } 
            else if (!unitGroup.Movement.HasMovementLeft)
            {
                stationaryUnitGroup.IntegrateUnitsOf(unitGroup);
            }
        }

        private void HandleHexControllerChanged(AxialCoordinates hexagonCoordinates, ulong playerId)
        {
            var player = HostSingleton.Instance.GameManager.GetPlayerByClientId(playerId);
            ChangeHexColorOnControlChangeClientRpc(hexagonCoordinates, player.PlayerColorType);
            
            var hexagonData = gridData.GetHexagonDataOnCoordinate(hexagonCoordinates);
            if (hexagonData.ControllerPlayerId.HasValue)
            {
                var previousControllingPlayer = HostSingleton.Instance.GameManager.GetPlayerByClientId(hexagonData.ControllerPlayerId.Value);
                previousControllingPlayer.OnLostControlOfHex(hexagonData);
            }
            player.OnGainedControlOfHex(hexagonData);
            
            gridData.UpdateControllingPlayerOfHex(hexagonCoordinates, playerId);
        }

        #endregion

        #region Client

        [ClientRpc]
        private void ChangeHexColorOnControlChangeClientRpc(AxialCoordinates coordinates, PlayerColor.ColorType playerColorType)
        {
            mapBuilder.Grid.Get(coordinates)
                .AdaptBorderToPlayerColor(PlayerColor.GetFromColorType(playerColorType));
        }

        #endregion
    }
}