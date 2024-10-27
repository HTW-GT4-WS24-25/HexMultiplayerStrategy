using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Networking.Client;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ClientSingleton clientPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        var isDedicatedServer = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
        if (isDedicatedServer)
            await LaunchDedicatedServer();
        else
            await LaunchClient();
    }

    private async Task LaunchDedicatedServer()
    {

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
