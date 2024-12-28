using System;
using Core.GameEvents;
using Core.Unit;
using UnityEngine;

namespace Core.Input
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private LayerMask selectionLayer;
        [SerializeField] private UnitPlacement unitPlacement;
    
        private InputState _currentInputState;
        private InputState _dayInputState;
        private InputState _nightInputState;

        private void Awake()
        {
            _dayInputState = new DayInputState(inputReader, selectionLayer);
            _nightInputState = new NightInputState(inputReader, selectionLayer);
            
            _currentInputState = _dayInputState;
        }

        private void OnEnable()
        {
            SubscribeInputStateToInputEvents(_currentInputState);

            ClientEvents.DayNightCycle.OnSwitchedCycleState += OnSwitchedDayNightState;
        }

        private void OnDisable()
        {
            UnsubscribeInputStateToInputEvents(_currentInputState);
            
            ClientEvents.DayNightCycle.OnSwitchedCycleState -= OnSwitchedDayNightState;
        }
        
        private void OnSwitchedDayNightState(DayNightCycle.DayNightCycle.CycleState newCycleState)
        {
            UnsubscribeInputStateToInputEvents(_currentInputState);

            _currentInputState = newCycleState switch
            {
                DayNightCycle.DayNightCycle.CycleState.Day => _dayInputState,
                DayNightCycle.DayNightCycle.CycleState.Night => _nightInputState,
                _ => throw new ArgumentException("Invalid DayNightCycle state")
            };

            SubscribeInputStateToInputEvents(_currentInputState);
        }

        private void SubscribeInputStateToInputEvents(InputState inputState)
        {
            inputReader.OnLeftMouseClick += inputState.HandleMouseDown;
            inputReader.OnRightMouseClick += inputState.HandleRightClick;
            inputReader.OnMouseDragged += inputState.HandleMainPointerDrag;
        }
        
        private void UnsubscribeInputStateToInputEvents(InputState inputState)
        {
            inputReader.OnLeftMouseClick -= inputState.HandleMouseDown;
            inputReader.OnRightMouseClick -= inputState.HandleRightClick;
            inputReader.OnMouseDragged -= inputState.HandleMainPointerDrag;
        }
    }
}