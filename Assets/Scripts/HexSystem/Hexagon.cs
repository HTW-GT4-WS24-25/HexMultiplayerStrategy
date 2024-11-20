using Player;
using UnityEngine;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine hexBorderLine;
        
        public bool isTraversable;
        
        public AxialCoordinates Coordinates { get; private set; }

        public void Initialize(AxialCoordinates coordinate)
        {
            Coordinates = coordinate;
            
            if(hexBorderLine != null)
                hexBorderLine.Initialize();
        }

        public void AdaptBorderToPlayerColor(PlayerColor playerColor)
        {
            hexBorderLine.HighlightBorderWithColor(playerColor);
        }
    }
}