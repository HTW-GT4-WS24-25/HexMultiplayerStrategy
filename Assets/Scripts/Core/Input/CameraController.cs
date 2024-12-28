using Core.GameEvents;
using UnityEngine;

namespace Core.Input
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera cam;
        [SerializeField] private Transform cameraHandle;
        
        [Header("Settings")] 
        [SerializeField] private float minZoom;
        [SerializeField] private float maxZoom;
        [SerializeField] private float zoomPerInput;
        [SerializeField] private float dragPerInputOnMaxZoom;
        [SerializeField] private float dragPerInputOnMinZoom;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float cameraMoveSpeed = 12;
        [SerializeField] private float moveAmount;
        [SerializeField] private float cameraRotationSpeed = 20;
        [SerializeField] private float rotationAmount;
    
        private float _currentZoom;
        private Vector3 _defaultCameraZoomPosition;
        private Vector3 _zoomVector;
        private Vector3 _newCameraHandlePosition;
        private Vector3 _newCameraZoomPosition;
        private float _dragPerInput;
        private Vector2 _currentMoveInput;
        private float _currentRotationInput;
        private Quaternion _newCameraHandleRotation;

        private void Awake()
        {
            _defaultCameraZoomPosition = cam.transform.localPosition;
            _newCameraZoomPosition = _defaultCameraZoomPosition;
            _newCameraHandlePosition = cameraHandle.localPosition;
            _newCameraHandleRotation = cameraHandle.rotation;
            _zoomVector = cam.transform.forward * zoomPerInput;
            
            AdjustDragPerInputToCurrentZoom();
        }

        private void OnEnable()
        {
            ClientEvents.Input.OnZoomInput += HandleZoom;
            ClientEvents.Input.OnDragInput += HandleDrag;
            ClientEvents.Input.OnCameraMoveInput += HandleMoveInput;
            ClientEvents.Input.OnCameraTurnInput += HandleRotationInput;
        }

        private void OnDisable()
        {
            ClientEvents.Input.OnZoomInput -= HandleZoom;
            ClientEvents.Input.OnDragInput -= HandleDrag;
            ClientEvents.Input.OnCameraMoveInput -= HandleMoveInput;
            ClientEvents.Input.OnCameraTurnInput -= HandleRotationInput;
        }

        private void Update()
        {
            if (_currentMoveInput != Vector2.zero)
            {
                var alignedMoveDirection = cameraHandle.rotation * new Vector3(_currentMoveInput.x, 0, _currentMoveInput.y);
                _newCameraHandlePosition += alignedMoveDirection * moveAmount * Time.deltaTime;
            }

            if (!_currentRotationInput.Equals(0f))
            {
                _newCameraHandleRotation *= Quaternion.Euler(Vector3.up * rotationAmount * _currentRotationInput * Time.deltaTime);
            }
                
            cameraHandle.localPosition = Vector3.Lerp(cameraHandle.position, _newCameraHandlePosition, Time.deltaTime * cameraMoveSpeed);
            cameraHandle.rotation = Quaternion.Lerp(cameraHandle.rotation, _newCameraHandleRotation, Time.deltaTime * cameraRotationSpeed);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, _newCameraZoomPosition, Time.deltaTime * zoomSpeed);
        }

        public void PositionCameraOnPlayerStartPosition(Vector3 startPosition, Vector3 mapCenterPosition)
        {
            cameraHandle.position = startPosition;
            _newCameraHandlePosition = cameraHandle.localPosition;

            var defaultCameraDirection = Vector3.forward;
            var cameraDiretionToCenter = mapCenterPosition - startPosition;
            var cameraAngle = Vector3.SignedAngle(defaultCameraDirection, cameraDiretionToCenter, Vector3.up);
            cameraHandle.Rotate(Vector3.up, cameraAngle);
        }

        private void HandleZoom(float zoomInput)
        {
            _currentZoom += zoomInput;
            _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
            
            _newCameraZoomPosition = _defaultCameraZoomPosition + _zoomVector * _currentZoom;

            AdjustDragPerInputToCurrentZoom();
        }

        private void AdjustDragPerInputToCurrentZoom()
        {
            var totalZoomRange = maxZoom - minZoom;
            var currentZoomValueRelativeToMax = (_currentZoom - minZoom) / totalZoomRange;
            
            _dragPerInput = Mathf.Lerp(dragPerInputOnMinZoom, dragPerInputOnMaxZoom, currentZoomValueRelativeToMax);
        }
        
        private void HandleDrag(Vector2 dragVector)
        {
            var alignedDragDirection = cameraHandle.rotation * new Vector3(dragVector.x, 0, dragVector.y);
            
            _newCameraHandlePosition += alignedDragDirection * -_dragPerInput;
        }

        private void HandleMoveInput(Vector2 moveVector)
        {
            _currentMoveInput = moveVector;
        }

        private void HandleRotationInput(float rotateValue)
        {
            _currentRotationInput = rotateValue;
        }
    }
}
