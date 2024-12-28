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
        
        private Topping _topping;
        
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

        public void SetTopping(Topping toppingPrefab)
        {
            if (_topping != null)
                Destroy(_topping.gameObject);
            
            if (toppingPrefab != null)
                _topping = Instantiate(toppingPrefab, transform.position, QuaternionUtils.GetRandomHexRotation());
        }

        public void SetToppingLevel(int level)
        {
            _topping.SetLevel(level);
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