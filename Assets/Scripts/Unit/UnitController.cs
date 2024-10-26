using System;
using System.Linq;
using HexSystem;
using UnityEngine;

namespace Unit
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private MapCreator mapCreator;
    
        private UnitGroup _selectedUnitGroup;

        private void OnEnable()
        {
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement += HandleHexClick;
            GameEvents.INPUT.OnUnitGroupDeselected += DeselectUnit;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight += DeselectUnit;
        }
        
        private void OnDisable()
        {
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement -= HandleHexClick;
            GameEvents.INPUT.OnUnitGroupDeselected -= DeselectUnit;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight -= DeselectUnit;
        }

        private void HandleHexClick(Hexagon clickedHex)
        {
            if (_selectedUnitGroup != null)
            {
                if (CanMoveOnHex(clickedHex))
                {
                    SetUnitMovement(clickedHex);
                }
            }
            else
            {
                SetSelectedUnit(clickedHex);
            }
        }
        
        private bool CanMoveOnHex(Hexagon hexagon)
        {
            return hexagon.isTraversable;
        }
        
        private void SetUnitMovement(Hexagon clickedHex)
        {
            var currentUnitCoordinates = _selectedUnitGroup.Movement.NextHexagon.Coordinates;
            var clickedCoordinates = clickedHex.Coordinates;
                
            var newUnitPath = mapCreator.Grid.GetPathBetween(currentUnitCoordinates, clickedCoordinates);
            _selectedUnitGroup.Movement.SetAllWaypoints(newUnitPath);
        }
        
        private void SetSelectedUnit(Hexagon clickedHex)
        {
            _selectedUnitGroup = clickedHex.unitGroups.FirstOrDefault();
            
            if(_selectedUnitGroup != null)
                GameEvents.UNIT.OnUnitGroupSelected?.Invoke(_selectedUnitGroup);
        }

        private void DeselectUnit()
        {
            _selectedUnitGroup = null;
        }
    }
}
