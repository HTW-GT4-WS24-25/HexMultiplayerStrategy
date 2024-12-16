using System.Collections.Generic;
using System.Linq;
using GameEvents;
using Networking.Server;
using Player;
using Unity.Netcode;
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

        public List<PlayerData> GetAllPlayerData()
        {
            return _playersByClientId.Select(keyValuePair => keyValuePair.Value).ToList();
        }

        public void SetPlayerColorType(ulong playerClientId, PlayerColor.ColorType newColorType)
        {
            _playersByClientId[playerClientId].PlayerColorType = newColorType;
        }

        public void SetPlayerFaction(ulong playerClientId, UnitModel.ModelType newFactionType)
        {
            _playersByClientId[playerClientId].PlayerUnitModelType = newFactionType;
        }

        public void SetPlayerScore(ulong playerClientId, int newScore)
        {
            Debug.Assert(_playersByClientId.ContainsKey(playerClientId));
            _playersByClientId[playerClientId].PlayerScore = newScore;
        }

        public void IncrementPlayerScore(ulong playerClientId, int points)
        {
            Debug.Assert(_playersByClientId.ContainsKey(playerClientId));
            _playersByClientId[playerClientId].PlayerScore += points;
        }

        public void RegisterNewPlayer(Player.Player newPlayer)
        {
            var playerClientId = newPlayer.OwnerClientId;
            var newPlayerData = new PlayerData
            {
                ClientId = playerClientId,
                PlayerName = _networkServer.GetPlayerDataByClientId(playerClientId).playerName,
                PlayerColorType = PlayerColor.ColorType.None,
            };
            Debug.Log($"Player {newPlayerData.PlayerName} registered!");
            
            _playersByClientId.Add(playerClientId, newPlayerData);
            ServerEvents.Player.OnPlayerConnected?.Invoke(playerClientId, newPlayerData.PlayerName);
        }
        
        public class PlayerData : INetworkSerializable
        {
            private int _playerScore;
            private PlayerColor.ColorType _playerColorType;
            
            public ulong ClientId;
            public string PlayerName;
            public PlayerMoney PlayerMoney = new();
            public UnitModel.ModelType PlayerUnitModelType;
            
            public PlayerColor.ColorType PlayerColorType
            {
                get => _playerColorType;
                set
                {
                    _playerColorType = value;
                    ServerEvents.Player.OnPlayerColorChanged?.Invoke(ClientId, (int)_playerColorType);
                }
            }
            
            public int PlayerScore
            {
                get => _playerScore;
                set
                {
                    Debug.Log($"Player {PlayerName}, new score: {value}, old score: {_playerScore}");
                    _playerScore = value;
                }
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _playerScore);
                serializer.SerializeValue(ref _playerColorType);
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref PlayerName);
            }
        }
    }
}