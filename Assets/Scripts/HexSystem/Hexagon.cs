using GameEvents;
using Player;
using UnityEngine;
using Utils;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine hexBorderLine;
        [SerializeField] private HexHighlighting hexHighlighting;
        
        public AxialCoordinates Coordinates { get; private set; }
        
        private GameObject _topping;
        
        public bool isTraversable;

        private void OnEnable()
        {
            ClientEvents.Hexagon.OnHideValidHexagonsForPlacement += UnmarkAsValidForPlacement;
        }
        
        private void OnDisable()
        {
            ClientEvents.Hexagon.OnHideValidHexagonsForPlacement -= UnmarkAsValidForPlacement;
        }

        public void Initialize(AxialCoordinates coordinate)
        {
            Coordinates = coordinate;
            
            if (hexBorderLine != null)
                hexBorderLine.Initialize();
        }

        public void SetTopping(GameObject topping)
        {
            if (_topping != null)
                Destroy(_topping.gameObject);
            
            _topping = topping;
            
            if (_topping != null)
                Instantiate(_topping, transform.position, QuaternionUtils.GetRandomHexRotation());
        }

        public void AdaptBorderToPlayerColor(PlayerColor playerColor)
        {
            hexBorderLine.HighlightBorderWithColor(playerColor);
        }

        public void MarkAsValidForPlacement()
        {
            hexHighlighting.gameObject.SetActive(true);
        }

        public void UnmarkAsValidForPlacement()
        {
            hexHighlighting.gameObject.SetActive(false);
        }
    }
}