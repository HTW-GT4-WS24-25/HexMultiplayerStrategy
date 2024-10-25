using UnityEngine;

namespace Unit
{
    [RequireComponent(typeof(LineRenderer))]
    public class UnitTravelLine : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GameObject lineEndPoint;
        [SerializeField] private LineRenderer lineRenderer;
    
        [Header("Settings")]
        [SerializeField] private Vector3 lineOffset;
    
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void SetAllPositions(Vector3[] positions)
        {
            lineRenderer.positionCount = positions.Length;
            for (var i = 0; i < positions.Length; i++)
            {
                lineRenderer.SetPosition(i, positions[i] + lineOffset);
            }
        
            UpdateEndPointPosition();
            gameObject.SetActive(true);
        }

        public void SetFirstNodePosition(Vector3 position)
        {
            lineRenderer.SetPosition(0, position + lineOffset);
        
            UpdateEndPointPosition();
        }

        private void UpdateEndPointPosition()
        {
            if (lineRenderer.positionCount <= 1)
                return;
        
            lineEndPoint.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        }
    }
}
