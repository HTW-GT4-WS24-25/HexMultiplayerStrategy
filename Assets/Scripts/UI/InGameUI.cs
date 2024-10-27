using System;
using Unit;
using UnityEngine;

namespace UI
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField] private UnitSelectionUI unitSelectionUI;

        private void OnEnable()
        {
            GameEvents.UNIT.OnUnitGroupSelected += HandleGroupSelected;
            GameEvents.INPUT.OnUnitGroupDeselected += HandleGroupDeselected;
        }
        
        private void OnDisable()
        {
            GameEvents.UNIT.OnUnitGroupSelected -= HandleGroupSelected;
            GameEvents.INPUT.OnUnitGroupDeselected -= HandleGroupDeselected;
        }

        private void HandleGroupSelected(UnitGroup selectedGroup)
        {
            if (selectedGroup.UnitCount > 1) unitSelectionUI.gameObject.SetActive(true);
            
            unitSelectionUI.SetSliderMaximum(selectedGroup.UnitCount);
        }
        
        private void HandleGroupDeselected()
        {
            unitSelectionUI.gameObject.SetActive(false);
        }
    }
}
