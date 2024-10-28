using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LineRenderer))]
public class HexBorderLine : MonoBehaviour
{
    [SerializeField] private Transform[] lineNodes;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float highlightWidthMultiplier = 0.05f;
    
    private LineRenderer _lineRenderer;


    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 6;
        _lineRenderer.SetPositions(lineNodes.Select(node => node.position).ToArray());
    }

    public void HighlightBorderWithColor(Color color)
    {
        highlightMaterial.color = color;
        _lineRenderer.material = highlightMaterial;
        _lineRenderer.widthMultiplier = highlightWidthMultiplier;
    }
}
