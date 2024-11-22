using System;
using System.Text;
using System.Threading.Tasks;
using Networking.Server;
using Networking.Shared;
using Player;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Host
{
    public class HostGameManager : IDisposable
    {
        public NetworkServer NetworkServer { get; private set; }
        public PlayerDataStorage PlayerData { get; private set; }
        public string JoinCode { get; private set; }
    
        private const int MaxConnections = 20;
        private const string LobbySceneName = "Lobby";
    
        private Allocation _allocation;
    
        public void Dispose()
        {
            NetworkServer?.Dispose();   
        }

        public async Task StartHostAsync(string hostSceneName = LobbySceneName)
        {
            try
            {
                _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            try
            {
                JoinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                Debug.Log($"JoinCode: {JoinCode}");
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayServerData = new RelayServerData(_allocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkServer = new NetworkServer(NetworkManager.Singleton);
            PlayerData = new PlayerDataStorage(NetworkServer);

            var playerData = new PlayerIdentificationData
            {
                playerName = PlayerNameStorage.Name,
                playerAuthId = AuthenticationService.Instance.PlayerId
            };
            var payload = JsonUtility.ToJson(playerData);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        
            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene(hostSceneName, LoadSceneMode.Single);
        }
    }
}