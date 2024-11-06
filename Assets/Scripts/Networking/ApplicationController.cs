using Networking.Client;
using Networking.Host;
using Player;
using UnityEngine;

namespace Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private HostSingleton hostPrefab;
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private PlayerColor[] playerColors;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SetupPlayerColorsForGame();
            
            LaunchClient();
        }

        private void SetupPlayerColorsForGame()
        {
            foreach (var playerColor in playerColors)
            {
                PlayerColor.AddColorToStorage(playerColor);
            }
        }

        private void LaunchClient()
        {
            var client = Instantiate(clientPrefab);
            var authenticated = client.CreateClient().GetAwaiter().GetResult();

            var host = Instantiate(hostPrefab);
            host.CreateHost();

            if(authenticated)
            {
                client.GameManager.GoToMenu();
            }
        }
    }
}
