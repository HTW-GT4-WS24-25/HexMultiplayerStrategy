﻿using Core.GameEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Input
{
    public class NightInputState : InputState
    {
        public NightInputState(InputReader inputReader, LayerMask groundLayer, LayerMask unitLayer) : base(inputReader, groundLayer, unitLayer)
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