using System;
using System.Threading.Tasks;
using Networking.Client;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private HostSingleton hostPrefab;
        [SerializeField] private ClientSingleton clientPrefab;

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);

            await LaunchClient();
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
