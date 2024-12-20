using Networking.Host;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Testing.QuickSinglePlayerSetup
{
    public class TestSinglePlayerSetup : MonoBehaviour
    {
        [SerializeField] string playerName = "TestPlayer";

        private const string FollowUpSceneName = "QuickSinglePlayerFollowUp2";

        private async void Start()
        {
            Debug.Log("Starting SinglePlayerSetup");
            PlayerNameStorage.Name = playerName;
            await HostSingleton.Instance.GameManager.StartHostAsync(FollowUpSceneName);
        }
    }
}
