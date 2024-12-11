using System;
using System.Collections.Generic;
using GameEvents;
using HexSystem;
using Networking.Host;
using Player;
using TMPro;
using Unit.Model;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Unit
{
    public class UnitGroup : NetworkBehaviour
    {
        [Header("References")]
        [field: SerializeField]
        public UnitGroupMovement Movement { get; private set; }

        [field: SerializeField] public WaypointQueue WaypointQueue { get; private set; }
        
        [SerializeField] private UnitGroupHealthBar healthBar;
        [SerializeField] private UnitGroupTravelLineDrawer travelLineDrawer;
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private UnitGroupCombatInitiator combatInitiator;

        public event UnityAction OnUnitHighlightEnabled;
        public event UnityAction OnUnitHighlightDisabled;

        public UnityEvent onUnitLost;
        public UnityEvent<float> onDamageTaken;

        public Action<int> OnUnitCountUpdated;

        public static Dictionary<ulong, UnitGroup> UnitGroupsInGame = new();
        
        public bool IsFighting { get; private set; }
        private bool IsResting { get; set; }
        public bool CanMove => !IsFighting && !IsResting;
        
        public ulong PlayerId { get; private set; }
        public NetworkVariable<int> UnitCount { get; private set; } = new(0);

        private float _combatHealth;
        private PlayerColor _playerColor;
        
        
        public override void OnNetworkSpawn()
        {
            UnitCount.OnValueChanged += HandleUnitCountChanged;
            UnitGroupsInGame.Add(NetworkObjectId, this);

            if (!IsServer)
            {
                Destroy(combatInitiator.gameObject);
                return;
            }
            
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleSwitchedDayNightCycle;
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
            Movement.Initialize(startHexagon);
        }

        public void IntegrateUnitsOf(UnitGroup otherUnitGroup)
        {
            AddUnits(otherUnitGroup.UnitCount.Value);
            otherUnitGroup.Delete();
        }
        
        public void AddUnits(int amount)
        {
            UnitCount.Value += amount;
            _combatHealth += amount;
        }

        public void TakeDamage(float damageAmount)
        {
            _combatHealth -= damageAmount;
            ApplyDamageClientRpc(_combatHealth, damageAmount);
            
            var unitLost = false;
            while (_combatHealth < UnitCount.Value - 1)
            {
                unitLost = true;
                SubtractUnits(1);
            }

            if(unitLost)
                TriggerOnUnitLostClientRpc();
        }

        public void UpdateFightingState(bool isFighting)
        {
            IsFighting = isFighting;
            _combatHealth = UnitCount.Value;
            ToggleHealthBarClientRpc(isFighting);
        }
        
        public void Delete()
        {
            ServerEvents.Unit.OnUnitGroupWithIdDeleted.Invoke(NetworkObjectId);
            Destroy(gameObject);
        }
        
        private void SubtractUnits(int amount)
        {
            UnitCount.Value -= amount;
        }
        
        private void HandleSwitchedDayNightCycle(DayNightCycle.CycleState newDayNightCycle)
        {
            IsResting = newDayNightCycle == DayNightCycle.CycleState.Night;
        }

        #endregion

        #region Client

        [ClientRpc]
        private void InitializeClientRpc(ulong playerId, int encodedPlayerColorType)
        {
            PlayerId = playerId;
            
            _playerColor = PlayerColor.GetFromColorType(PlayerColor.IntToColorType(encodedPlayerColorType));
            meshRenderer.material = _playerColor.unitMaterial;

            travelLineDrawer.InitializeTravelLine(_playerColor);
            healthBar.Initialize(_playerColor.baseColor);
        }
        
        [ClientRpc]
        private void SetAsSelectedClientRpc()
        {
            if(NetworkManager.Singleton.LocalClientId == PlayerId)
                ClientEvents.Unit.OnUnitGroupSelected?.Invoke(this);
        }

        [ClientRpc]
        private void ApplyDamageClientRpc(float newHealth, float damageAmount)
        {
            healthBar.SetHealth(newHealth);
            onDamageTaken?.Invoke(damageAmount);
        }

        [ClientRpc]
        private void TriggerOnUnitLostClientRpc()
        {
            onUnitLost?.Invoke();
        }
        
        private void HandleUnitCountChanged(int previousValue, int newValue)
        {
            unitCountText.text = newValue.ToString();
            
            if(IsServer)
                OnUnitCountUpdated?.Invoke(newValue);
        }

        [ClientRpc]
        private void ToggleHealthBarClientRpc(bool on)
        {
            if (on)
                healthBar.Show(UnitCount.Value);
            else
                healthBar.Hide();
        }

        [ClientRpc]
        private void IncreaseHealthBarClientRpc(int amount)
        {
            healthBar.IncreaseMaxUnitCount(amount);
            healthBar.SetHealth(_combatHealth);
        }
        
        public void EnableHighlight()
        {
            meshRenderer.material = _playerColor.highlightedUnitMaterial;
            OnUnitHighlightEnabled?.Invoke();
        }

        public void DisableHighlight()
        {
            meshRenderer.material = _playerColor.unitMaterial;
            OnUnitHighlightDisabled?.Invoke();
        }
        
        #endregion
    }
}
