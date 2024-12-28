using Core.GameEvents;
using Core.HexSystem.Hex;
using Core.Unit.Group;
using UnityEngine;

namespace Core.Input
{
    public abstract class InputState
    {
        protected readonly InputReader InputReader;
        
        private readonly LayerMask _groundLayer;
        private readonly LayerMask _unitLayer;
        private readonly RaycastHit[] _raycastHits = new RaycastHit[16];

        protected InputState(InputReader inputReader, LayerMask groundLayer, LayerMask unitLayer)
        {
            InputReader = inputReader;
            _groundLayer = groundLayer;
            _unitLayer = unitLayer;
        }

        public abstract void HandleMouseDown();
        public abstract void HandleRightClick();

        public void HandleMainPointerDrag(Vector2 dragValue)
        {
            ClientEvents.Input.OnDragInput?.Invoke(dragValue);
        }

        protected bool TryGetUnitOnScreenPosition(Vector2 screenPosition, out UnitGroup unitGroup)
        {
            unitGroup = null;
            var ray = Camera.main!.ScreenPointToRay(screenPosition);
            var hitCount = Physics.RaycastNonAlloc(ray, _raycastHits, 100f, _unitLayer);

            for (var i = 0; i < hitCount; i++)
            {
                var clickedUnit = _raycastHits[i].collider.gameObject.GetComponentInParent<UnitGroup>();
                if (clickedUnit != null)
                {
                    unitGroup = clickedUnit;       
                    return true;
                }
            }

            return false;
        }

        protected bool TryGetHexOnScreenPosition(Vector2 screenPosition, out Hexagon hexagon)
        {
            hexagon = null;
            var ray = Camera.main!.ScreenPointToRay(screenPosition);
            var hitCount = Physics.RaycastNonAlloc(ray, _raycastHits, 100f, _groundLayer);

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