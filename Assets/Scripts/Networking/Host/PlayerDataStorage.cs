using System.Collections.Generic;
using System.Linq;
using Networking.Server;
using Player;
using UnityEngine;

namespace Networking.Host
{
    public class PlayerDataStorage
    {
        private readonly Dictionary<ulong, PlayerData> _playersByClientId = new ();
        private NetworkServer _networkServer;

        public PlayerDataStorage(NetworkServer networkServer)
        {
            _networkServer = networkServer;
        }

        public List<PlayerData> GetPlayerList()
        {
            return _playersByClientId.Select(keyValuePair => keyValuePair.Value).ToList();
        }

        public PlayerData GetPlayerById(ulong playerId)
        {
            return _playersByClientId[playerId];
        }

        public void SetPlayerColorType(ulong playerClientId, PlayerColor.ColorType newColorType)
        {
            _playersByClientId[playerClientId].PlayerColorType = newColorType;
            GameEvents.NETWORK_SERVER.OnPlayerColorChanged?.Invoke(playerClientId, (int)newColorType);
        }

        public void RegisterNewPlayer(Player.Player newPlayer)
        {
            var playerClientId = newPlayer.OwnerClientId;
            var newPlayerData = new PlayerData
            {
                ClientId = playerClientId,
                PlayerName = _networkServer.GetPlayerDataByClientId(playerClientId).playerName,
                PlayerColorType = PlayerColor.ColorType.None,
                PlayerGameObject = newPlayer
            };
            Debug.Log($"Player {newPlayerData.PlayerName} registered!");
            
            _playersByClientId.Add(playerClientId, newPlayerData);
            GameEvents.NETWORK_SERVER.OnPlayerConnected?.Invoke(playerClientId, newPlayerData.PlayerName);
        }
        
        public class PlayerData
        {
            public ulong ClientId;
            public string PlayerName;
            public PlayerColor.ColorType PlayerColorType;
            public Player.Player PlayerGameObject;
        }
    }
}