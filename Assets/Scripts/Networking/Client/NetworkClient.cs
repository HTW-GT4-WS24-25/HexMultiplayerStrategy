using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class NetworkClient : IDisposable
    {
        private const string MenuSceneName = "Menu";
        private NetworkManager _networkManager;
    
        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }
    
        public void Dispose()
        {
            if (_networkManager != null)
            {
                _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if(clientId != 0 && clientId != _networkManager.LocalClientId)
                return;

            if (SceneManager.GetActiveScene().name != MenuSceneName)
            {
                SceneManager.LoadScene(MenuSceneName);
            }

            if (_networkManager.IsConnectedClient)
            {
                _networkManager.Shutdown();
            }
        }
    }
}
