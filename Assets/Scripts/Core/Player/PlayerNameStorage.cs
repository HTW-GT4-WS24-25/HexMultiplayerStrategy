using UnityEngine;

namespace Core.Player
{
    public static class PlayerNameStorage
    {
        private const string PlayerNameKey = "PlayerName";
        
        public static string Name
        {
            get => _name ?? PlayerPrefs.GetString(PlayerNameKey, "");

            set
            {
                _name = value;
                PlayerPrefs.SetString(PlayerNameKey, _name);
            }
        }
        
        private static string _name;
    }
}