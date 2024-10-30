using UnityEngine;

namespace Input
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
        [SerializeField] private float dragSpeed;
    
        private float _currentZoom;
        private Vector3 _defaultCameraZoomPosition;
        private Vector3 _zoomVector;
        private Vector3 _newCameraHandlePosition;
        private Vector3 _newCameraZoomPosition;
        private float _dragPerInput;

        private void Awake()
        {
            _defaultCameraZoomPosition = cam.transform.localPosition;
            _newCameraZoomPosition = _defaultCameraZoomPosition;
            _newCameraHandlePosition = cameraHandle.position;
            _zoomVector = cam.transform.forward * zoomPerInput;
            
            AdjustDragPerInputToCurrentZoom();
        }

        private void OnEnable()
        {
            GameEvents.INPUT.OnZoomInput += HandleZoom;
            GameEvents.INPUT.OnDragInput += HandleDrag;
        }

        private void OnDisable()
        {
            GameEvents.INPUT.OnZoomInput -= HandleZoom;
            GameEvents.INPUT.OnDragInput -= HandleDrag;
        }

        private void Update()
        {
            cameraHandle.transform.position = Vector3.Lerp(cameraHandle.transform.position, _newCameraHandlePosition, Time.deltaTime * dragSpeed);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, _newCameraZoomPosition, Time.deltaTime * zoomSpeed);
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
            _newCameraHandlePosition += new Vector3(dragVector.x, 0, dragVector.y) * -_dragPerInput;
        }
    }
}
