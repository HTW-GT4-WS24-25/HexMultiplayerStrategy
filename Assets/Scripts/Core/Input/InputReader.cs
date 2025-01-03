using Core.GameEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Core.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        [SerializeField] private float maxDragDistanceForClick;
        
        public event UnityAction OnLeftMouseClick;
        public event UnityAction OnRightMouseClick;
        public event UnityAction<Vector2> OnMouseDragged; 
    
        public Vector2 MousePosition { get; private set; }
    
        private Controls _controls;
        private Vector2 _mousePressedPosition;
        private bool _mouseIsDown;
        private bool _isDragging;
        private bool _previousUnitShortcutPressed;
    
        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
        
            _controls.Player.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Disable();
        }

        private void CheckForDragging(Vector2 inputPosition)
        {
            if (_isDragging)
            {
                var dragChangeToLastInput = inputPosition - MousePosition;
                OnMouseDragged?.Invoke(dragChangeToLastInput);
            }
            else if (_mouseIsDown)
            {
                var dragVector = inputPosition - _mousePressedPosition;
                if (dragVector.magnitude > maxDragDistanceForClick)
                {
                    OnMouseDragged?.Invoke(dragVector);
                    _isDragging = true;
                }
            }
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            var inputPosition = context.ReadValue<Vector2>();
            CheckForDragging(inputPosition);
            
            MousePosition = context.ReadValue<Vector2>();
        }

        public void OnMouseDown(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
            
            if (EventSystem.current.IsPointerOverGameObject()) // is over UI
                return;
                
            _mousePressedPosition = MousePosition;
            _mouseIsDown = true;
        }

        public void OnMouseUp(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
            
            if(!_isDragging)
                OnLeftMouseClick?.Invoke();
                
            _mouseIsDown = false;
            _isDragging = false;
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnRightMouseClick?.Invoke();
            }
        }

        public void OnZoomScroll(InputAction.CallbackContext context) // TODO: Refactor this to InputHandler
        {
            if (!context.performed) 
                return;
            
            var scrollValue = context.ReadValue<float>();
            ClientEvents.Input.OnZoomInput?.Invoke(scrollValue);
        }

        public void OnCameraMove(InputAction.CallbackContext context)
        {
            var moveInput = context.ReadValue<Vector2>();
            ClientEvents.Input.OnCameraMoveInput?.Invoke(moveInput);
        }

        public void OnCameraTurn(InputAction.CallbackContext context)
        {
            var rotateInput = context.ReadValue<float>();
            ClientEvents.Input.OnCameraTurnInput?.Invoke(rotateInput);
        }

        public void OnNextUnitPressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum1Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum2Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum3Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum4Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum5Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum6Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum7Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum8Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnNum9Pressed(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
        }

        public void OnPauseToggle(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            ClientEvents.Input.PauseTogglePressed?.Invoke();
        }
    }
}
