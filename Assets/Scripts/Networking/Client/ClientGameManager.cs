using System;
using System.Threading.Tasks;
using Core.PlayerData;
using Networking.Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class ClientGameManager : IDisposable
    {
        private JoinAllocation _joinAllocation;
        private NetworkClient _networkClient;
        
        private const string MenuSceneName = "Menu";
    
        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);

            var authState = await AuthenticationWrapper.DoAuthentication();

            if(authState == AuthState.Authenticated)
            {
                return true;
            }

            return false;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        public async Task StartClientAsync(string joinCode)
        {
            try
            {
                _joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayServerData = new RelayServerData(_joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            var playerData = new PlayerIdentificationData()
            {
                playerName = PlayerNameStorage.Name,
                playerAuthId = AuthenticationService.Instance.PlayerId
            };

            var payload = JsonUtility.ToJson(playerData);
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
        
            NetworkManager.Singleton.StartClient();
        }
    
        public void Dispose()
        {
            _networkClient?.Dispose();
        }
    }
}
