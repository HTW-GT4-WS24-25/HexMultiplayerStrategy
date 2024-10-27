using System.Threading.Tasks;
using UnityEngine;

namespace Networking.Client
{
    public class ClientSingleton : MonoBehaviour
    {
        public static ClientSingleton Instance;

        public ClientGameManager GameManager { get; private set; }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async Task<bool> CreateClient()
        {
            GameManager = new ClientGameManager();

            return await GameManager.InitAsync();
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}
