using Core.PlayerData;
using Core.Unit.Model;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Testing.QuickSinglePlayerSetup
{
    public class TestSinglePlayerColorSetter : MonoBehaviour
    {
        [SerializeField] PlayerColor.ColorType playerColorType = PlayerColor.ColorType.Red;
        [SerializeField] UnitModel.ModelType modelType = UnitModel.ModelType.Rabbit;
        
        private const string GameSceneName = "MatchScene";
        
        void Start()
        {
            var localPlayer =
                HostSingleton.Instance.GameManager.GetPlayerByClientId(NetworkManager.Singleton.LocalClientId);
            localPlayer.PlayerColorType = playerColorType;
            localPlayer.UnitModelType = modelType;
            
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
    }
}
