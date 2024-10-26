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
            inputReader.OnMainPointerDown += _currentInputState.HandleMainPointerDown;
            inputReader.OnRightMouseButtonDown += _currentInputState.HandleRightClick;

            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToDay += () => SwitchToState(_dayInputState);
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight += () => SwitchToState(_nightInputState);
        }
    
        private void OnDisable()
        {
            inputReader.OnMainPointerDown -= _currentInputState.HandleMainPointerDown;
            inputReader.OnRightMouseButtonDown -= _currentInputState.HandleRightClick;
            
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToDay -= () => SwitchToState(_dayInputState);
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight -= () => SwitchToState(_nightInputState);
        }

        private void SwitchToState(InputState newState)
        {
            inputReader.OnMainPointerDown -= _currentInputState.HandleMainPointerDown;
            inputReader.OnRightMouseButtonDown -= _currentInputState.HandleRightClick;
            
            _currentInputState = newState;
            
            inputReader.OnMainPointerDown += _currentInputState.HandleMainPointerDown;
            inputReader.OnRightMouseButtonDown += _currentInputState.HandleRightClick;
        }
    }
}