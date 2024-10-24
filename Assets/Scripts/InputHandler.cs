using HexSystem;
using UnityEngine;
public class InputHandler : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private UnitController unitController;
    [SerializeField] private LayerMask selectionLayer;

    private void OnEnable()
    {
        inputReader.OnMainPointerDown += HandleMainPointerDown;
        inputReader.OnRightMouseButtonDown += HandleRightClick;
    }
    
    private void OnDisable()
    {
        inputReader.OnMainPointerDown += HandleMainPointerDown;
        inputReader.OnRightMouseButtonDown += HandleRightClick;
    }

    private void HandleMainPointerDown()
    {
        if (TryGetHexOnScreenPosition(inputReader.MainPointerPosition, out var clickedHex))
        {
            unitController.HandleHexClick(clickedHex);
        }
    }

    private void HandleRightClick()
    {
        unitController.DeselectUnit();
    }
       
    private bool TryGetHexOnScreenPosition(Vector2 screenPosition, out Hexagon hexagon)
    {
        hexagon = null;
        var ray = Camera.main.ScreenPointToRay(screenPosition);
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
