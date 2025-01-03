using System.Collections.Generic;
using Core.GameEvents;
using Core.Player;
using Core.Unit.Model;
using Helper;
using Networking.Host;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.UI.Lobby
{
    public class LobbyUI : NetworkBehaviour
    {
        [SerializeField] private Transform playerEntryContainer;
        [SerializeField] private LobbyUIPlayerEntry playerEntryPrefab;
        [SerializeField] private JoinCodeText joinCodeText;
        [FormerlySerializedAs("colorSelectionUI")] [SerializeField] private PlayerConfigurationUI playerConfigurationUI;
        [SerializeField] private Button startGameButton;

        private const string MatchSceneName = "MatchScene";
        private const int MinimumPlayerCount = 1;
            
        private readonly List<LobbyUIPlayerEntry> _lobbyList = new();
        private readonly List<PlayerColor.ColorType> _unavailableColors = new();
        
        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                playerConfigurationUI.Initialize();
                
                RequestJoinCodeServerRPC(NetworkManager.Singleton.LocalClientId);
                RequestAllPlayerDataServerRPC(NetworkManager.Singleton.LocalClientId);
                
                if(IsHost)
                    startGameButton.gameObject.SetActive(true);
            }

            if (IsServer)
            {
                ServerEvents.Player.OnPlayerConnected += HandlePlayerConnected;
                ServerEvents.Player.OnPlayerColorChanged += HandlePlayerColorChangedClientRPC;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                ServerEvents.Player.OnPlayerConnected += HandlePlayerConnected;
                ServerEvents.Player.OnPlayerColorChanged += HandlePlayerColorChangedClientRPC;
            }
        }

        #region Server

        [Rpc(SendTo.Server)]
        private void RequestJoinCodeServerRPC(ulong requestClientId)
        {
            FixedString32Bytes joinCode = HostSingleton.Instance.GameManager.JoinCode;
            var clientRpcParams = HelperMethods.GetClientRpcParamsToSingleTarget(requestClientId);
            
            SetJoinCodeTextClientRPC(joinCode, clientRpcParams);
        }
        
        [Rpc(SendTo.Server)]
        private void RequestAllPlayerDataServerRPC(ulong requestClientId)
        {
            var playerList = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
            var clientRpcParams = HelperMethods.GetClientRpcParamsToSingleTarget(requestClientId);
            
            foreach (var playerData in playerList)
            {
                AddPlayerDataToLobbyListClientRPC(playerData.ClientId, playerData.Name, (int)playerData.PlayerColorType, clientRpcParams);    
            }
        }

        [Rpc(SendTo.Server)]
        private void RequestPlayerColorSelectionServerRPC(ulong requestClientId, PlayerColor.ColorType playerColorType, UnitModel.ModelType playerModelType)
        {
            if (!_unavailableColors.Contains(playerColorType))
            {
                var clientRpcParams = HelperMethods.GetClientRpcParamsToSingleTarget(requestClientId);
                HostSingleton.Instance.GameManager.PlayerData.SetPlayerColorType(requestClientId, playerColorType);
                HostSingleton.Instance.GameManager.PlayerData.SetPlayerFaction(requestClientId, playerModelType);
                
                OnPlayerConfigurationSuccessfulClientRPC(clientRpcParams);
            }
        }
        
        public static void StartMatch()
        {
            NetworkManager.Singleton.SceneManager.LoadScene(MatchSceneName, LoadSceneMode.Single);
        }

        private void HandlePlayerConnected(ulong clientId, FixedString32Bytes playerName)
        {
            AddPlayerDataToLobbyListClientRPC(clientId, playerName, (int)PlayerColor.ColorType.None);
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
            if (_lobbyList.Exists(entry => entry.ClientId == playerClientId))
                return;
            
            var colorType = PlayerColor.IntToColorType(encodedColorType);
            RegisterNewUnavailableColor(colorType);
            
            var playerColor = PlayerColor.GetFromColorType(colorType);
            
            var newLobbyEntry = Instantiate(playerEntryPrefab, playerEntryContainer);
            newLobbyEntry.Initialize(playerName.Value, playerColor.BaseColor, playerClientId);
            _lobbyList.Add(newLobbyEntry);
        }

        [ClientRpc]
        private void HandlePlayerColorChangedClientRPC(ulong playerClientId, int encodedColor)
        {
            var colorType = PlayerColor.IntToColorType(encodedColor);
            RegisterNewUnavailableColor(colorType);
            
            var playerColor = PlayerColor.GetFromColorType(colorType);
            
            var playerEntryToChange = _lobbyList.Find(entry => entry.ClientId == playerClientId);
            playerEntryToChange?.UpdatePlayerColor(playerColor.BaseColor);

            if (IsHost)
                startGameButton.interactable = _lobbyList.Count >= MinimumPlayerCount && _lobbyList.TrueForAll(player => player.ReadyToPlay);
        }

        [ClientRpc]
        private void OnPlayerConfigurationSuccessfulClientRPC(ClientRpcParams clientRpcParams)
        {
            playerConfigurationUI.gameObject.SetActive(false);
        }

        private void RegisterNewUnavailableColor(PlayerColor.ColorType colorType)
        {
            if (_unavailableColors.Contains(colorType)) 
                return;
            
            _unavailableColors.Add(colorType);
            playerConfigurationUI.SetUnavailableColors(_unavailableColors);
        }

        public void SubmitPlayerSelection(PlayerColor.ColorType colorType, UnitModel.ModelType modelType)
        {
            RequestPlayerColorSelectionServerRPC(NetworkManager.Singleton.LocalClientId, colorType, modelType);
        }

        #endregion
    }
}
