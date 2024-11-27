using Networking.Host;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Testing.QuickSinglePlayerSetup
{
    public class TestSinglePlayerColorSetter : MonoBehaviour
    {
        [SerializeField] PlayerColor.ColorType playerColorType = PlayerColor.ColorType.Red;
        
        private const string GameSceneName = "MatchScene";
        
        void Start()
        {
            HostSingleton.Instance.GameManager.PlayerData.SetPlayerColorType(NetworkManager.Singleton.LocalClientId, playerColorType);
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
    }
}
