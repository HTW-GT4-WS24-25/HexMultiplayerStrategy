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
        
        public ulong PlayerId { get; private set; }
        public Hexagon Hexagon { get; set; }
        public NetworkVariable<int> UnitCount { get; private set; } = new(0);
        public Color DominanceColor { get; private set; } = Color.yellow;
        public PlayerColor PlayerColor { get; private set; }

        public override void OnNetworkSpawn()
        {
            UnitCount.OnValueChanged += HandleUnitCountChanged;
        }

        public void Initialize(Hexagon hexagon, int unitCount, ulong playerId)
        {
            PlayerId = playerId;
            
            PlaceOnHex(hexagon);
            UnitCount.Value = unitCount;
            
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId);
            InitializeClientRpc(playerId, (int)playerData.PlayerColorType);
            Movement.NextHexagon = hexagon;
        }

        [ClientRpc]
        private void InitializeClientRpc(ulong playerId, int encodedPlayerColorType)
        {
            PlayerId = playerId;
            
            PlayerColor = PlayerColor.GetFromColorType(PlayerColor.IntToColorType(encodedPlayerColorType));
            meshRenderer.material = PlayerColor.unitMaterial;
            
            Movement.Initialize(PlayerColor);
        }
        
        private void HandleUnitCountChanged(int previousvalue, int newvalue)
        {
            unitCountText.text = newvalue.ToString();
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
            Hexagon.unitGroups.Remove(this);
            Destroy(gameObject);
        }
        
        public void PlaceOnHex(Hexagon hexagon)
        {
            Hexagon = hexagon;
            hexagon.unitGroups.Add(this);
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

    }
}
