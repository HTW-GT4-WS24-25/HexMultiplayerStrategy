using HexSystem;
using UnityEngine;
public class InputHandler : MonoBehaviour
{
       
    [SerializeField] private UnitController unitController;
    [SerializeField] private LayerMask selectionLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryGetTraversableHexOnMouse(out var clickedHex))
            {
                unitController.HandleHexClick(clickedHex);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            unitController.DeselectUnit();
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
            if (clickedHexagon != null)
            {
                hexagon = clickedHexagon;       
                return true;
            }
        }

        return false;
    }
}
