using System;
using System.Collections.Generic;
using Networking.Shared;
using Unity.Netcode;
using UnityEngine;

namespace Networking.Server
{
    public class NetworkServer : IDisposable
    {
        private readonly NetworkManager _networkManager;
        private readonly Dictionary<ulong, string> _authIdsByClientIds = new();
        private readonly Dictionary<string, PlayerIdentificationData> _playersByAuthIds = new();
    
        public NetworkServer(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.ConnectionApprovalCallback += ApprovalCheck;
            _networkManager.OnServerStarted += OnNetworkReady;
        }
    
        public void Dispose()
        {
            if (_networkManager != null)
            {
                _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
                _networkManager.OnServerStarted -= OnNetworkReady;
                _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            
                if(_networkManager.IsListening)
                    _networkManager.Shutdown();
            }
        }

        public PlayerIdentificationData GetPlayerDataByClientId(ulong clientId)
        {
            if (_authIdsByClientIds.TryGetValue(clientId, out var authId))
            {
                if (_playersByAuthIds.TryGetValue(authId, out var playerData))
                {
                    return playerData;
                }
            }

            return null;
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            var playerData = JsonUtility.FromJson<PlayerIdentificationData>(payload);
        
            _authIdsByClientIds[request.ClientNetworkId] = playerData.playerAuthId;
            _playersByAuthIds[playerData.playerAuthId] = playerData;

            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Position = Vector3.zero;
            response.Rotation = Quaternion.identity;
        }
    
        private void OnNetworkReady()
        {
            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (_authIdsByClientIds.TryGetValue(clientId, out var authId))
            {
                _authIdsByClientIds.Remove(clientId);
                _playersByAuthIds.Remove(authId);
            }
        }
    }
}
