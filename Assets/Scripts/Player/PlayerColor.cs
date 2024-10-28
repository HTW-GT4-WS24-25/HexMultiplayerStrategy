using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "New PlayerColor", menuName = "Create Player Color")]
    public class PlayerColor : ScriptableObject
    {
        public ColorType colorType;
        public Color playerColor;
        public Material unitMaterial;

        public enum ColorType
        {
            None = 0,
            Red = 1,
            Blue = 2,
            Green = 3
        }
    }
}
