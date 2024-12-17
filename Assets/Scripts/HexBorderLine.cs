using System.Linq;
using GameEvents;
using Player;
using UnityEngine;

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
        ClientEvents.Hexagon.OnShowValidHexagonsForPlacement += ShowValidForPlacement;
        ClientEvents.Hexagon.OnHideValidHexagonsForPlacement += HideValidForPlacement;
        
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
    
    public void ShowValidForPlacement()
    {
        if (ownerColor)
        {
            _lineRenderer.material = validForPlacementMaterial;
        }
    }

    public void HideValidForPlacement()
    {
        if (ownerColor)
        {
            HighlightBorderWithColor(ownerColor);
        }
        else
        {
            _lineRenderer.material = defaultMaterial;
            _lineRenderer.widthMultiplier = 0.015f;
        }
    }
}
