using System.Threading.Tasks;
using Core.Player;
using Core.Unit.Model;
using Networking.Client;
using Networking.Host;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private HostSingleton hostPrefab;
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private PlayerColor[] playerColors;
        [SerializeField] private UnitModel[] unitModelPrefabs;
        [SerializeField] private string sceneNameAfterInitialization = "Menu";

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SetupPlayerColorsForGame();
            SetupUnitModelsForGame();
            
            LaunchClient();
        }

        private void SetupPlayerColorsForGame()
        {
            foreach (var playerColor in playerColors)
            {
                PlayerColor.AddColorToStorage(playerColor);
            }
        }

        private void SetupUnitModelsForGame()
        {
            foreach (var unitModelPrefab in unitModelPrefabs)
            {
                UnitModel.AddModelPrefabToStorage(unitModelPrefab);
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
