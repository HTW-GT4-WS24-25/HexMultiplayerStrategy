using System;
using System.Collections.Generic;
using Combat;
using GameEvents;
using HexSystem;
using Networking.Host;
using Player;
using TMPro;
using Unit.Model;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Unit
{
    public class UnitGroup : NetworkBehaviour
    {
        [Header("References")]
        [field: SerializeField] public UnitGroupMovement Movement { get; private set; }
        [field: SerializeField] public WaypointQueue WaypointQueue { get; private set; }
        
        [SerializeField] private UnitGroupHealthBar healthBar;
        [SerializeField] private UnitGroupTravelLineDrawer travelLineDrawer;
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private AnimalMaskTint modelColorTint;
        [SerializeField] private UnitGroupCombatInitiator combatInitiator;
        [SerializeField] private UnitAnimator unitAnimator;
        [SerializeField] private UnitDeathDummy deathDummyPrefab;

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

        public void InitializeOnHexCenter(int unitCount, ulong playerId, Hexagon hexagon)
        {
            Initialize(unitCount, playerId);
            Movement.InitializeAsStationary(hexagon);
        }

        public void InitializeAsSplitFrom(UnitGroup other, int splitAmount)
        {
            Debug.Assert(other.UnitCount.Value > splitAmount, "Tried to split a unitGroup by a bigger value than it's unit count.");
            
            Initialize(splitAmount, other.PlayerId);
            other.UnitCount.Value -= splitAmount;
            
            var otherPath = other.WaypointQueue.GetWaypoints();
            if (other.Movement.HasMovementLeft)
            {
                Movement.Initialize(other.Movement.StartHexagon);
                otherPath.Insert(0, other.Movement.GoalHexagon);
                WaypointQueue.UpdateWaypoints(otherPath);
            }
            else
            {
                Movement.InitializeAsStationary(other.Movement.StartHexagon);
            }
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

        public void PlayHitAnimationInSeconds(float secondsUntilHit)
        {
            PlayHitAnimationInSecondsClientRpc(secondsUntilHit);
        }

        public void UpdateFightingState(bool isFighting)
        {
            IsFighting = isFighting;
            _combatHealth = UnitCount.Value;
            ToggleHealthBarClientRpc(isFighting);

            UpdateMovementPauseState();
        }

        public void StartFighting(CombatIndicator combatIndicator)
        {
            IsFighting = true;
            ToggleHealthBarClientRpc(true);
            UpdateMovementPauseState();
            _combatHealth = UnitCount.Value;
            
            var combatPosition = combatIndicator.transform.position;
            combatPosition.y = transform.position.y;
            transform.LookAt(combatPosition);
        }
        
        public void EndFighting()
        {
            IsFighting = false;
            ToggleHealthBarClientRpc(false);
            UpdateMovementPauseState();
            
            StopFightingAnimationClientRpc();
        }

        public void DieInCombat()
        {
            SpawnDeathDummyClientRpc(_playerColor.Type);
            ServerEvents.Unit.OnUnitGroupWithIdDeleted.Invoke(NetworkObjectId);
            Destroy(gameObject);
        }
        
        public void Delete()
        {
            ServerEvents.Unit.OnUnitGroupWithIdDeleted.Invoke(NetworkObjectId);
            Destroy(gameObject);
        }

        public void HandleUnitGroupReachedNewHex(Hexagon hexagon)
        {
            ServerEvents.Unit.OnUnitGroupReachedNewHex?.Invoke(this, hexagon.Coordinates);
        }

        public void HandleUnitGroupReachedHexCenter(Hexagon hexagon)
        {
            ServerEvents.Unit.OnUnitGroupReachedHexCenter?.Invoke(this, hexagon.Coordinates);
        }

        public void HandleUnitGroupLeftHexCenter()
        {
            ServerEvents.Unit.OnUnitGroupLeftHexCenter?.Invoke(this);
        }
        
        private void Initialize(int unitCount, ulong playerId)
        {
            UnitCount.Value = unitCount;
            PlayerId = playerId;
            
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId);
            InitializeClientRpc(playerId, (int)playerData.PlayerColorType);
        }
        
        private void SubtractUnits(int amount)
        {
            UnitCount.Value -= amount;
        }
        
        private void HandleSwitchedDayNightCycle(DayNightCycle.CycleState newDayNightCycle)
        {
            IsResting = newDayNightCycle == DayNightCycle.CycleState.Night;
            UpdateMovementPauseState();
        }

        private void UpdateMovementPauseState()
        {
            Movement.SetPaused(!CanMove);
        }

        #endregion

        #region Client

        [ClientRpc]
        private void InitializeClientRpc(ulong playerId, int encodedPlayerColorType)
        {
            PlayerId = playerId;
            
            _playerColor = PlayerColor.GetFromColorType(PlayerColor.IntToColorType(encodedPlayerColorType));
            modelColorTint.ApplyMaterials(_playerColor.UnitColoringMaterial);

            travelLineDrawer.InitializeTravelLine(_playerColor);
            healthBar.Initialize(_playerColor.BaseColor);
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

        [ClientRpc]
        private void PlayHitAnimationInSecondsClientRpc(float secondsUntilHit)
        {
            unitAnimator.PlayHitAnimation(secondsUntilHit);
        }

        [ClientRpc]
        private void StopFightingAnimationClientRpc()
        {
            unitAnimator.StopFightAnimations();
        }

        [ClientRpc]
        private void SpawnDeathDummyClientRpc(PlayerColor.ColorType playerColorType)
        {
            var unitDeathDummy = Instantiate(deathDummyPrefab, transform.position, transform.rotation);
            unitDeathDummy.Initialize(PlayerColor.GetFromColorType(playerColorType));
        }
        
        public void EnableHighlight()
        {
            modelColorTint.ApplyMaterials(_playerColor.HighlightedUnitMaterial);
            OnUnitHighlightEnabled?.Invoke();
        }

        public void DisableHighlight()
        {
            modelColorTint.ApplyMaterials(_playerColor.UnitColoringMaterial);
            OnUnitHighlightDisabled?.Invoke();
        }
        
        #endregion
    }
}
