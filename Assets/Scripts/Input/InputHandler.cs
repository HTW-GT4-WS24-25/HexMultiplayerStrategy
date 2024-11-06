using System;
using UnityEngine;

namespace Input
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
            _nightInputState = new NightInputState(inputReader, selectionLayer, unitPlacement);
            
            _currentInputState = _nightInputState;
        }

        private void OnEnable()
        {
            SubscribeInputStateToInputEvents(_currentInputState);

            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += OnSwitchedDayNightState;
        }

        private void OnDisable()
        {
            UnsubscribeInputStateToInputEvents(_currentInputState);
            
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState -= OnSwitchedDayNightState;
        }
        
        private void OnSwitchedDayNightState(DayNightCycle.CycleState newCycleState)
        {
            UnsubscribeInputStateToInputEvents(_currentInputState);

            _currentInputState = newCycleState switch
            {
                DayNightCycle.CycleState.Day => _dayInputState,
                DayNightCycle.CycleState.Night => _nightInputState,
                _ => throw new ArgumentException("Invalid DayNightCycle state")
            };

            SubscribeInputStateToInputEvents(_currentInputState);
        }

        private void SubscribeInputStateToInputEvents(InputState inputState)
        {
            inputReader.OnMainPointerClicked += inputState.HandleMainPointerDown;
            inputReader.OnRightMouseButtonDown += inputState.HandleRightClick;
            inputReader.OnMainPointerDragged += inputState.HandleMainPointerDrag;
        }
        
        private void UnsubscribeInputStateToInputEvents(InputState inputState)
        {
            inputReader.OnMainPointerClicked -= inputState.HandleMainPointerDown;
            inputReader.OnRightMouseButtonDown -= inputState.HandleRightClick;
            inputReader.OnMainPointerDragged -= inputState.HandleMainPointerDrag;
        }
    }
}