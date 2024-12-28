using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Unit.Model
{
    public class AnimalOutline : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Renderer[] renderers;
        
        private const int HoverLayerIndex = 8;
        private const int SelectedLayerIndex = 9;
        
        [Button]
        public void ActivateHoverOutline()
        {
            AddLayerToRenderers(HoverLayerIndex);
        }

        [Button]
        public void DeactivateOverOutline()
        {
            RemoveLayerFromRenderers(HoverLayerIndex);
        }

        [Button]
        public void ActivateSelectedOutline()
        {
            AddLayerToRenderers(SelectedLayerIndex);
        }

        [Button]
        public void DeactivateSelectedOutline()
        {
            RemoveLayerFromRenderers(SelectedLayerIndex);
        }

        [Button]
        public void DeactivateOutlines()
        {
            RemoveLayerFromRenderers(HoverLayerIndex);
            RemoveLayerFromRenderers(SelectedLayerIndex);
        }

        private void AddLayerToRenderers(int layerIndex)
        {
            foreach (var rend in renderers)
            {
                rend.renderingLayerMask |= 1U << layerIndex;
            }
        }

        private void RemoveLayerFromRenderers(int layerIndex)
        {
            foreach (var rend in renderers)
            {
                rend.renderingLayerMask &= ~(1U << layerIndex);
            }
        }
    }
}