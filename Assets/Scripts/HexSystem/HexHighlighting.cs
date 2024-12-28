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
        [ColorUsage(true, true)]
        [SerializeField] private Color floorMouseOverColorNight;
        [ColorUsage(true, true)]
        [SerializeField] private Color wallMouseOverColorNight;
        
        private static readonly int HighlightColorId = Shader.PropertyToID("_Highlight_Color");

        [Button]
        public void ApplyMouseOverHighlightDay()
        {
            floorRenderer.material = floorHighlightMaterialDay;
            wallRenderer.material = wallHighlightMaterialDay;
            gameObject.SetActive(true);
        }
        
        [Button]
        public void ApplyMouseOverHighlightNight()
        {
            Debug.Assert(gameObject.activeSelf, "Tried to set mouseOver highlight on a non-active hex highlight object.");
            floorRenderer.material = GetColoredMaterial(floorHighlightMaterialNight, floorMouseOverColorNight);
            wallRenderer.material = GetColoredMaterial(wallHighlightMaterialNight, wallMouseOverColorNight);
        }

        [Button]
        public void ApplyAvailabilityHighlightNight()
        {
            floorRenderer.material = floorHighlightMaterialNight;
            wallRenderer.material = wallHighlightMaterialNight;
            gameObject.SetActive(true);
        }

        [Button]
        public void DisableMouseOverNightHighlight()
        {
            ApplyAvailabilityHighlightNight();
        }

        [Button]
        public void DisableHighlight()
        {
            gameObject.SetActive(false);
        }

        private Material GetColoredMaterial(Material material, Color highlightColor)
        {
            var customMaterial = new Material(material);
            customMaterial.SetColor(HighlightColorId, highlightColor);
            return customMaterial;
        }
    }
}