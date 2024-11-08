using Player;
using UnityEngine;

namespace HexSystem
{
    public class ClientHexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine hexBorderLine;
        
        public bool isTraversable;
        
        public AxialCoordinates Coordinates { get; private set; }

        public void Initialize(AxialCoordinates coordinates, bool asServer = false)
        {
            Coordinates = coordinates;
            
            if(hexBorderLine != null)
                hexBorderLine.Initialize();

            if (asServer)
                gameObject.AddComponent<ServerHexagon>();
        }

        public void AdaptBorderToPlayerColor(PlayerColor playerColor)
        {
            hexBorderLine.HighlightBorderWithColor(playerColor);
        }
    }
}