using System.Collections.Generic;
using HexSystem;
using Networking.Host;
using Player;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Unit
{
    public class UnitGroup : NetworkBehaviour
    {
        [Header("References")]
        [field: SerializeField] public UnitGroupMovement Movement { get; private set; }
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private MeshRenderer meshRenderer;

        public UnityEvent OnUnitHighlightEnabled;
        public UnityEvent OnUnitHighlightDisabled;

        public static Dictionary<ulong, UnitGroup> UnitGroupsInGame = new();
        
        public ulong PlayerId { get; private set; }
        public ServerHexagon ServerHexagon { get; set; }
        public NetworkVariable<int> UnitCount { get; private set; } = new(0);
        public PlayerColor PlayerColor { get; private set; }

        public override void OnNetworkSpawn()
        {
            UnitCount.OnValueChanged += HandleUnitCountChanged;
            
            UnitGroupsInGame.Add(NetworkObjectId, this);
        }

        public override void OnNetworkDespawn()
        {
            UnitGroupsInGame.Remove(NetworkObjectId);
        }

        #region Server

        public void Initialize(ServerHexagon serverHexagon, int unitCount, ulong playerId)
        {
            PlayerId = playerId;
            
            PlaceOnHex(serverHexagon);
            UnitCount.Value = unitCount;
            
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId);
            InitializeClientRpc(playerId, (int)playerData.PlayerColorType);
            Movement.NextHexagon = serverHexagon;
        }
        
        public void AddUnits(int amount)
        {
            UnitCount.Value += amount;
        }

        public void SubtractUnits(int amount)
        {
            UnitCount.Value -= amount;
        }
        
        public void Delete()
        {
            ServerHexagon.UnitGroups.Remove(this);
            Destroy(gameObject);
        }
        
        public void PlaceOnHex(ServerHexagon serverHexagon)
        {
            ServerHexagon = serverHexagon;
            serverHexagon.UnitGroups.Add(this);
        }

        public void SetAsSelectedForControllingPlayer()
        {
            SetAsSelectedClientRpc();
        }

        #endregion

        #region Client

        [ClientRpc]
        private void InitializeClientRpc(ulong playerId, int encodedPlayerColorType)
        {
            PlayerId = playerId;
            
            PlayerColor = PlayerColor.GetFromColorType(PlayerColor.IntToColorType(encodedPlayerColorType));
            meshRenderer.material = PlayerColor.unitMaterial;
            
            Movement.Initialize(PlayerColor);
        }
        
        [ClientRpc]
        private void SetAsSelectedClientRpc()
        {
            if(NetworkManager.Singleton.LocalClientId != PlayerId)
                return;
            
            GameEvents.UNIT.OnUnitGroupSelected?.Invoke(this);
        }
        
        private void HandleUnitCountChanged(int previousvalue, int newvalue)
        {
            unitCountText.text = newvalue.ToString();
        }
        
        public void EnableHighlight()
        {
            meshRenderer.material = PlayerColor.highlightedUnitMaterial;
            OnUnitHighlightEnabled?.Invoke();
        }

        public void DisableHighlight()
        {
            meshRenderer.material = PlayerColor.unitMaterial;
            OnUnitHighlightDisabled?.Invoke();
        }
        
        #endregion
    }
}
