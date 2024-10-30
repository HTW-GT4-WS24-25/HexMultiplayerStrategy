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

            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToDay += () => SwitchToState(_dayInputState);
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight += () => SwitchToState(_nightInputState);
        }
    
        private void OnDisable()
        {
            UnsubscribeInputStateToInputEvents(_currentInputState);
            
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToDay -= () => SwitchToState(_dayInputState);
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight -= () => SwitchToState(_nightInputState);
        }

        private void SwitchToState(InputState newState)
        {
            UnsubscribeInputStateToInputEvents(_currentInputState);
            
            _currentInputState = newState;
            
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