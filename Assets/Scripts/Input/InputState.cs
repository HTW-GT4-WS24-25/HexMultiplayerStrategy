using GameEvents;
using HexSystem;
using UnityEngine;

namespace Input
{
    public abstract class InputState
    {
        protected readonly InputReader InputReader;
        
        private readonly LayerMask _selectionLayer;
        private readonly RaycastHit[] _raycastHits = new RaycastHit[16];

        protected InputState(InputReader inputReader, LayerMask selectionLayer)
        {
            InputReader = inputReader;
            _selectionLayer = selectionLayer;
        }

        public abstract void HandleMouseDown();
        public abstract void HandleRightClick();

        public void HandleMainPointerDrag(Vector2 dragValue)
        {
            ClientEvents.Input.OnDragInput?.Invoke(dragValue);
        }

        protected bool TryGetHexOnScreenPosition(Vector2 screenPosition, out Hexagon hexagon)
        {
            hexagon = null;
            var ray = Camera.main!.ScreenPointToRay(screenPosition);
            var hitCount = Physics.RaycastNonAlloc(ray, _raycastHits, 100f, _selectionLayer);

            for (var i = 0; i < hitCount; i++)
            {
                var clickedHexagon = _raycastHits[i].collider.gameObject.GetComponentInParent<Hexagon>();
                if (clickedHexagon != null)
                {
                    hexagon = clickedHexagon;       
                    return true;
                }
            }

            return false;
        }
    }
}