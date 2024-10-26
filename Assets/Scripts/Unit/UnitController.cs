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
        private int _selectedUnitCount;

        private void OnEnable()
        {
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement += HandleHexClick;
            GameEvents.INPUT.OnUnitGroupDeselected += DeselectUnit;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight += DeselectUnit;
            GameEvents.UNIT.OnUnitSelectionSliderUpdate += UpdateSelectedUnitCount;
            GameEvents.UNIT.OnUnitGroupDeleted += DeselectDeletedUnit;
        }
        
        private void OnDisable()
        {
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement -= HandleHexClick;
            GameEvents.INPUT.OnUnitGroupDeselected -= DeselectUnit;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight -= DeselectUnit;
            GameEvents.UNIT.OnUnitSelectionSliderUpdate -= UpdateSelectedUnitCount;
            GameEvents.UNIT.OnUnitGroupDeleted -= DeselectDeletedUnit;
        }

        private void HandleHexClick(Hexagon clickedHex)
        {
            if (_selectedUnitGroup != null)
            {
                if (CanMoveOnHex(clickedHex))
                {
                    SplitSelectedUnit();
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

        private void SplitSelectedUnit()
        {
            if (_selectedUnitCount == _selectedUnitGroup.UnitCount) return;
            
            _selectedUnitGroup = _selectedUnitGroup.SplitUnitGroup(_selectedUnitCount);
            GameEvents.UNIT.OnUnitGroupSelected.Invoke(_selectedUnitGroup);
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
            clickedHex.unitGroups.RemoveAll(unitGroup => unitGroup == null); //Unfortunately necessary, because sometimes deleted Groups are still referenced from the hex
            _selectedUnitGroup = clickedHex.unitGroups.FirstOrDefault();
            
            if(_selectedUnitGroup != null)
                GameEvents.UNIT.OnUnitGroupSelected?.Invoke(_selectedUnitGroup);
        }

        private void DeselectUnit()
        {
            _selectedUnitGroup = null;
        }

        private void UpdateSelectedUnitCount(int count)
        {
            _selectedUnitCount = count;
        }

        private void DeselectDeletedUnit(UnitGroup unitGroup)
        {
            if (_selectedUnitGroup != unitGroup) return;
            
            GameEvents.INPUT.OnUnitGroupDeselected.Invoke();
        }
    }
}
