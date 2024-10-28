using System;
using System.Threading.Tasks;
using Networking.Client;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private HostSingleton hostPrefab;
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private PlayerColor[] playerColors;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            SetupPlayerColorsForGame();
            
            await LaunchClient();
        }

        private void SetupPlayerColorsForGame()
        {
            foreach (var playerColor in playerColors)
            {
                PlayerColor.AddColorToStorage(playerColor);
            }
        }

        private async Task LaunchClient()
        {
            var client = Instantiate(clientPrefab);
            var authenticated = await client.CreateClient();

            var host = Instantiate(hostPrefab);
            host.CreateHost();

            if(authenticated)
            {
                client.GameManager.GoToMenu();
            }
        }
    }
}
