using Player;
using UnityEngine;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine hexBorderLine;
        [SerializeField] private HexagonCenterTriggerArea centerTriggerArea;
        
        public bool isTraversable;
        
        public AxialCoordinates Coordinates { get; private set; }

        public void Initialize(AxialCoordinates coordinate, bool asServer)
        {
            Coordinates = coordinate;
            
            if(hexBorderLine != null)
                hexBorderLine.Initialize();

            if (!asServer && centerTriggerArea != null)
                Destroy(centerTriggerArea.gameObject);
        }

        public void AdaptBorderToPlayerColor(PlayerColor playerColor)
        {
            hexBorderLine.HighlightBorderWithColor(playerColor);
        }
    }
}