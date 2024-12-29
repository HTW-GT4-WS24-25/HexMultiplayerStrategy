using Core.GameEvents;
using Core.PlayerData;
using Core.Unit.Model;
using UnityEngine;

namespace Core
{
    public class Player
    {
        public readonly ulong ClientId;
        public readonly string Name;
        public readonly PlayerMoney Money = new();
        public PlayerColor.ColorType PlayerColorType
        {
            get => _playerColorType;
            set
            {
                _playerColorType = value;
                ServerEvents.Player.OnPlayerColorChanged?.Invoke(ClientId, (int)_playerColorType);
            }
        }
            
        public int Score { get; set; }
        public UnitModel.ModelType UnitModelType;
        
        private PlayerColor.ColorType _playerColorType;
        private int _score;
        
        public Player(ulong clientId, string name)
        {
            ClientId = clientId;
            Name = name;
        }

        public void IncrementScore(int amount)
        {
            Score += amount;
        }
    }
}
