using System;
using System.Collections.Generic;
using HexSystem;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Unit
{
    public class UnitGroup : MonoBehaviour
    {
        [Header("References")]
        [field: SerializeField] public UnitGroupMovement Movement { get; private set; }
        [SerializeField] private TextMeshProUGUI unitCountText;
        [SerializeField] private MeshRenderer meshRenderer;

        public UnityEvent OnUnitHighlightEnabled;
        public UnityEvent OnUnitHighlightDisabled;
        
        public Hexagon Hexagon { get; set; }
        public int UnitCount { get; private set; }
        public Color DominanceColor { get; private set; } = Color.yellow;
        public PlayerColor PlayerColor { get; private set; }

        public void Initialize(Hexagon hexagon, int unitCount, PlayerColor playerColor)
        {
            PlaceOnHex(hexagon);
            Movement.Initialize(Hexagon, playerColor);
            UpdateUnitCount(unitCount);
            
            PlayerColor = playerColor;
            meshRenderer.material = playerColor.unitMaterial;
        }

        public void AddUnits(int amount)
        {
            UpdateUnitCount(UnitCount + amount);
        }

        public void SubtractUnits(int amount)
        {
            UpdateUnitCount(UnitCount - amount);
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
        
        private void UpdateUnitCount(int unitCount)
        {
            UnitCount = unitCount;
            unitCountText.text = unitCount.ToString();
        }

    }
}
