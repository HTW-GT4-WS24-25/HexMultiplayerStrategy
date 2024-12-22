using Sirenix.OdinInspector;
using UnityEngine;

namespace HexSystem
{
    public class HexHighlighting : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Material floorHighlightMaterialDay;
        [SerializeField] private Material wallHighlightMaterialDay;
        [SerializeField] private Material floorHighlightMaterialNight;
        [SerializeField] private Material wallHighlightMaterialNight;
        [SerializeField] private MeshRenderer floorRenderer;
        [SerializeField] private MeshRenderer wallRenderer;

        [Header("Settings")] 
        [SerializeField] private float floorCircleDiameterOnMouseOver;
        [SerializeField] private float wallHighlightLevelOnMouseOver;
        [SerializeField] private float floorCircleDiameterOnMouseOverAndAvailability;
        [SerializeField] private float wallHighlightLevelOnMouseOverAndAvailability;
        
        private bool _isDayHighlight;
        private bool _mouseOverHighlightActive;
        private bool _availableHighlightActive;
        
        private static readonly int CircleDiameter = Shader.PropertyToID("_Circle_Diameter");
        private static readonly int HighlightLevel = Shader.PropertyToID("_Highlight_Level");

        [Button]
        public void ApplyMouseOverHighlightDay()
        {
            floorRenderer.material = floorHighlightMaterialDay;
            wallRenderer.material = wallHighlightMaterialDay;
            gameObject.SetActive(true);

            _mouseOverHighlightActive = true;
            _isDayHighlight = true;
        }
        
        [Button]
        public void ApplyMouseOverHighlightNight()
        {
            if (!_availableHighlightActive)
            {
                SetHighlightingToMouseOverNight();
                
                gameObject.SetActive(true);
            }
            else
            {
                var customFloorMaterial = floorRenderer.material;
                customFloorMaterial.SetFloat(CircleDiameter, floorCircleDiameterOnMouseOverAndAvailability);
                var customWallMaterial = wallRenderer.material;
                customWallMaterial.SetFloat(HighlightLevel, wallHighlightLevelOnMouseOverAndAvailability);
            }
            
            _mouseOverHighlightActive = true;
            _isDayHighlight = false;
        }

        [Button]
        public void ApplyAvailabilityHighlightNight()
        {
            SetHighlightingToAvailabilityDefault();

            if (_mouseOverHighlightActive)
            {
                var customFloorMaterial = floorRenderer.material;
                customFloorMaterial.SetFloat(CircleDiameter, floorCircleDiameterOnMouseOverAndAvailability);
                var customWallMaterial = wallRenderer.material;
                customWallMaterial.SetFloat(HighlightLevel, wallHighlightLevelOnMouseOverAndAvailability);
            }

            gameObject.SetActive(true);
            _availableHighlightActive = true;
            _isDayHighlight = false;
        }

        [Button]
        public void DisableMouseOverHighlight()
        {
            _mouseOverHighlightActive = false;
            if(_isDayHighlight || !_availableHighlightActive)
                gameObject.SetActive(false);
            
            SetHighlightingToAvailabilityDefault();
        }

        [Button]
        public void DisableAvailabilityHighlight()
        {
            _availableHighlightActive = false;
            if(!_mouseOverHighlightActive)
                gameObject.SetActive(false);
            
            SetHighlightingToMouseOverNight();
        }

        private void SetHighlightingToAvailabilityDefault()
        {
            floorRenderer.material = floorHighlightMaterialNight;
            wallRenderer.material = wallHighlightMaterialNight;
        }

        private void SetHighlightingToMouseOverNight()
        {
            var customFloorMaterial = new Material(floorHighlightMaterialNight);
            customFloorMaterial.SetFloat(CircleDiameter, floorCircleDiameterOnMouseOver);
            floorRenderer.material = customFloorMaterial;

            var customWallMaterial = new Material(wallHighlightMaterialNight);
            customWallMaterial.SetFloat(HighlightLevel, wallHighlightLevelOnMouseOver);
            wallRenderer.material = wallHighlightMaterialNight;
        }
    }
}