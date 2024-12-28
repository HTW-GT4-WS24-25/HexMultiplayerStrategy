using GameEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input
{
    public class NightInputState : InputState
    {
        public NightInputState(InputReader inputReader, LayerMask selectionLayer) : base(inputReader, selectionLayer)
        { }

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