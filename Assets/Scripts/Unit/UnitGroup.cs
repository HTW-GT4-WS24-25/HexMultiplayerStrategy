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

        private int _unitCount;

        public void Initialize(Hexagon hexagon, int unitCount)
        {
            Hexagon = hexagon;
            Movement.Initialize(Hexagon);
            
            _unitCount = unitCount;
            UpdateUnitCountText();
        }

        public void AddUnits(int amount)
        {
            _unitCount += amount;
            UpdateUnitCountText();
        }

        private void UpdateUnitCountText()
        {
            unitCountText.text = _unitCount.ToString();
        }

    }
}
