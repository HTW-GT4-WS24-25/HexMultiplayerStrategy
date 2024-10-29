using System;
using System.Collections.Generic;
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
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight += () => GameEvents.UNIT.OnUnitGroupDeselected.Invoke();
            GameEvents.UNIT.OnUnitSelectionSliderUpdate += UpdateSelectedUnitCount;
            GameEvents.UNIT.OnUnitGroupDeselected += DeselectUnit;
            GameEvents.UNIT.OnUnitGroupDeleted += DeselectDeletedUnit;
        }
        
        private void OnDisable()
        {
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement -= HandleHexClick;
            GameEvents.UNIT.OnUnitGroupDeselected -= DeselectUnit;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight -= () => GameEvents.UNIT.OnUnitGroupDeselected.Invoke();
            GameEvents.UNIT.OnUnitSelectionSliderUpdate -= UpdateSelectedUnitCount;
            GameEvents.UNIT.OnUnitGroupDeleted -= DeselectDeletedUnit;
        }

        private void HandleHexClick(Hexagon clickedHex)
        {
            if (_selectedUnitGroup != null)
            {
                ProcessMoveCommand(clickedHex);
            }
            else
            {
                SetSelectedUnit(clickedHex);
            }
        }

        private void ProcessMoveCommand(Hexagon clickedHex)
        {
            if (!CanMoveOnHex(clickedHex))
                return;
            
            var newUnitPath = GetPathForSelectedUnitGroup(clickedHex);
            
            if(_selectedUnitCount < _selectedUnitGroup.UnitCount && _selectedUnitGroup.UnitCount > 1)
            {
                var splitUnit = SplitSelectedUnit();
                var splitUnitFollowsOldOne = !newUnitPath.Contains(_selectedUnitGroup.Movement.PreviousHexagon);

                _selectedUnitGroup = splitUnit;
                
                if(splitUnitFollowsOldOne)
                    newUnitPath.Insert(0, _selectedUnitGroup.Movement.NextHexagon);
            }
            
            _selectedUnitGroup.Movement.SetAllWaypoints(newUnitPath);
        }
        
        private bool CanMoveOnHex(Hexagon hexagon)
        {
            return hexagon.isTraversable;
        }

        private UnitGroup SplitSelectedUnit()
        {
            _selectedUnitGroup.ChangeUnitCount(-_selectedUnitCount);
            
            var splitUnitGroup = Instantiate(unitGroupPrefab, _selectedUnitGroup.transform.position, Quaternion.identity);
            splitUnitGroup.Initialize(_selectedUnitGroup.Movement.NextHexagon, _selectedUnitCount);

            return splitUnitGroup;
        }
        
        private List<Hexagon> GetPathForSelectedUnitGroup(Hexagon clickedHex)
        {
            var currentUnitCoordinates = _selectedUnitGroup.Movement.NextHexagon.Coordinates;
            var clickedCoordinates = clickedHex.Coordinates;
                
            return mapCreator.Grid.GetPathBetween(currentUnitCoordinates, clickedCoordinates);
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
            
            GameEvents.UNIT.OnUnitGroupDeselected.Invoke();
        }
    }
}
