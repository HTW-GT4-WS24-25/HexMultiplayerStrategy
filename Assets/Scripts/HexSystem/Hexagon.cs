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
        
        private GameObject _topping;
        
        public bool isTraversable;

        public void Initialize(AxialCoordinates coordinate)
        {
            Coordinates = coordinate;
            
            if (hexBorderLine != null)
                hexBorderLine.Initialize();
        }

        public void SetTopping(GameObject toppingPrefab)
        {
            if (_topping != null)
                Destroy(_topping.gameObject);
            
            if (toppingPrefab != null)
                _topping = Instantiate(toppingPrefab, transform.position, QuaternionUtils.GetRandomHexRotation());
        }

        public void AdaptBorderToPlayerColor(PlayerColor playerColor)
        {
            hexBorderLine.HighlightBorderWithColor(playerColor);
        }
    }
}