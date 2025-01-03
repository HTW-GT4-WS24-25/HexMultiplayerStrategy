using Core.GameEvents;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Input
{
    public class DayInputState : InputState
    {
        public DayInputState(InputReader inputReader, LayerMask groundLayer, LayerMask unitLayer) : base(inputReader, groundLayer, unitLayer)
        { }

        public override void HandleMouseDown()
        {
            if(EventSystem.current.IsPointerOverGameObject()) // Todo: why do we need this?
                return;
            
            if (TryGetUnitOnScreenPosition(InputReader.MousePosition, out var clickedUnit) &&
                clickedUnit.PlayerId == NetworkManager.Singleton.LocalClientId)
            {
                ClientEvents.Input.OnUnitGroupSelected?.Invoke(clickedUnit);
                return;
            }
            
            if (TryGetHexOnScreenPosition(InputReader.MousePosition, out var clickedHexagon))
                ClientEvents.Input.OnHexSelectedForUnitMovement?.Invoke(clickedHexagon);
        }

        public override void HandleRightClick()
        {
            ClientEvents.Unit.OnUnitGroupDeselected?.Invoke();
            HandleMouseDown();
        }
    }
}