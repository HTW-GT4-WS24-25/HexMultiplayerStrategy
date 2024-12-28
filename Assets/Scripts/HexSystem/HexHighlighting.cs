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
            var floorMouseOverNightMaterial = new Material(floorHighlightMaterialNight);
            floorMouseOverNightMaterial.SetColor(HighlightColorId, floorMouseOverColorNight);
            floorRenderer.material = floorMouseOverNightMaterial;
            
            var wallMouseOverNightMaterial =new Material(wallHighlightMaterialNight);
            wallMouseOverNightMaterial.SetColor(HighlightColorId, wallMouseOverColorNight);
            wallRenderer.material = wallMouseOverNightMaterial;
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
    }
}