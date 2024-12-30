using System;
using System.Collections.Generic;
using Core.Combat;
using Core.Factions;
using Core.GameEvents;
using Core.HexSystem.Hex;
using Core.PlayerData;
using Core.Unit.Group.Display;
using Core.Unit.Model;
using Networking.Host;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Unit.Group
{
    public class UnitGroup : NetworkBehaviour
    {
        [Header("References")]
        [field: SerializeField] public Movement Movement { get; private set; }
        [field: SerializeField] public WaypointQueue WaypointQueue { get; private set; }
        
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private TravelLineDrawer travelLineDrawer;
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private CombatInitiator combatInitiator;
        [SerializeField] private UnitDeathDummy deathDummyPrefab;
        [SerializeField] private Transform modelHolder;

        public event UnityAction OnUnitHighlightEnabled;
        public event UnityAction OnUnitHighlightDisabled;

        public UnityEvent onUnitLost;
        public UnityEvent<float> onDamageTaken;
        public UnityEvent<float> onHealthHealed;

        public Action<int> OnUnitCountUpdated;

        public static Dictionary<ulong, UnitGroup> UnitGroupsInGame = new();
        
        public bool IsFighting { get; private set; }
        private bool IsResting { get; set; }
        public bool CanMove => !IsFighting && !IsResting;
        public UnitAnimator UnitAnimator { get; private set; }
        
        public ulong PlayerId { get; private set; }
        public NetworkVariable<int> UnitCount { get; private set; } = new();

        private float _combatMaxHealth;
        private float _combatHealth;
        private PlayerColor _playerColor;
        private FactionType _factionType;
        private UnitModel _model;
        
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
                UnitCount.Value--;
            }

            if(unitLost)
                TriggerOnUnitLostClientRpc();
        }

        public void Heal(float healAmount)
        {
            var effectiveAmount = Mathf.Min(healAmount, _combatMaxHealth - _combatHealth);
            if (!(effectiveAmount > float.Epsilon)) 
                return;
            
            _combatHealth += effectiveAmount;
            ApplyHealClientRpc(_combatHealth, effectiveAmount);
            
            while (_combatHealth > UnitCount.Value)
            {
                UnitCount.Value++;
            }
        }

        public void PlayHitAnimationInSeconds(float secondsUntilHit)
        {
            PlayHitAnimationInSecondsClientRpc(secondsUntilHit);
        }

        public void StartFighting(CombatIndicator combatIndicator)
        {
            IsFighting = true;
            ToggleHealthBarClientRpc(true);
            UpdateMovementPauseState();
            _combatMaxHealth = UnitCount.Value; // Todo: this probably needs to be updated when a reinforcement Unit joins the fight
            _combatHealth = _combatMaxHealth;
            
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
            SpawnDeathDummyClientRpc(_playerColor.Type, _factionType);
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

            var player = HostSingleton.Instance.GameManager.GetPlayerByClientId(playerId);
            InitializeClientRpc(playerId, player.PlayerColorType, player.Faction.Type);
        }
        
        private void HandleSwitchedDayNightCycle(DayNightCycle.DayNightCycle.CycleState newDayNightCycle)
        {
            IsResting = newDayNightCycle == DayNightCycle.DayNightCycle.CycleState.Night;
            UpdateMovementPauseState();
        }

        private void UpdateMovementPauseState()
        {
            Movement.SetPaused(!CanMove);
        }

        #endregion

        #region Client

        [ClientRpc]
        private void InitializeClientRpc(ulong playerId, PlayerColor.ColorType playerColorType, FactionType factionType)
        {
            PlayerId = playerId;
            
            _factionType = factionType;
            _model = Instantiate(UnitModel.GetModelPrefabFromFactionType(factionType), modelHolder.position, modelHolder.rotation, modelHolder);
            UnitAnimator = _model.Animator;
            Movement.OnMoveAnimationSpeedChanged += UnitAnimator.SetMoveSpeed;
            
            _playerColor = PlayerColor.GetFromColorType(playerColorType);
            _model.MaskTint.ApplyColoringMaterial(_playerColor.UnitColoringMaterial);

            travelLineDrawer.InitializeTravelLine(_playerColor);
            healthBar.Initialize(_playerColor.BaseColor);
        }

        [ClientRpc]
        private void ApplyDamageClientRpc(float newHealth, float damageAmount)
        {
            healthBar.SetHealth(newHealth);
            onDamageTaken?.Invoke(damageAmount);
        }

        [ClientRpc]
        private void ApplyHealClientRpc(float newHealth, float healAmount)
        {
            healthBar.SetHealth(newHealth);
            onHealthHealed?.Invoke(healAmount);
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
        private void PlayHitAnimationInSecondsClientRpc(float secondsUntilHit)
        {
            UnitAnimator.PlayHitAnimation(secondsUntilHit);
        }

        [ClientRpc]
        private void StopFightingAnimationClientRpc()
        {
            UnitAnimator.StopFightAnimations();
        }

        [ClientRpc]
        private void SpawnDeathDummyClientRpc(PlayerColor.ColorType playerColorType, FactionType factionType)
        {
            var unitDeathDummy = Instantiate(deathDummyPrefab, transform.position, transform.rotation);
            unitDeathDummy.Initialize(PlayerColor.GetFromColorType(playerColorType), factionType);
        }
        
        public void EnableSelectionHighlight()
        {
            _model.ActivateSelectedOutline();
            OnUnitHighlightEnabled?.Invoke();
        }

        public void DisableSelectionHighlight()
        {
            _model.DeactivateSelectedOutline();
            OnUnitHighlightDisabled?.Invoke();
        }

        public void EnableHoverHighlight()
        {
            _model.ActivateHoverOutline();    
        }

        public void DisableHoverHighlight()
        {
            _model.DeactivateHoverOutline();
        }
        
        #endregion
    }
}
