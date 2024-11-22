using System.Threading.Tasks;
using Networking.Client;
using Networking.Host;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private HostSingleton hostPrefab;
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private PlayerColor[] playerColors;
        [SerializeField] private string sceneNameAfterInitialization = "Menu";

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

        private async Task LaunchClient()
        {
            var client = Instantiate(clientPrefab);
            var authenticated = await client.CreateClient();

            var host = Instantiate(hostPrefab);
            host.CreateHost();

            if(authenticated)
            {
                SceneManager.LoadScene(sceneNameAfterInitialization);
            }
        }
    }
}
