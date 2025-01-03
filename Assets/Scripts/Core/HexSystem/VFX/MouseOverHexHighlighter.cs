using System.Collections.Generic;
using Core.HexSystem.Hex;
using Core.Input;
using UnityEngine;

namespace Core.HexSystem.VFX
{
    public class MouseOverHexHighlighter : MonoBehaviour
    {
        [SerializeField] InputReader inputReader;
        [SerializeField] LayerMask groundLayer;

        public List<Hexagon> ValidHexagons { get; set; }

        private Hexagon _currentHoveredHexagon;
        private Collider _currentHoveredHexagonCollider;
    
        void Update()
        {
            var ray = Camera.main.ScreenPointToRay(inputReader.MousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
            {
                ProcessGroundHit(hit);   
            }
            else
            {
                DisableCurrentMouseOverHighlight();
            }
        }

        public void Enable()
        {
            ValidHexagons = null;
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
            DisableCurrentMouseOverHighlight();
        }

        private void ProcessGroundHit(RaycastHit hit)
        {
            if(hit.collider == _currentHoveredHexagonCollider)
                return;

            DisableCurrentMouseOverHighlight();

            if (!hit.collider.transform.parent.TryGetComponent<Hexagon>(out var hexagon)) 
                return;
        
            Debug.Log("Hexagon hit!");

            if (ValidHexagons != null && !ValidHexagons.Contains(hexagon))
                return;

            if (!hexagon.isTraversable)
                return;
        
            hexagon.HighlightOnMouseOver();
            _currentHoveredHexagon = hexagon;
            _currentHoveredHexagonCollider = hit.collider;
        }

        private void DisableCurrentMouseOverHighlight()
        {
            _currentHoveredHexagon?.DisableMouseOverHighlight();
            _currentHoveredHexagon = null;
            _currentHoveredHexagonCollider = null;
        }
    }
}
