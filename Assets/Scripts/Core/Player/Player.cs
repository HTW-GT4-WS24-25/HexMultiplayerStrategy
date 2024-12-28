using Networking.Host;
using Unity.Netcode;

namespace Core.Player
{
    public class Player : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            DontDestroyOnLoad(this);

            if (!IsServer) 
                return;
            
            var playerData = HostSingleton.Instance.GameManager.NetworkServer.GetPlayerDataByClientId(OwnerClientId);
            gameObject.name = playerData.playerName;
                
            HostSingleton.Instance.GameManager.PlayerData.RegisterNewPlayer(this);
        }
    }
}
