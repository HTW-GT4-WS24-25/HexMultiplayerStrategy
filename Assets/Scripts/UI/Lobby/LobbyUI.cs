using System.Collections.Generic;
using System.Linq;
using Player;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby
{
    public class LobbyUI : NetworkBehaviour
    {
        [SerializeField] private Transform playerEntryContainer;
        [SerializeField] private LobbyUIPlayerEntry playerEntryPrefab;
        [SerializeField] private JoinCodeText joinCodeText;
        [SerializeField] private ColorSelectionUI colorSelectionUI;
        [SerializeField] private Button startGameButton;

        private readonly List<LobbyUIPlayerEntry> _lobbyList = new();
        private List<PlayerColor.ColorType> _unavailableColors = new();
        
        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                colorSelectionUI.Initialize();
                
                RequestJoinCodeServerRPC(NetworkManager.Singleton.LocalClientId);
                RequestAllPlayerDataServerRPC(NetworkManager.Singleton.LocalClientId);
                
                if(IsHost)
                    startGameButton.gameObject.SetActive(true);
            }

            if (IsServer)
            {
                GameEvents.NETWORK_SERVER.OnPlayerConnected += (pId, pName) => AddPlayerDataToLobbyListClientRPC(pId, pName, (int)PlayerColor.ColorType.None);
                GameEvents.NETWORK_SERVER.OnPlayerColorChanged += HandlePlayerColorChangedClientRPC;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                GameEvents.NETWORK_SERVER.OnPlayerConnected -= (pId, pName) => AddPlayerDataToLobbyListClientRPC(pId, pName, (int)PlayerColor.ColorType.None);
                GameEvents.NETWORK_SERVER.OnPlayerColorChanged -= HandlePlayerColorChangedClientRPC;
            }
        }

        #region Server

        [Rpc(SendTo.Server)]
        private void RequestJoinCodeServerRPC(ulong requestClientId)
        {
            FixedString32Bytes joinCode = HostSingleton.Instance.GameManager.JoinCode;
            var clientRpcParams = CreateClientRpcParamsOneTarget(requestClientId);
            
            SetJoinCodeTextClientRPC(joinCode, clientRpcParams);
        }
        
        [Rpc(SendTo.Server)]
        private void RequestAllPlayerDataServerRPC(ulong requestClientId)
        {
            var playerList = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
            var clientRpcParams = CreateClientRpcParamsOneTarget(requestClientId);
            
            foreach (var playerData in playerList)
            {
                AddPlayerDataToLobbyListClientRPC(playerData.ClientId, playerData.PlayerName, (int)playerData.PlayerColorType, clientRpcParams);    
            }
        }

        [Rpc(SendTo.Server)]
        private void RequestPlayerColorSelectionServerRPC(ulong requestClientId, int encodedColor)
        {
            var colorType = PlayerColor.IntToColorType(encodedColor);

            if (!_unavailableColors.Contains(colorType))
            {
                var clientRpcParams = CreateClientRpcParamsOneTarget(requestClientId);
                HostSingleton.Instance.GameManager.PlayerData.SetPlayerColorType(requestClientId, colorType);
                
                OnColorSelectionSuccessfulClientRPC(clientRpcParams);
            }
        }

        private static ClientRpcParams CreateClientRpcParamsOneTarget(ulong clientId)
        {
            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { clientId }
                }
            };
        }

        #endregion

        #region Client

        [ClientRpc]
        private void SetJoinCodeTextClientRPC(FixedString32Bytes joinCode, ClientRpcParams clientRpcParams)
        {
            joinCodeText.SetJoinCode(joinCode.Value);
        }
        
        [ClientRpc]
        private void AddPlayerDataToLobbyListClientRPC(ulong playerClientId, FixedString32Bytes playerName, int encodedColorType, ClientRpcParams clientRpcParams = default)
        {
            if (_lobbyList.Any(entry => entry.ClientId == playerClientId))
                return;
            
            var colorType = PlayerColor.IntToColorType(encodedColorType);
            RegisterNewUnavailableColor(colorType);
            
            var playerColor = PlayerColor.GetFromColorType(colorType);
            
            var newLobbyEntry = Instantiate(playerEntryPrefab, playerEntryContainer);
            newLobbyEntry.Initialize(playerName.Value, playerColor.baseColor, playerClientId);
            _lobbyList.Add(newLobbyEntry);
        }

        [ClientRpc]
        private void HandlePlayerColorChangedClientRPC(ulong playerClientId, int encodedColor)
        {
            var colorType = PlayerColor.IntToColorType(encodedColor);
            RegisterNewUnavailableColor(colorType);
            
            var playerColor = PlayerColor.GetFromColorType(colorType);
            
            var playerEntryToChange = _lobbyList.FirstOrDefault(entry => entry.ClientId == playerClientId);
            playerEntryToChange?.UpdatePlayerColor(playerColor.baseColor);

            if (IsHost)
                startGameButton.interactable = _lobbyList.Count > 1 && _lobbyList.All(player => player.ReadyToPlay);
        }

        [ClientRpc]
        private void OnColorSelectionSuccessfulClientRPC(ClientRpcParams clientRpcParams)
        {
            colorSelectionUI.gameObject.SetActive(false);
        }

        private void RegisterNewUnavailableColor(PlayerColor.ColorType colorType)
        {
            if (_unavailableColors.Contains(colorType)) 
                return;
            
            _unavailableColors.Add(colorType);
            colorSelectionUI.SetUnavailableColors(_unavailableColors);
        }

        public void SubmitPlayerColorSelection(PlayerColor.ColorType colorType)
        {
            RequestPlayerColorSelectionServerRPC(NetworkManager.Singleton.LocalClientId, (int)colorType);
        }

        #endregion
    }
}
