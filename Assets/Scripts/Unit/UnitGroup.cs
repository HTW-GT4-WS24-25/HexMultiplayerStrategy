using System;
using System.Collections.Generic;
using HexSystem;
using TMPro;
using UnityEngine;

namespace Unit
{
    public class UnitGroup : MonoBehaviour
    {
        [Header("References")]
        [field: SerializeField] public UnitGroupMovement Movement { get; private set; }
        [SerializeField] private TextMeshProUGUI unitCountText;
        
        public Hexagon Hexagon { get; set; }
        public int UnitCount { get; private set; }
        public Color DominanceColor { get; private set; } = Color.yellow;

        public void Initialize(Hexagon hexagon, int unitCount)
        {
            PlaceOnHex(hexagon);
            Movement.Initialize(Hexagon);
            UpdateUnitCount(unitCount);
        }

        public void ChangeUnitCount(int changeAmount)
        {
            UpdateUnitCount(UnitCount + changeAmount);
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
        
        private void UpdateUnitCount(int unitCount)
        {
            UnitCount = unitCount;
            unitCountText.text = unitCount.ToString();
        }

    }
}
