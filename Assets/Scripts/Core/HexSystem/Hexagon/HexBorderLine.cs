using System.Linq;
using Core.Player;
using UnityEngine;

namespace Core.HexSystem.Hexagon
{
    [RequireComponent(typeof(LineRenderer))]
    public class HexBorderLine : MonoBehaviour
    {
        [SerializeField] private Transform[] lineNodes;
        [SerializeField] private float highlightWidthMultiplier = 0.05f;

        [SerializeField] private Material validForPlacementMaterial;
        [SerializeField] private Material defaultMaterial;
        public PlayerColor ownerColor;
    
        private LineRenderer _lineRenderer;
    
        public void Initialize()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 6;
            _lineRenderer.SetPositions(lineNodes.Select(node => node.position).ToArray());
        }

        public void HighlightBorderWithColor(PlayerColor playerColor)
        {
            _lineRenderer.colorGradient = new Gradient();
            _lineRenderer.material = playerColor.HexBorderMaterial;
            _lineRenderer.widthMultiplier = highlightWidthMultiplier;
        
            ownerColor = playerColor;
        }
    }
}
