using System;
using Unit;
using UnityEngine;

namespace UI
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField] private UnitSelectionUI unitSelectionUI;
        [SerializeField] private GameObject cancelSelectionWindow;

        private void OnEnable()
        {
            GameEvents.UNIT.OnUnitGroupSelected += HandleGroupSelected;
            GameEvents.UNIT.OnUnitGroupDeselected += HandleGroupDeselected;
        }
        
        private void OnDisable()
        {
            GameEvents.UNIT.OnUnitGroupSelected -= HandleGroupSelected;
            GameEvents.UNIT.OnUnitGroupDeselected -= HandleGroupDeselected;
        }

        private void HandleGroupSelected(UnitGroup selectedGroup)
        {
            cancelSelectionWindow.SetActive(true);
            if (selectedGroup.UnitCount.Value > 1) unitSelectionUI.gameObject.SetActive(true);
         
            unitSelectionUI.SetSliderMaximumAndValue(selectedGroup.UnitCount.Value);
        }
        
        private void HandleGroupDeselected()
        {
            unitSelectionUI.gameObject.SetActive(false);
            cancelSelectionWindow.SetActive(false);
        }
    }
}
