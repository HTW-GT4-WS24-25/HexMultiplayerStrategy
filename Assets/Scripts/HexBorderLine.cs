using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LineRenderer))]
public class HexBorderLine : MonoBehaviour
{
    [SerializeField] private Transform[] lineNodes;
    [SerializeField] private float highlightWidthMultiplier = 0.05f;
    
    private LineRenderer _lineRenderer;


    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 6;
        _lineRenderer.SetPositions(lineNodes.Select(node => node.position).ToArray());
    }

    public void HighlightBorderWithColor(PlayerColor playerColor)
    {
        _lineRenderer.colorGradient = new Gradient();
        _lineRenderer.material = playerColor.hexBorderMaterial;
        _lineRenderer.widthMultiplier = highlightWidthMultiplier;
    }
}
