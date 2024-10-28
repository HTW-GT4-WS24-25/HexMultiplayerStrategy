using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace UI
{
    public class LobbyUI : NetworkBehaviour
    {
        [SerializeField] private Transform playerEntryContainer;
        [SerializeField] private LobbyUIPlayerEntry playerEntryPrefab;
        [SerializeField] private TextMeshProUGUI joinCodeText;

        private readonly List<LobbyUIPlayerEntry> _lobbyList = new();
        
        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                RequestJoinCodeServerRPC(NetworkManager.Singleton.LocalClientId);
                RequestAllPlayerDataRPC(NetworkManager.Singleton.LocalClientId);
            }

            if (IsServer)
            {
                GameEvents.NETWORK_SERVER.OnPlayerConnected += AddDataOnPlayerConnectedClientRPC;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                GameEvents.NETWORK_SERVER.OnPlayerConnected -= AddDataOnPlayerConnectedClientRPC;
            }
        }

        [Rpc(SendTo.Server)]
        private void RequestJoinCodeServerRPC(ulong requestClientId)
        {
            FixedString32Bytes joinCode = HostSingleton.Instance.GameManager.JoinCode;
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { requestClientId }
                }
            };
            
            SetJoinCodeTextClientRPC(joinCode, clientRpcParams);
        }

        [ClientRpc]
        private void SetJoinCodeTextClientRPC(FixedString32Bytes joinCode, ClientRpcParams clientRpcParams)
        {
            joinCodeText.text = $"Code: {joinCode.Value}";
        }

        [Rpc(SendTo.Server)]
        private void RequestAllPlayerDataRPC(ulong requestClientId)
        {
            var playerList = HostSingleton.Instance.GameManager.PlayerData.GetPlayerList();
            foreach (var playerData in playerList)
            {
                AddDataToListAfterRequestRPC(playerData.ClientId, playerData.PlayerName, RpcTarget.Single(requestClientId, RpcTargetUse.Temp));    
            }
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void AddDataToListAfterRequestRPC(ulong playerClientId, FixedString32Bytes playerName, RpcParams rpcParams)
        {
            AddPlayerDataToList(playerClientId, playerName);
        }
        
        [ClientRpc]
        private void AddDataOnPlayerConnectedClientRPC(ulong playerClientId, FixedString32Bytes playerName)
        {
            AddPlayerDataToList(playerClientId, playerName);
        }

        private void AddPlayerDataToList(ulong playerClientId, FixedString32Bytes playerName)
        {
            if (_lobbyList.Any(entry => entry.ClientId == playerClientId))
                return;
            
            var newLobbyEntry = Instantiate(playerEntryPrefab, playerEntryContainer);
            newLobbyEntry.Initialize(playerName.Value, playerClientId);
            _lobbyList.Add(newLobbyEntry);
        }
    }
}
