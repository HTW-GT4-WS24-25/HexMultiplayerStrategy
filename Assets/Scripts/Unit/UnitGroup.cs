using System;
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
        [SerializeField] private UnitGroupCombatInitiator combatInitiator;

        public UnityEvent OnUnitHighlightEnabled;
        public UnityEvent OnUnitHighlightDisabled;
        public UnityEvent OnDamageTaken;

        public Action<int> OnUnitCountUpdated;

        public static Dictionary<ulong, UnitGroup> UnitGroupsInGame = new();
        
        public ulong PlayerId { get; private set; }
        public NetworkVariable<int> UnitCount { get; private set; } = new(0);
        public PlayerColor PlayerColor { get; private set; }

        public override void OnNetworkSpawn()
        {
            UnitCount.OnValueChanged += HandleUnitCountChanged;
            UnitGroupsInGame.Add(NetworkObjectId, this);
            if (!IsServer)
                Destroy(combatInitiator.gameObject);
        }

        public override void OnNetworkDespawn()
        {
            UnitGroupsInGame.Remove(NetworkObjectId);
        }

        #region Server

        public void Initialize(int unitCount, ulong playerId, Hexagon startHexagon, GridData gridData)
        {
            PlayerId = playerId;
            
            UnitCount.Value = unitCount;
            
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId);
            InitializeClientRpc(playerId, (int)playerData.PlayerColorType);
            Movement.NextHexagon = startHexagon;
            Movement.GridData = gridData;
        }
        
        public void AddUnits(int amount)
        {
            UnitCount.Value += amount;
        }

        public void TakeDamage(int damageAmount)
        {
            SubtractUnits(damageAmount);
            TriggerOnDamageTakenClientRpc();
        }
        
        public void Delete()
        {
            GameEvents.UNIT.OnUnitGroupDeleted.Invoke(this);
            Destroy(gameObject);
        }
        
        private void SubtractUnits(int amount)
        {
            UnitCount.Value -= amount;
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
            if(NetworkManager.Singleton.LocalClientId == PlayerId)
                GameEvents.UNIT.OnUnitGroupSelected?.Invoke(this);
        }

        [ClientRpc]
        private void TriggerOnDamageTakenClientRpc()
        {
            OnDamageTaken?.Invoke();
        }
        
        private void HandleUnitCountChanged(int previousvalue, int newvalue)
        {
            unitCountText.text = newvalue.ToString();
            
            if(IsServer)
                OnUnitCountUpdated?.Invoke(newvalue);
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
