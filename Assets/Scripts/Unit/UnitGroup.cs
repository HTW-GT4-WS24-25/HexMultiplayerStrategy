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
        [field: SerializeField] public UnitGroupMovement Movement { get; private set; }
        [field: SerializeField] public WaypointQueue WaypointQueue { get; private set; }
        
        [SerializeField] private UnitGroupTravelLineDrawer travelLineDrawer;
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private UnitGroupCombatInitiator combatInitiator;

        public event UnityAction OnUnitHighlightEnabled;
        public event UnityAction OnUnitHighlightDisabled;

        public UnityEvent OnDamageTaken;

        public Action<int> OnUnitCountUpdated;

        public static Dictionary<ulong, UnitGroup> UnitGroupsInGame = new();
        
        public bool IsFighting { get; set; }
        private bool IsResting { get; set; }
        public bool CanMove => !IsFighting && !IsResting;

        private float _combatHealth;
        
        public ulong PlayerId { get; private set; }
        public NetworkVariable<int> UnitCount { get; private set; } = new(0);
        public PlayerColor PlayerColor { get; private set; }

        
        
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
            while (_combatHealth < UnitCount.Value - 1)
                SubtractUnits(1);

            TriggerOnDamageTakenClientRpc();
        }

        public void SetHealthToCount()
        {
            _combatHealth = UnitCount.Value;
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
            
            PlayerColor = PlayerColor.GetFromColorType(PlayerColor.IntToColorType(encodedPlayerColorType));
            meshRenderer.material = PlayerColor.unitMaterial;
            
            travelLineDrawer.InitializeTravelLine(PlayerColor);
        }
        
        [ClientRpc]
        private void SetAsSelectedClientRpc()
        {
            if(NetworkManager.Singleton.LocalClientId == PlayerId)
                ClientEvents.Unit.OnUnitGroupSelected?.Invoke(this);
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
