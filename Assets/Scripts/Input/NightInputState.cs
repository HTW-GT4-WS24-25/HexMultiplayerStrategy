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
            _unitPlacement.SetPlacementAmount(1); // This is only for testing
        }

        public override void HandleMainPointerDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            if (TryGetHexOnScreenPosition(InputReader.MainPointerPosition, out var clickedHexagon))
            {
                //_unitPlacement.TryAddUnitsToHex(clickedHexagon); TODO: Replace with Client Request
                GameEvents.INPUT.OnHexSelectedDuringNightShop?.Invoke(clickedHexagon);
            }
        }

        public override void HandleRightClick()
        {
            Debug.Log("Right Click functionality not implemented.");
        }
    }
}