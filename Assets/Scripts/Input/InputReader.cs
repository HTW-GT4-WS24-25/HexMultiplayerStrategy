using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        public event UnityAction OnMainPointerDown;
        public event UnityAction OnRightMouseButtonDown;
        public event UnityAction OnSwitchDayNightCycle;
    
        public Vector2 MainPointerPosition { get; private set; }
    
        private Controls _controls;
    
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
        
            _controls.Player.Enable();
        }
    
        public void OnPointerPosition(InputAction.CallbackContext context)
        {
            MainPointerPosition = context.ReadValue<Vector2>();
        }

        public void OnMainPointer(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnMainPointerDown?.Invoke();   
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnRightMouseButtonDown?.Invoke();
            }
        }

        public void OnTestSwitchCycle(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSwitchDayNightCycle?.Invoke();
            }
        }
    }
}
