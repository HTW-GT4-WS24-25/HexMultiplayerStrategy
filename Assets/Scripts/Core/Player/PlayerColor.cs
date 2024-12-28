using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Player
{
    [CreateAssetMenu(fileName = "New PlayerColor", menuName = "Create Player Color")]
    public class PlayerColor : ScriptableObject
    {
        [field: SerializeField] public ColorType Type { get; private set; }
        [field: SerializeField] public Color BaseColor { get; private set; }

        [field: SerializeField] public Material UnitColoringMaterial { get; private set; }
        [field: SerializeField] public Material HighlightedUnitMaterial { get; private set; }
        
        [field: SerializeField] public Material HexBorderMaterial { get; private set; }
        
        [field: SerializeField] public Material TravelLineMaterial { get; private set; }
        [field: SerializeField] public Material HighlightedTravelLineMaterial { get; private set; }
        [field: SerializeField] public Material TravelEndPointMaterial { get; private set; }
        [field: SerializeField] public Material HighlightedTravelEndPointMaterial { get; private set; }
        
        public static void AddColorToStorage(PlayerColor playerColor) => _playerColorStorage.Add(playerColor.Type, playerColor);
        public static PlayerColor GetFromColorType(ColorType colorType) => _playerColorStorage[colorType];
        public static PlayerColor[] GetAll() => _playerColorStorage.Values.ToArray();
        
        private static Dictionary<ColorType, PlayerColor> _playerColorStorage = new();

        public static ColorType IntToColorType(int color)
        {
            if (Enum.IsDefined(typeof(ColorType), color))
            {
                return (ColorType)color;
            }
            
            throw new ArgumentException($"Tried to convert \"{color}\" to colorType!");
        }

        public enum ColorType
        {
            None = 0,
            Red = 1,
            Blue = 2,
            Green = 3
        }
    }
}
