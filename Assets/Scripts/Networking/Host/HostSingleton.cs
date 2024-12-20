using UnityEngine;

namespace Networking.Host
{
    public class HostSingleton : MonoBehaviour
    {
        public static HostSingleton Instance;

        public HostGameManager GameManager { get; private set; }

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

        public void CreateHost()
        {
            GameManager = new HostGameManager();
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}
