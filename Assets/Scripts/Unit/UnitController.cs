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
            GameEvents.INPUT.OnUnitDeselected += DeselectUnit;
        }
        
        private void OnDisable()
        {
            GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement -= HandleHexClick;
            GameEvents.INPUT.OnUnitDeselected -= DeselectUnit;
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
            var currentUnitCoordinates = _selectedUnitGroup.Movement.NextWaypoint.Coordinates;
            var clickedCoordinates = clickedHex.Coordinates;
                
            var newUnitPath = mapCreator.Grid.GetPathBetween(currentUnitCoordinates, clickedCoordinates);
            _selectedUnitGroup.Movement.SetAllWaypoints(newUnitPath.Select(hex => new UnitGroupMovement.Waypoint(hex.Coordinates, hex.transform.position)).ToList());
        }
        
        private void SetSelectedUnit(Hexagon clickedHex)
        {
            _selectedUnitGroup = clickedHex.unitGroups.FirstOrDefault();
        }

        private void DeselectUnit()
        {
            _selectedUnitGroup = null;
        }
    }
}
