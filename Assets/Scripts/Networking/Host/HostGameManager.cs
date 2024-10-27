using System;
using System.Threading.Tasks;
using Networking.Server;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Host
{
    public class HostGameManager : IDisposable
    {
        public NetworkServer NetworkServer { get; private set; }
    
        private const int MaxConnections = 20;
        private const string GameSceneName = "Game";
        private const string PlayerNameKey = "PlayerName";
    
        private Allocation _allocation;
        private string _joinCode;
    
        public void Dispose()
        {
            NetworkServer?.Dispose();   
        }

        public async Task StartHostAsync()
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
                _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                Debug.Log($"JoinCode: {_joinCode}");
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayServerData = new RelayServerData(_allocation, "dtls");
            transport.SetRelayServerData(relayServerData);
        
            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
    }
}