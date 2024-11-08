using Networking.Host;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace HexSystem
{
    public class HexControlSystem : NetworkBehaviour
    {
        [SerializeField] private MapBuilder mapBuilder;
        
        public void Initialize()
        {
            GameEvents.NETWORK_SERVER.OnHexChangedController += HandleHexChangedController;
        }
        
        #region Server

        private void HandleHexChangedController(AxialCoordinates coordinates, ulong controllerId)
        {
            var serverHexagon = mapBuilder.Grid.Get(coordinates).GetComponent<ServerHexagon>();
            serverHexagon.ControllingPlayerId = controllerId;
            
            var controllerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(controllerId);
            ChangeHexBorderColorClientRpc(coordinates, (int)controllerData.PlayerColorType);
        }

        #endregion

        #region Client

        [ClientRpc]
        private void ChangeHexBorderColorClientRpc(AxialCoordinates coordinates, int encodedColorType)
        {
            var colorType = PlayerColor.IntToColorType(encodedColorType);
            var color = PlayerColor.GetFromColorType(colorType);
            
            mapBuilder.Grid.Get(coordinates).AdaptBorderToPlayerColor(color);
        }

        #endregion
    }
}