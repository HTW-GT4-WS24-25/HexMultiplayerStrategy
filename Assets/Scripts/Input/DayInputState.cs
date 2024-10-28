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
            {
                GameEvents.INPUT.OnHexSelectedForUnitSelectionOrMovement?.Invoke(clickedHexagon);
            }
        }

        public override void HandleRightClick()
        {
            GameEvents.UNIT.OnUnitGroupDeselected?.Invoke();
            HandleMainPointerDown();
        }
    }
}