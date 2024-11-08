using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "InputReader")]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        [SerializeField] private float maxDragDistanceForClick;
        
        public event UnityAction OnMainPointerClicked;
        public event UnityAction OnRightMouseButtonDown;
        public event UnityAction<Vector2> OnMainPointerDragged; 
        public event UnityAction OnSwitchDayNightCycle;
    
        public Vector2 MainPointerPosition { get; private set; }
    
        private Controls _controls;
        private Vector2 _mainPointerPressedPosition;
        private bool _mainPointerIsDown;
        private bool _isDragging;
    
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
            var inputPosition = context.ReadValue<Vector2>();
            CheckForDragging(inputPosition);
            
            MainPointerPosition = context.ReadValue<Vector2>();
        }

        private void CheckForDragging(Vector2 inputPosition)
        {
            if (_isDragging)
            {
                var dragChangeToLastInput = inputPosition - MainPointerPosition;
                OnMainPointerDragged?.Invoke(dragChangeToLastInput);
            }
            else if (_mainPointerIsDown)
            {
                var dragVector = inputPosition - _mainPointerPressedPosition;
                if (dragVector.magnitude > maxDragDistanceForClick)
                {
                    OnMainPointerDragged?.Invoke(dragVector);
                    _isDragging = true;
                }
            }
        }

        public void OnMainPointerDown(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (EventSystem.current.IsPointerOverGameObject()) // is over UI
                    return;
                
                _mainPointerPressedPosition = MainPointerPosition;
                _mainPointerIsDown = true;
                Debug.Log("OnMainPointer performed");
            }
        }
        
        public void OnMainPointerUp(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if(!_isDragging)
                    OnMainPointerClicked?.Invoke();
                
                _mainPointerIsDown = false;
                _isDragging = false;
                Debug.Log("OnMainPointer released");
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

        public void OnZoomScroll(InputAction.CallbackContext context) // TODO: Refactor this to InputHandler
        {
            if(context.performed)
            {
                var scrollValue = context.ReadValue<float>();
                GameEvents.INPUT.OnZoomInput?.Invoke(scrollValue);
            }
        }
    }
}
