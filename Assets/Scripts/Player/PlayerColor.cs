using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "New PlayerColor", menuName = "Create Player Color")]
    public class PlayerColor : ScriptableObject
    {
        private static Dictionary<ColorType, PlayerColor> _playerColorStorage = new();
            
        public ColorType colorType;
        public Color baseColor;
        public Material unitMaterial;

        public static void AddColorToStorage(PlayerColor playerColor) => _playerColorStorage.Add(playerColor.colorType, playerColor);
        public static PlayerColor GetFromColorType(ColorType colorType) => _playerColorStorage[colorType];
        public static PlayerColor[] GetAll() => _playerColorStorage.Values.ToArray();

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
