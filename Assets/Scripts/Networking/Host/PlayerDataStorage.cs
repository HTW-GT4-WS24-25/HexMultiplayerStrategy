using System.Collections.Generic;
using System.Linq;
using Core.GameEvents;
using Core.Player;
using Core.Unit.Model;
using Networking.Server;
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
            _playersByClientId[playerClientId].UnitModelType = newFactionType;
        }

        public void SetPlayerScore(ulong playerClientId, int newScore)
        {
            Debug.Assert(_playersByClientId.ContainsKey(playerClientId));
            _playersByClientId[playerClientId].Score = newScore;
        }

        public void IncrementPlayerScore(ulong playerClientId, int points)
        {
            Debug.Assert(_playersByClientId.ContainsKey(playerClientId));
            _playersByClientId[playerClientId].Score += points;
        }

        public void RegisterNewPlayer(Player newPlayer)
        {
            var playerClientId = newPlayer.OwnerClientId;
            var newPlayerData = new PlayerData
            {
                ClientId = playerClientId,
                Name = _networkServer.GetPlayerDataByClientId(playerClientId).playerName,
                PlayerColorType = PlayerColor.ColorType.None,
            };
            Debug.Log($"Player {newPlayerData.Name} registered!");
            
            _playersByClientId.Add(playerClientId, newPlayerData);
            ServerEvents.Player.OnPlayerConnected?.Invoke(playerClientId, newPlayerData.Name);
        }
        
        public class PlayerData : INetworkSerializable
        {
            private int _score;
            private PlayerColor.ColorType _playerColorType;
            
            public ulong ClientId;
            public string Name;
            public PlayerMoney Money = new();
            public UnitModel.ModelType UnitModelType;
            
            public PlayerColor.ColorType PlayerColorType
            {
                get => _playerColorType;
                set
                {
                    _playerColorType = value;
                    ServerEvents.Player.OnPlayerColorChanged?.Invoke(ClientId, (int)_playerColorType);
                }
            }
            
            public int Score
            {
                get => _score;
                set
                {
                    Debug.Log($"Player {Name}, new score: {value}, old score: {_score}");
                    _score = value;
                }
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _score);
                serializer.SerializeValue(ref _playerColorType);
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref Name);
            }
        }
    }
}