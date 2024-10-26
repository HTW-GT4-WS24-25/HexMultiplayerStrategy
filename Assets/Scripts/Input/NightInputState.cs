using UnityEditor;
using UnityEngine;

namespace Input
{
    public class NightInputState : InputState
    {
        private UnitPlacement _unitPlacement;

        public NightInputState(InputReader inputReader, LayerMask selectionLayer, UnitPlacement unitPlacement) : base(
            inputReader, selectionLayer)
        {
            _unitPlacement = unitPlacement;
            _unitPlacement.SetPlacementAmount(1); // This is only for testing
        }

        public override void HandleMainPointerDown()
        {
            if (TryGetHexOnScreenPosition(InputReader.MainPointerPosition, out var clickedHexagon))
            {
                _unitPlacement.TryAddUnitsToHex(clickedHexagon);
            }
        }

        public override void HandleRightClick()
        {
            Debug.Log("Right Click functionality not implemented.");
        }
    }
}