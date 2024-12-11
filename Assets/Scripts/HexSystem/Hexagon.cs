using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine hexBorderLine;
        
        public AxialCoordinates Coordinates { get; private set; }
        
        private Topping _topping;
        
        public bool IsTraversable => _topping == null || _topping.IsTraversable;

        public void Initialize(AxialCoordinates coordinate)
        {
            Coordinates = coordinate;
            
            if(hexBorderLine != null)
                hexBorderLine.Initialize();
        }

        public void SetTopping(Topping topping)
        {
            _topping = topping;
            Instantiate(_topping, transform.position, QuaternionUtils.GetRandomHexRotation());
        }

        public void AdaptBorderToPlayerColor(PlayerColor playerColor)
        {
            hexBorderLine.HighlightBorderWithColor(playerColor);
        }
    }
}