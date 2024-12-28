using System.Collections.Generic;
using Core.Input;
using UnityEngine;

namespace Core.HexSystem.VFX
{
    public class MouseOverHighlighter : MonoBehaviour
    {
        [SerializeField] InputReader inputReader;
        [SerializeField] LayerMask groundLayer;

        public List<Hexagon.Hexagon> ValidHexagons { get; set; }
    
        private GameObject lastHexagonObjectHighlighted;
    
        void Update()
        {
            var ray = Camera.main.ScreenPointToRay(inputReader.MousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
            {
                ProcessGroundHit(hit);   
            }
            else
            {
                lastHexagonObjectHighlighted?.GetComponent<Hexagon.Hexagon>().DisableMouseOverHighlight();
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
            lastHexagonObjectHighlighted?.GetComponent<Hexagon.Hexagon>().DisableMouseOverHighlight();
            lastHexagonObjectHighlighted = null;
        }

        private void ProcessGroundHit(RaycastHit hit)
        {
            if(hit.collider.gameObject == lastHexagonObjectHighlighted)
                return;

            lastHexagonObjectHighlighted?.GetComponent<Hexagon.Hexagon>().DisableMouseOverHighlight();

            if (!hit.collider.transform.parent.TryGetComponent<Hexagon.Hexagon>(out var hexagon)) 
                return;
        
            Debug.Log("Hexagon hit!");

            if (ValidHexagons != null && !ValidHexagons.Contains(hexagon))
                return;

            if (!hexagon.isTraversable)
                return;
        
            hexagon.HighlightOnMouseOver();
            lastHexagonObjectHighlighted = hexagon.gameObject;
        }
    }
}
