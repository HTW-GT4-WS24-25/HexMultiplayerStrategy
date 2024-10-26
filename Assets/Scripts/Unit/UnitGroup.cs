using System;
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

        public void Initialize(Hexagon hexagon, int unitCount)
        {
            Hexagon = hexagon;
            Movement.Initialize(Hexagon);
            
            UnitCount = unitCount;
            UpdateUnitCountText();
        }

        public void AddUnits(int amount)
        {
            UnitCount += amount;
            UpdateUnitCountText();
        }

        private void UpdateUnitCountText()
        {
            unitCountText.text = UnitCount.ToString();
        }

    }
}
