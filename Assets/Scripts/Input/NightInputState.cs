using GameEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input
{
    public class NightInputState : InputState
    {
        private readonly UnitPlacement _unitPlacement;

        public NightInputState(InputReader inputReader, LayerMask selectionLayer, UnitPlacement unitPlacement) : base(
            inputReader, selectionLayer)
        {
            _unitPlacement = unitPlacement;
        }

        public override void HandleMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            if (TryGetHexOnScreenPosition(InputReader.MousePosition, out var clickedHexagon))
            {
                ClientEvents.Input.OnHexSelectedDuringNightShop?.Invoke(clickedHexagon);
            }
        }

        public override void HandleRightClick()
        {
            ClientEvents.NightShop.OnCardDeselected?.Invoke();
        }
    }
}