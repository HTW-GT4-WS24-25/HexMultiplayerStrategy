using System;
using Unit;
using UnityEngine;

namespace UI
{
    public class InGameUI : MonoBehaviour
    {
        [SerializeField] UnitSelectionUI unitSelectionUI;

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
            unitSelectionUI.gameObject.SetActive(true);   
        }
        
        private void HandleGroupDeselected()
        {
            unitSelectionUI.gameObject.SetActive(false);
        }
    }
}
