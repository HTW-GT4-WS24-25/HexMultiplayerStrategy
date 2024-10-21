using System;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask selectionLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetTilePositionOnMousePosition(out var tilePosition))
            {
                selectedUnit.AddWaypoint(tilePosition);
            }
        }
    }

    private bool TryGetTilePositionOnMousePosition(out Vector3 tilePosition)
    {
        tilePosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 100f, selectionLayer);
        
        foreach (var raycastHit in hits)
        {
            var clickedTile = raycastHit.collider.gameObject.GetComponentInParent<Tile>();
            if (clickedTile != null && clickedTile.isTraversable)
            {
                tilePosition = clickedTile.transform.position;
                return true;
            }
        }

        return false;
    }
}
