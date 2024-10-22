using System;
using System.Linq;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask selectionLayer;
    [SerializeField] private MapCreator mapCreator;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetTraversableHexOnMouse(out var clickedHex))
            {
                var currentUnitCoordinates = selectedUnit.NextWaypoint.Coordinates;
                var clickedCoordinates = clickedHex.Coordinates;
                
                var newUnitPath = mapCreator.Grid.GetPathBetween(currentUnitCoordinates, clickedCoordinates);
                selectedUnit.SetAllWaypoints(newUnitPath.Select(hex => new Unit.Waypoint(hex.Coordinates, hex.transform.position)).ToList());
            }
        }
    }

    private bool TryGetTraversableHexOnMouse(out Hexagon hexagon)
    {
        hexagon = null;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 100f, selectionLayer);
        
        foreach (var raycastHit in hits)
        {
            var clickedHexagon = raycastHit.collider.gameObject.GetComponentInParent<Hexagon>();
            if (clickedHexagon != null && clickedHexagon.IsTraversable)
            {
                hexagon = clickedHexagon;       
                return true;
            }
        }

        return false;
    }
}
