using System;
using System.Linq;
using HexSystem;
using UnityEngine;

namespace Unit
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private MapCreator mapCreator;
        [SerializeField] private UnitGroup unitGroupPrefab;
    
        private UnitGroup _selectedUnitGroup;
        private int _selectedUnitCount = 1;

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
                    var isSplitAndRunsWithGroup = SplitSelectedUnit(clickedHex);
                    SetUnitMovement(clickedHex, isSplitAndRunsWithGroup);
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

        private bool SplitSelectedUnit(Hexagon clickedHex)
        {
            if (_selectedUnitCount == _selectedUnitGroup.UnitCount || _selectedUnitGroup.UnitCount == 1) return false;
            
            var isSplitAndRunsWithGroup = _selectedUnitGroup.Movement.PreviousHexagon != clickedHex;
            _selectedUnitGroup.AddUnits(-_selectedUnitCount);
            
            _selectedUnitGroup = CreateNewUnitGroup();
            GameEvents.UNIT.OnUnitGroupSelected.Invoke(_selectedUnitGroup);
            
            return isSplitAndRunsWithGroup;
        }

        private UnitGroup CreateNewUnitGroup()
        {
            var newUnitGroup = Instantiate(unitGroupPrefab, _selectedUnitGroup.transform.position, Quaternion.identity);
            newUnitGroup.Initialize(_selectedUnitGroup.Movement.NextHexagon, _selectedUnitCount);
            return newUnitGroup;
        }


        private void SetUnitMovement(Hexagon clickedHex, bool isSplitAndRunsWithGroup)
        {
            var currentUnitCoordinates = _selectedUnitGroup.Movement.NextHexagon.Coordinates;
            var clickedCoordinates = clickedHex.Coordinates;
                
            var newUnitPath = mapCreator.Grid.GetPathBetween(currentUnitCoordinates, clickedCoordinates);
            if (isSplitAndRunsWithGroup) newUnitPath.Insert(0, _selectedUnitGroup.Movement.NextHexagon);
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
