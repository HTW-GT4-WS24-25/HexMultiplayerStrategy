using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HexBorderLine : MonoBehaviour
{
    [SerializeField] private Transform[] lineNodes;

    private LineRenderer _lineRenderer;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 6;
        _lineRenderer.SetPositions(lineNodes.Select(node => node.position).ToArray());
    }
}
