using HexSystem;
using Input;
using UnityEngine;

public class MouseOverHighlighter : MonoBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] LayerMask groundLayer;

    private GameObject lastHexagonObjectHit;
    
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(inputReader.MousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
        {
            ProcessGroundHit(hit);   
        }
        else if (lastHexagonObjectHit is not null)
        {
            lastHexagonObjectHit.GetComponent<Hexagon>().DisableMouseOverHighlight();
        }
    }

    private void ProcessGroundHit(RaycastHit hit)
    {
        if(hit.collider.gameObject == lastHexagonObjectHit)
            return;
        
        if(lastHexagonObjectHit is not null)
            lastHexagonObjectHit.GetComponent<Hexagon>().DisableMouseOverHighlight();
        
        if (hit.collider.transform.parent.TryGetComponent<Hexagon>(out var hexagon))
        {
            Debug.Log("Hexagon hit!");
            hexagon.HighlightOnMouseOver();
            lastHexagonObjectHit = hexagon.gameObject;
        }
    }
}
