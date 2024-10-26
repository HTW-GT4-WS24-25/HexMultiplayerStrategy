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

        public void Initialize(Hexagon hexagon, int unitCount)
        {
            Hexagon = hexagon;
            hexagon.unitGroups.Add(this);
            Movement.Initialize(Hexagon);

            UpdateUnitCount(unitCount);
        }

        public void AddUnits(int amount)
        {
            UpdateUnitCount(UnitCount + amount);
        }

        private void UpdateUnitCount(int unitCount)
        {
            UnitCount = unitCount;
            unitCountText.text = unitCount.ToString();
        }

        public UnitGroup SplitUnitGroup(int newUnitCount)
        {
            UpdateUnitCount(UnitCount - newUnitCount);
            var newUnitGroup = Instantiate(this);
            newUnitGroup.Initialize(Hexagon, newUnitCount);
            return newUnitGroup;
        }

        public void Delete()
        {
            Hexagon.unitGroups.Remove(this);
            Destroy(gameObject);
        }

    }
}
