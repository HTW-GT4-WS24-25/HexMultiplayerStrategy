using GameEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input
{
    public class DayInputState : InputState
    {
        public DayInputState(InputReader inputReader, LayerMask selectionLayer) : base(inputReader, selectionLayer)
        { }

        public override void HandleMainPointerDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            if (TryGetHexOnScreenPosition(InputReader.MainPointerPosition, out var clickedHexagon))
                ClientEvents.Input.OnHexSelectedForUnitSelectionOrMovement?.Invoke(clickedHexagon);
        }

        public override void HandleRightClick()
        {
            ClientEvents.Unit.OnUnitGroupDeselected?.Invoke();
            HandleMainPointerDown();
        }
    }
}